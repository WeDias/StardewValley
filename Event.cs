// Decompiled with JetBrains decompiler
// Type: StardewValley.Event
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using xTile.Dimensions;

namespace StardewValley
{
  public class Event
  {
    protected static Dictionary<string, MethodInfo> _commandLookup;
    [InstancedStatic]
    protected static object[] _eventCommandArgs = new object[3];
    public const int weddingEventId = -2;
    private const float timeBetweenSpeech = 500f;
    private const float viewportMoveSpeed = 3f;
    public const string festivalTextureName = "Maps\\Festivals";
    public bool simultaneousCommand;
    public string[] eventCommands;
    public int currentCommand;
    public int farmerAddedSpeed;
    public int int_useMeForAnything;
    public int int_useMeForAnything2;
    public List<NPC> actors = new List<NPC>();
    public List<Object> props = new List<Object>();
    public List<Prop> festivalProps = new List<Prop>();
    public string messageToScreen;
    public string playerControlSequenceID;
    public string spriteTextToDraw;
    public bool showActiveObject;
    public bool continueAfterMove;
    public bool specialEventVariable1;
    public bool forked;
    public bool eventSwitched;
    public bool isFestival;
    public bool showGroundObjects = true;
    public bool isWedding;
    public bool doingSecretSanta;
    public bool showWorldCharacters;
    public bool isMemory;
    public bool ignoreObjectCollisions = true;
    protected bool _playerControlSequence;
    private Dictionary<string, Vector3> actorPositionsAfterMove;
    private float timeAccumulator;
    private float viewportXAccumulator;
    private float viewportYAccumulator;
    public float float_useMeForAnything;
    private Vector3 viewportTarget;
    private Microsoft.Xna.Framework.Color previousAmbientLight;
    public List<NPC> npcsWithUniquePortraits = new List<NPC>();
    public LocationRequest exitLocation;
    public ICustomEventScript currentCustomEventScript;
    public List<Farmer> farmerActors = new List<Farmer>();
    private HashSet<long> festivalWinners = new HashSet<long>();
    public Action onEventFinished;
    protected bool _repeatingLocationSpecificCommand;
    private readonly LocalizedContentManager festivalContent = Game1.content.CreateTemporary();
    private GameLocation temporaryLocation;
    public Point playerControlTargetTile;
    private Texture2D _festivalTexture;
    public List<NPCController> npcControllers;
    public NPC secretSantaRecipient;
    public NPC mySecretSanta;
    public bool skippable;
    public int id;
    public List<Vector2> characterWalkLocations = new List<Vector2>();
    public bool ignoreTileOffsets;
    public Vector2 eventPositionTileOffset = Vector2.Zero;
    [NonInstancedStatic]
    public static HashSet<string> invalidFestivals = new HashSet<string>();
    private Dictionary<string, string> festivalData;
    private int oldShirt;
    private Microsoft.Xna.Framework.Color oldPants;
    private bool drawTool;
    public bool skipped;
    private bool waitingForMenuClose;
    private int oldTime;
    public List<TemporaryAnimatedSprite> underwaterSprites;
    public List<TemporaryAnimatedSprite> aboveMapSprites;
    private NPC festivalHost;
    private string hostMessage;
    public int festivalTimer;
    public int grangeScore = -1000;
    public bool grangeJudged;
    private int previousFacingDirection = -1;
    public Dictionary<string, Dictionary<ISalable, int[]>> festivalShops;
    private int previousAnswerChoice = -1;
    private bool startSecretSantaAfterDialogue;
    public bool specialEventVariable2;
    private List<Farmer> winners;

    public virtual void setupEventCommands()
    {
      if (Event._commandLookup != null)
        return;
      Event._commandLookup = new Dictionary<string, MethodInfo>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      MethodInfo[] array = ((IEnumerable<MethodInfo>) typeof (Event).GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (method_info => method_info.Name.StartsWith("command_"))).ToArray<MethodInfo>();
      foreach (MethodInfo methodInfo in array)
        Event._commandLookup.Add(methodInfo.Name.Substring("command_".Length), methodInfo);
      Console.WriteLine("setupEventCommands() registered '{0}' methods", (object) array.Length);
    }

    public virtual void tryEventCommand(GameLocation location, GameTime time, string[] split)
    {
      Event._eventCommandArgs[0] = (object) location;
      Event._eventCommandArgs[1] = (object) time;
      Event._eventCommandArgs[2] = (object) split;
      if (split.Length == 0)
        return;
      MethodInfo methodInfo;
      if (Event._commandLookup.TryGetValue(split[0], out methodInfo))
      {
        try
        {
          methodInfo.Invoke((object) this, Event._eventCommandArgs);
        }
        catch (TargetInvocationException ex)
        {
          this.LogErrorAndHalt(ex.InnerException);
        }
      }
      else
        Console.WriteLine("ERROR: Invalid command: " + split[0]);
    }

    public virtual void command_ignoreEventTileOffset(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.ignoreTileOffsets = true;
      ++this.CurrentCommand;
    }

    public virtual void command_move(GameLocation location, GameTime time, string[] split)
    {
      for (int index = 1; index < split.Length && split.Length - index >= 3; index += 4)
      {
        if (split[index].Contains("farmer") && !this.actorPositionsAfterMove.ContainsKey(split[index]))
        {
          Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(split[index], this.farmer);
          if (farmerNumberString != null)
          {
            farmerNumberString.canOnlyWalk = false;
            farmerNumberString.setRunning(false, true);
            farmerNumberString.canOnlyWalk = true;
            farmerNumberString.convertEventMotionCommandToMovement(new Vector2((float) Convert.ToInt32(split[index + 1]), (float) Convert.ToInt32(split[index + 2])));
            this.actorPositionsAfterMove.Add(split[index], this.getPositionAfterMove((Character) this.farmer, Convert.ToInt32(split[index + 1]), Convert.ToInt32(split[index + 2]), Convert.ToInt32(split[index + 3])));
          }
        }
        else
        {
          NPC actorByName = this.getActorByName(split[index]);
          string key = split[index].Equals("rival") ? Utility.getOtherFarmerNames()[0] : split[index];
          if (!this.actorPositionsAfterMove.ContainsKey(key))
          {
            actorByName.convertEventMotionCommandToMovement(new Vector2((float) Convert.ToInt32(split[index + 1]), (float) Convert.ToInt32(split[index + 2])));
            this.actorPositionsAfterMove.Add(key, this.getPositionAfterMove((Character) actorByName, Convert.ToInt32(split[index + 1]), Convert.ToInt32(split[index + 2]), Convert.ToInt32(split[index + 3])));
          }
        }
      }
      if (((IEnumerable<string>) split).Last<string>().Equals("true"))
      {
        this.continueAfterMove = true;
        ++this.CurrentCommand;
      }
      else
      {
        if (!((IEnumerable<string>) split).Last<string>().Equals("false"))
          return;
        this.continueAfterMove = false;
        if (split.Length != 2 || this.actorPositionsAfterMove.Count != 0)
          return;
        ++this.CurrentCommand;
      }
    }

    public virtual void command_speak(GameLocation location, GameTime time, string[] split)
    {
      if (this.skipped || Game1.dialogueUp)
        return;
      this.timeAccumulator += (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.timeAccumulator < 500.0)
        return;
      this.timeAccumulator = 0.0f;
      NPC actorByName = this.getActorByName(split[1]);
      if (actorByName == null)
        Game1.getCharacterFromName(split[1].Equals("rival") ? Utility.getOtherFarmerNames()[0] : split[1]);
      if (actorByName == null)
      {
        Game1.eventFinished();
      }
      else
      {
        int num = this.eventCommands[this.currentCommand].IndexOf('"');
        if (num > 0)
        {
          int length = this.eventCommands[this.CurrentCommand].Substring(num + 1).LastIndexOf('"');
          Game1.player.checkForQuestComplete(actorByName, -1, -1, (Item) null, (string) null, 5);
          if (Game1.NPCGiftTastes.ContainsKey(split[1]) && !Game1.player.friendshipData.ContainsKey(split[1]))
            Game1.player.friendshipData.Add(split[1], new Friendship(0));
          if (length > 0)
            actorByName.CurrentDialogue.Push(new Dialogue(this.eventCommands[this.CurrentCommand].Substring(num + 1, length), actorByName));
          else
            actorByName.CurrentDialogue.Push(new Dialogue("...", actorByName));
        }
        else
          actorByName.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString(split[2]), actorByName));
        Game1.drawDialogue(actorByName);
      }
    }

    public virtual void command_beginSimultaneousCommand(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.simultaneousCommand = true;
      ++this.CurrentCommand;
    }

    public virtual void command_endSimultaneousCommand(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.simultaneousCommand = false;
      ++this.CurrentCommand;
    }

    public virtual void command_minedeath(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.dialogueUp)
        return;
      Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + Game1.timeOfDay);
      int num1 = Math.Min(random.Next(Game1.player.Money / 20, Game1.player.Money / 4), 5000);
      int num2 = num1 - (int) ((double) Game1.player.LuckLevel * 0.01 * (double) num1);
      int sub1_1 = num2 - num2 % 100;
      int sub1_2 = 0;
      double num3 = 0.25 - (double) Game1.player.LuckLevel * 0.05 - Game1.player.DailyLuck;
      Game1.player.itemsLostLastDeath.Clear();
      for (int index = Game1.player.Items.Count - 1; index >= 0; --index)
      {
        if (Game1.player.Items[index] != null && (!(Game1.player.Items[index] is Tool) || Game1.player.Items[index] is MeleeWeapon && (Game1.player.Items[index] as MeleeWeapon).InitialParentTileIndex != 47 && (Game1.player.Items[index] as MeleeWeapon).InitialParentTileIndex != 4) && Game1.player.Items[index].canBeTrashed() && !(Game1.player.Items[index] is Ring) && random.NextDouble() < num3)
        {
          Item obj = Game1.player.Items[index];
          Game1.player.Items[index] = (Item) null;
          ++sub1_2;
          Game1.player.itemsLostLastDeath.Add(obj);
        }
      }
      Game1.player.Stamina = Math.Min(Game1.player.Stamina, 2f);
      Game1.player.Money = Math.Max(0, Game1.player.Money - sub1_1);
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1057") + " " + (sub1_1 <= 0 ? "" : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1058", (object) sub1_1)) + (sub1_2 > 0 ? (sub1_1 <= 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1060") + (sub1_2 == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1061") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1062", (object) sub1_2)) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1063") + (sub1_2 == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1061") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1062", (object) sub1_2))) : (sub1_1 <= 0 ? "" : ".")));
      List<string> list = ((IEnumerable<string>) this.eventCommands).ToList<string>();
      list.Insert(this.CurrentCommand + 1, "showItemsLost");
      this.eventCommands = list.ToArray();
    }

    public virtual void command_hospitaldeath(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.dialogueUp)
        return;
      Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + Game1.timeOfDay);
      int sub1_1 = 0;
      double num = 0.25 - (double) Game1.player.LuckLevel * 0.05 - Game1.player.DailyLuck;
      Game1.player.itemsLostLastDeath.Clear();
      for (int index = Game1.player.Items.Count - 1; index >= 0; --index)
      {
        if (Game1.player.Items[index] != null && (!(Game1.player.Items[index] is Tool) || Game1.player.Items[index] is MeleeWeapon && (Game1.player.Items[index] as MeleeWeapon).InitialParentTileIndex != 47 && (Game1.player.Items[index] as MeleeWeapon).InitialParentTileIndex != 4) && Game1.player.Items[index].canBeTrashed() && !(Game1.player.Items[index] is Ring) && random.NextDouble() < num)
        {
          Item obj = Game1.player.Items[index];
          Game1.player.Items[index] = (Item) null;
          ++sub1_1;
          Game1.player.itemsLostLastDeath.Add(obj);
        }
      }
      Game1.player.Stamina = Math.Min(Game1.player.Stamina, 2f);
      int sub1_2 = Math.Min(1000, Game1.player.Money);
      Game1.player.Money -= sub1_2;
      Game1.drawObjectDialogue((sub1_2 > 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1068", (object) sub1_2) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1070")) + (sub1_1 > 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1071") + (sub1_1 == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1061") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1062", (object) sub1_1)) : ""));
      List<string> list = ((IEnumerable<string>) this.eventCommands).ToList<string>();
      list.Insert(this.CurrentCommand + 1, "showItemsLost");
      this.eventCommands = list.ToArray();
    }

    public virtual void command_showItemsLost(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.activeClickableMenu != null)
        return;
      Game1.activeClickableMenu = (IClickableMenu) new ItemListMenu(Game1.content.LoadString("Strings\\UI:ItemList_ItemsLost"), Game1.player.itemsLostLastDeath.ToList<Item>());
    }

    public virtual void command_end(GameLocation location, GameTime time, string[] split) => this.endBehaviors(split, location);

    public virtual void command_locationSpecificCommand(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (split.Length <= 1)
        return;
      if (location.RunLocationSpecificEventCommand(this, split[1], !this._repeatingLocationSpecificCommand, ((IEnumerable<string>) split).Skip<string>(2).ToArray<string>()))
      {
        this._repeatingLocationSpecificCommand = false;
        ++this.CurrentCommand;
      }
      else
        this._repeatingLocationSpecificCommand = true;
    }

    public virtual void command_unskippable(GameLocation location, GameTime time, string[] split)
    {
      this.skippable = false;
      ++this.CurrentCommand;
    }

    public virtual void command_skippable(GameLocation location, GameTime time, string[] split)
    {
      this.skippable = true;
      ++this.CurrentCommand;
    }

    public virtual void command_emote(GameLocation location, GameTime time, string[] split)
    {
      bool flag = split.Length > 3;
      if (split[1].Contains("farmer"))
      {
        if (this.getFarmerFromFarmerNumberString(split[1], this.farmer) != null)
          this.farmer.doEmote(Convert.ToInt32(split[2]), !flag);
      }
      else
      {
        NPC actorByName = this.getActorByName(split[1]);
        if (!actorByName.isEmoting)
          actorByName.doEmote(Convert.ToInt32(split[2]), !flag);
      }
      if (!flag)
        return;
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_stopMusic(GameLocation location, GameTime time, string[] split)
    {
      Game1.changeMusicTrack("none", music_context: Game1.MusicContext.Event);
      ++this.CurrentCommand;
    }

    public virtual void command_playSound(GameLocation location, GameTime time, string[] split)
    {
      Game1.playSound(split[1]);
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_tossConcession(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      NPC actorByName = this.getActorByName(split[1]);
      int tilePosition = int.Parse(split[2]);
      Game1.playSound("dwop");
      location.temporarySprites.Add(new TemporaryAnimatedSprite()
      {
        texture = Game1.concessionsSpriteSheet,
        sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.concessionsSpriteSheet, tilePosition, 16, 16),
        animationLength = 1,
        totalNumberOfLoops = 1,
        motion = new Vector2(0.0f, -6f),
        acceleration = new Vector2(0.0f, 0.2f),
        interval = 1000f,
        scale = 4f,
        position = this.OffsetPosition(new Vector2(actorByName.Position.X, actorByName.Position.Y - 96f)),
        layerDepth = (float) actorByName.getStandingY() / 10000f
      });
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_pause(GameLocation location, GameTime time, string[] split)
    {
      if ((double) Game1.pauseTime > 0.0)
        return;
      Game1.pauseTime = (float) Convert.ToInt32(split[1]);
    }

    public virtual void command_resetVariable(GameLocation location, GameTime time, string[] split)
    {
      this.specialEventVariable1 = false;
      ++this.currentCommand;
    }

    public virtual void command_faceDirection(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Contains("farmer"))
      {
        Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(split[1], this.farmer);
        if (farmerNumberString != null)
        {
          farmerNumberString.FarmerSprite.StopAnimation();
          farmerNumberString.completelyStopAnimatingOrDoingAction();
          farmerNumberString.faceDirection(Convert.ToInt32(split[2]));
        }
      }
      else if (split[1].Contains("spouse"))
      {
        if (Game1.player.spouse != null && Game1.player.spouse.Length > 0 && this.getActorByName(Game1.player.spouse) != null && !Game1.player.isRoommate(Game1.player.spouse))
          this.getActorByName(Game1.player.spouse).faceDirection(Convert.ToInt32(split[2]));
      }
      else
        this.getActorByName(split[1])?.faceDirection(Convert.ToInt32(split[2]));
      if (split.Length == 3 && (double) Game1.pauseTime <= 0.0)
      {
        Game1.pauseTime = 500f;
      }
      else
      {
        if (split.Length <= 3)
          return;
        ++this.CurrentCommand;
        this.checkForNextCommand(location, time);
      }
    }

    public virtual void command_warp(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Contains("farmer"))
      {
        Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(split[1], this.farmer);
        if (farmerNumberString != null)
        {
          farmerNumberString.setTileLocation(this.OffsetTile(new Vector2((float) Convert.ToInt32(split[2]), (float) Convert.ToInt32(split[3]))));
          farmerNumberString.position.Y -= 16f;
          if (this.farmerActors.Contains(farmerNumberString))
            farmerNumberString.completelyStopAnimatingOrDoingAction();
        }
      }
      else if (split[1].Contains("spouse"))
      {
        if (Game1.player.spouse != null && Game1.player.spouse.Length > 0 && this.getActorByName(Game1.player.spouse) != null && !Game1.player.isRoommate(Game1.player.spouse))
        {
          if (this.npcControllers != null)
          {
            for (int index = this.npcControllers.Count - 1; index >= 0; --index)
            {
              if (this.npcControllers[index].puppet.Name.Equals(Game1.player.spouse))
                this.npcControllers.RemoveAt(index);
            }
          }
          this.getActorByName(Game1.player.spouse).Position = this.OffsetPosition(new Vector2((float) (Convert.ToInt32(split[2]) * 64), (float) (Convert.ToInt32(split[3]) * 64)));
        }
      }
      else
      {
        NPC actorByName = this.getActorByName(split[1]);
        if (actorByName != null)
        {
          actorByName.position.X = this.OffsetPositionX((float) (Convert.ToInt32(split[2]) * 64 + 4));
          actorByName.position.Y = this.OffsetPositionY((float) (Convert.ToInt32(split[3]) * 64));
        }
      }
      ++this.CurrentCommand;
      if (split.Length <= 4)
        return;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_speed(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Equals("farmer"))
        this.farmerAddedSpeed = Convert.ToInt32(split[2]);
      else
        this.getActorByName(split[1]).speed = Convert.ToInt32(split[2]);
      ++this.CurrentCommand;
    }

    public virtual void command_stopAdvancedMoves(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (((IEnumerable<string>) split).Count<string>() > 1)
      {
        if (split[1].Equals("next"))
        {
          foreach (NPCController npcController in this.npcControllers)
            npcController.destroyAtNextCrossroad();
        }
      }
      else
        this.npcControllers.Clear();
      ++this.CurrentCommand;
    }

    public virtual void command_doAction(GameLocation location, GameTime time, string[] split)
    {
      Location tile_location = new Location(this.OffsetTileX(Convert.ToInt32(split[1])), this.OffsetTileY(Convert.ToInt32(split[2])));
      Game1.hooks.OnGameLocation_CheckAction(location, tile_location, Game1.viewport, this.farmer, (Func<bool>) (() => location.checkAction(tile_location, Game1.viewport, this.farmer)));
      ++this.CurrentCommand;
    }

    public virtual void command_removeTile(GameLocation location, GameTime time, string[] split)
    {
      location.removeTile(this.OffsetTileX(Convert.ToInt32(split[1])), this.OffsetTileY(Convert.ToInt32(split[2])), split[3]);
      ++this.CurrentCommand;
    }

    public virtual void command_textAboveHead(GameLocation location, GameTime time, string[] split)
    {
      NPC actorByName = this.getActorByName(split[1]);
      if (actorByName != null)
      {
        int startIndex = this.eventCommands[this.CurrentCommand].IndexOf('"') + 1;
        int length = this.eventCommands[this.CurrentCommand].Substring(startIndex).LastIndexOf('"');
        actorByName.showTextAboveHead(this.eventCommands[this.CurrentCommand].Substring(startIndex, length));
      }
      ++this.CurrentCommand;
    }

    public virtual void command_showFrame(GameLocation location, GameTime time, string[] split)
    {
      if (split.Length > 2 && !split[2].Equals("flip") && !split[1].Contains("farmer"))
      {
        NPC actorByName = this.getActorByName(split[1]);
        if (actorByName != null)
        {
          int int32 = Convert.ToInt32(split[2]);
          if (split[1].Equals("spouse") && actorByName.Gender == 0 && int32 >= 36 && int32 <= 38)
            int32 += 12;
          actorByName.Sprite.CurrentFrame = int32;
        }
      }
      else
      {
        Farmer farmer = this.getFarmerFromFarmerNumberString(split[1], this.farmer);
        if (split.Length == 2)
          farmer = this.farmer;
        if (farmer != null)
        {
          if (split.Length > 2)
            split[1] = split[2];
          farmer.FarmerSprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(Convert.ToInt32(split[1]), 100, false, split.Length > 2)
          }.ToArray());
          farmer.FarmerSprite.loop = true;
          farmer.FarmerSprite.loopThisAnimation = true;
          farmer.FarmerSprite.PauseForSingleAnimation = true;
          farmer.Sprite.currentFrame = Convert.ToInt32(split[1]);
        }
      }
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_farmerAnimation(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.farmer.FarmerSprite.setCurrentSingleAnimation(Convert.ToInt32(split[1]));
      this.farmer.FarmerSprite.PauseForSingleAnimation = true;
      ++this.CurrentCommand;
    }

    public virtual void command_ignoreMovementAnimation(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      bool flag = true;
      if (split.Length > 2)
        split[2].Equals("true");
      if (split[1].Contains("farmer"))
      {
        Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(split[1], this.farmer);
        if (farmerNumberString != null)
          farmerNumberString.ignoreMovementAnimation = flag;
      }
      else
      {
        NPC actorByName = this.getActorByName(split[1].Replace('_', ' '));
        if (actorByName != null)
          actorByName.ignoreMovementAnimation = flag;
      }
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_animate(GameLocation location, GameTime time, string[] split)
    {
      int int32 = Convert.ToInt32(split[4]);
      bool flip = split[2].Equals("true");
      bool flag = split[3].Equals("true");
      List<FarmerSprite.AnimationFrame> animation = new List<FarmerSprite.AnimationFrame>();
      for (int index = 5; index < split.Length; ++index)
        animation.Add(new FarmerSprite.AnimationFrame(Convert.ToInt32(split[index]), int32, false, flip));
      if (split[1].Contains("farmer"))
      {
        Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(split[1], this.farmer);
        if (farmerNumberString != null)
        {
          farmerNumberString.FarmerSprite.setCurrentAnimation(animation.ToArray());
          farmerNumberString.FarmerSprite.loop = true;
          farmerNumberString.FarmerSprite.loopThisAnimation = flag;
          farmerNumberString.FarmerSprite.PauseForSingleAnimation = true;
        }
      }
      else
      {
        NPC actorByName = this.getActorByName(split[1].Replace('_', ' '));
        if (actorByName != null)
        {
          actorByName.Sprite.setCurrentAnimation(animation);
          actorByName.Sprite.loop = flag;
        }
      }
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_stopAnimation(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Contains("farmer"))
      {
        Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(split[1], this.farmer);
        if (farmerNumberString != null)
        {
          farmerNumberString.completelyStopAnimatingOrDoingAction();
          farmerNumberString.Halt();
          farmerNumberString.FarmerSprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
          switch (farmerNumberString.FacingDirection)
          {
            case 0:
              farmerNumberString.FarmerSprite.setCurrentSingleFrame(12);
              break;
            case 1:
              farmerNumberString.FarmerSprite.setCurrentSingleFrame(6);
              break;
            case 2:
              farmerNumberString.FarmerSprite.setCurrentSingleFrame(0);
              break;
            case 3:
              farmerNumberString.FarmerSprite.setCurrentSingleFrame(6, flip: true);
              break;
          }
        }
      }
      else
      {
        NPC actorByName = this.getActorByName(split[1]);
        if (actorByName != null)
        {
          actorByName.Sprite.StopAnimation();
          if (split.Length > 2)
          {
            actorByName.Sprite.currentFrame = Convert.ToInt32(split[2]);
            actorByName.Sprite.UpdateSourceRect();
          }
        }
      }
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_showRivalFrame(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.getActorByName("rival").Sprite.currentFrame = Convert.ToInt32(split[1]);
      ++this.CurrentCommand;
    }

    public virtual void command_weddingSprite(GameLocation location, GameTime time, string[] split)
    {
      this.getActorByName("WeddingOutfits").Sprite.currentFrame = Convert.ToInt32(split[1]);
      ++this.CurrentCommand;
    }

    public virtual void command_changeLocation(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.changeLocation(split[1], this.farmer.getTileX(), this.farmer.getTileY(), onComplete: ((Action) (() => ++this.CurrentCommand)));
    }

    public virtual void command_halt(GameLocation location, GameTime time, string[] split)
    {
      foreach (Character actor in this.actors)
        actor.Halt();
      this.farmer.Halt();
      ++this.CurrentCommand;
      this.continueAfterMove = false;
      this.actorPositionsAfterMove.Clear();
    }

    public virtual void command_message(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.dialogueUp || Game1.activeClickableMenu != null)
        return;
      int startIndex = this.eventCommands[this.CurrentCommand].IndexOf('"') + 1;
      int num = this.eventCommands[this.CurrentCommand].LastIndexOf('"');
      if (num > 0 && num > startIndex)
        Game1.drawDialogueNoTyping(Game1.parseText(this.eventCommands[this.CurrentCommand].Substring(startIndex, num - startIndex)));
      else
        Game1.drawDialogueNoTyping("...");
    }

    public virtual void command_addCookingRecipe(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      Game1.player.cookingRecipes.Add(this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf(' ') + 1), 0);
      ++this.CurrentCommand;
    }

    public virtual void command_itemAboveHead(GameLocation location, GameTime time, string[] split)
    {
      if (split.Length > 1 && split[1].Equals("pan"))
        this.farmer.holdUpItemThenMessage((Item) new Pan());
      else if (split.Length > 1 && split[1].Equals("hero"))
        this.farmer.holdUpItemThenMessage((Item) new Object(Vector2.Zero, 116));
      else if (split.Length > 1 && split[1].Equals("sculpture"))
        this.farmer.holdUpItemThenMessage((Item) new Furniture(1306, Vector2.Zero));
      else if (split.Length > 1 && split[1].Equals("samBoombox"))
        this.farmer.holdUpItemThenMessage((Item) new Furniture(1309, Vector2.Zero));
      else if (split.Length > 1 && split[1].Equals("joja"))
        this.farmer.holdUpItemThenMessage((Item) new Object(Vector2.Zero, 117));
      else if (split.Length > 1 && split[1].Equals("slimeEgg"))
        this.farmer.holdUpItemThenMessage((Item) new Object(680, 1));
      else if (split.Length > 1 && split[1].Equals("rod"))
        this.farmer.holdUpItemThenMessage((Item) new FishingRod());
      else if (split.Length > 1 && split[1].Equals("sword"))
        this.farmer.holdUpItemThenMessage((Item) new MeleeWeapon(0));
      else if (split.Length > 1 && split[1].Equals("ore"))
        this.farmer.holdUpItemThenMessage((Item) new Object(378, 1), false);
      else if (split.Length > 1 && split[1].Equals("pot"))
        this.farmer.holdUpItemThenMessage((Item) new Object(Vector2.Zero, 62), false);
      else if (split.Length > 1 && split[1].Equals("jukebox"))
        this.farmer.holdUpItemThenMessage((Item) new Object(Vector2.Zero, 209), false);
      else
        this.farmer.holdUpItemThenMessage((Item) null, false);
      ++this.CurrentCommand;
    }

    public virtual void command_addCraftingRecipe(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (!Game1.player.craftingRecipes.ContainsKey(this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf(' ') + 1)))
        Game1.player.craftingRecipes.Add(this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf(' ') + 1), 0);
      ++this.CurrentCommand;
    }

    public virtual void command_hostMail(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.IsMasterGame && !Game1.player.hasOrWillReceiveMail(split[1]))
        Game1.addMailForTomorrow(split[1]);
      ++this.CurrentCommand;
    }

    public virtual void command_mail(GameLocation location, GameTime time, string[] split)
    {
      if (!Game1.player.hasOrWillReceiveMail(split[1]))
        Game1.addMailForTomorrow(split[1]);
      ++this.CurrentCommand;
    }

    public virtual void command_shake(GameLocation location, GameTime time, string[] split)
    {
      this.getActorByName(split[1]).shake(Convert.ToInt32(split[2]));
      ++this.CurrentCommand;
    }

    public virtual void command_temporarySprite(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      location.TemporarySprites.Add(new TemporaryAnimatedSprite(Convert.ToInt32(split[3]), this.OffsetPosition(new Vector2((float) (Convert.ToInt32(split[1]) * 64), (float) (Convert.ToInt32(split[2]) * 64))), Microsoft.Xna.Framework.Color.White, Convert.ToInt32(split[4]), split.Length > 6 && split[6] == "true", split.Length > 5 ? (float) Convert.ToInt32(split[5]) : 300f, sourceRectWidth: 64, layerDepth: (split.Length > 7 ? (float) Convert.ToDouble(split[7]) : -1f)));
      ++this.CurrentCommand;
    }

    public virtual void command_removeTemporarySprites(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      location.TemporarySprites.Clear();
      ++this.CurrentCommand;
    }

    public virtual void command_null(GameLocation location, GameTime time, string[] split)
    {
    }

    public virtual void command_specificTemporarySprite(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.addSpecificTemporarySprite(split[1], location, split);
      ++this.CurrentCommand;
    }

    public virtual void command_playMusic(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Equals("samBand"))
      {
        if (Game1.player.DialogueQuestionsAnswered.Contains(78))
          Game1.changeMusicTrack("shimmeringbastion", music_context: Game1.MusicContext.Event);
        else if (Game1.player.DialogueQuestionsAnswered.Contains(79))
          Game1.changeMusicTrack("honkytonky", music_context: Game1.MusicContext.Event);
        else if (Game1.player.DialogueQuestionsAnswered.Contains(77))
          Game1.changeMusicTrack("heavy", music_context: Game1.MusicContext.Event);
        else
          Game1.changeMusicTrack("poppy", music_context: Game1.MusicContext.Event);
      }
      else if ((double) Game1.options.musicVolumeLevel > 0.0)
      {
        StringBuilder stringBuilder = new StringBuilder(split[1]);
        for (int index = 2; index < split.Length; ++index)
          stringBuilder.Append(" " + split[index]);
        Game1.changeMusicTrack(stringBuilder.ToString(), music_context: Game1.MusicContext.Event);
      }
      ++this.CurrentCommand;
    }

    public virtual void command_nameSelect(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.nameSelectUp)
        return;
      Game1.showNameSelectScreen(split[1]);
    }

    public virtual void command_makeInvisible(GameLocation location, GameTime time, string[] split)
    {
      if (((IEnumerable<string>) split).Count<string>() == 3)
      {
        int x = this.OffsetTileX(Convert.ToInt32(split[1]));
        int y = this.OffsetTileY(Convert.ToInt32(split[2]));
        Object objectAtTile = location.getObjectAtTile(x, y);
        if (objectAtTile != null)
          objectAtTile.isTemporarilyInvisible = true;
      }
      else
      {
        int num1 = this.OffsetTileX(Convert.ToInt32(split[1]));
        int num2 = this.OffsetTileY(Convert.ToInt32(split[2]));
        int int32_1 = Convert.ToInt32(split[3]);
        int int32_2 = Convert.ToInt32(split[4]);
        for (int x = num1; x < num1 + int32_1; ++x)
        {
          for (int y = num2; y < num2 + int32_2; ++y)
          {
            Object objectAtTile = location.getObjectAtTile(x, y);
            if (objectAtTile != null)
              objectAtTile.isTemporarilyInvisible = true;
            else if (location.terrainFeatures.ContainsKey(new Vector2((float) x, (float) y)))
              location.terrainFeatures[new Vector2((float) x, (float) y)].isTemporarilyInvisible = true;
          }
        }
      }
      ++this.CurrentCommand;
    }

    public virtual void command_addObject(GameLocation location, GameTime time, string[] split)
    {
      float num = (float) (this.OffsetTileY(Convert.ToInt32(split[2])) * 64) / 10000f;
      if (split.Length > 4)
        num = Convert.ToSingle(split[4]);
      location.TemporarySprites.Add(new TemporaryAnimatedSprite(Convert.ToInt32(split[3]), 9999f, 1, 9999, this.OffsetPosition(new Vector2((float) Convert.ToInt32(split[1]), (float) Convert.ToInt32(split[2])) * 64f), false, false)
      {
        layerDepth = num
      });
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_addBigProp(GameLocation location, GameTime time, string[] split)
    {
      this.props.Add(new Object(this.OffsetTile(new Vector2((float) Convert.ToInt32(split[1]), (float) Convert.ToInt32(split[2]))), Convert.ToInt32(split[3])));
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_addFloorProp(GameLocation location, GameTime time, string[] split) => this.command_addProp(location, time, split);

    public virtual void command_addProp(GameLocation location, GameTime time, string[] split)
    {
      int tileX1 = this.OffsetTileX(Convert.ToInt32(split[2]));
      int tileY1 = this.OffsetTileY(Convert.ToInt32(split[3]));
      int int32_1 = Convert.ToInt32(split[1]);
      int tilesWideSolid = split.Length > 4 ? Convert.ToInt32(split[4]) : 1;
      int tilesHighDraw = split.Length > 5 ? Convert.ToInt32(split[5]) : 1;
      int tilesHighSolid = split.Length > 6 ? Convert.ToInt32(split[6]) : tilesHighDraw;
      bool solid = !split[0].Contains("Floor");
      this.festivalProps.Add(new Prop(this.festivalTexture, int32_1, tilesWideSolid, tilesHighSolid, tilesHighDraw, tileX1, tileY1, solid));
      if (split.Length > 7)
      {
        int int32_2 = Convert.ToInt32(split[7]);
        for (int tileX2 = tileX1 + int32_2; tileX2 != tileX1; tileX2 -= Math.Sign(int32_2))
          this.festivalProps.Add(new Prop(this.festivalTexture, int32_1, tilesWideSolid, tilesHighSolid, tilesHighDraw, tileX2, tileY1, solid));
      }
      if (split.Length > 8)
      {
        int int32_3 = Convert.ToInt32(split[8]);
        for (int tileY2 = tileY1 + int32_3; tileY2 != tileY1; tileY2 -= Math.Sign(int32_3))
          this.festivalProps.Add(new Prop(this.festivalTexture, int32_1, tilesWideSolid, tilesHighSolid, tilesHighDraw, tileX1, tileY2, solid));
      }
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_addToTable(GameLocation location, GameTime time, string[] split)
    {
      if (location is FarmHouse)
        (location as FarmHouse).furniture[0].heldObject.Value = new Object(Vector2.Zero, Convert.ToInt32(split[3]), 1);
      else
        location.objects[this.OffsetTile(new Vector2((float) Convert.ToInt32(split[1]), (float) Convert.ToInt32(split[2])))].heldObject.Value = new Object(Vector2.Zero, Convert.ToInt32(split[3]), 1);
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_removeObject(GameLocation location, GameTime time, string[] split)
    {
      Vector2 other = this.OffsetPosition(new Vector2((float) Convert.ToInt32(split[1]), (float) Convert.ToInt32(split[2])) * 64f);
      for (int index = location.temporarySprites.Count - 1; index >= 0; --index)
      {
        if (location.temporarySprites[index].position.Equals(other))
        {
          location.temporarySprites.RemoveAt(index);
          break;
        }
      }
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_glow(GameLocation location, GameTime time, string[] split)
    {
      bool hold = false;
      if (split.Length > 4 && split[4].Equals("true"))
        hold = true;
      Game1.screenGlowOnce(new Microsoft.Xna.Framework.Color(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), Convert.ToInt32(split[3])), hold);
      ++this.CurrentCommand;
    }

    public virtual void command_stopGlowing(GameLocation location, GameTime time, string[] split)
    {
      Game1.screenGlowUp = false;
      Game1.screenGlowHold = false;
      ++this.CurrentCommand;
    }

    public virtual void command_addQuest(GameLocation location, GameTime time, string[] split)
    {
      Game1.player.addQuest(Convert.ToInt32(split[1]));
      ++this.CurrentCommand;
    }

    public virtual void command_removeQuest(GameLocation location, GameTime time, string[] split)
    {
      Game1.player.removeQuest(Convert.ToInt32(split[1]));
      ++this.CurrentCommand;
    }

    public virtual void command_awardFestivalPrize(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (this.festivalWinners.Contains(Game1.player.UniqueMultiplayerID))
      {
        string str = this.festivalData["file"];
        if (!(str == "spring13"))
        {
          if (!(str == "winter8"))
            return;
          if (!Game1.player.mailReceived.Contains("Ice Festival"))
          {
            if (Game1.activeClickableMenu == null)
              Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) new List<Item>()
              {
                (Item) new StardewValley.Objects.Hat(17),
                (Item) new Object(687, 1),
                (Item) new Object(691, 1),
                (Item) new Object(703, 1)
              }, (object) this).setEssential(true);
            Game1.player.mailReceived.Add("Ice Festival");
            ++this.CurrentCommand;
          }
          else
          {
            Game1.player.Money += 2000;
            Game1.playSound("money");
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1164"));
            this.CurrentCommand += 2;
          }
        }
        else if (!Game1.player.mailReceived.Contains("Egg Festival"))
        {
          if (Game1.activeClickableMenu == null)
            Game1.player.addItemByMenuIfNecessary((Item) new StardewValley.Objects.Hat(4));
          Game1.player.mailReceived.Add("Egg Festival");
          ++this.CurrentCommand;
          if (Game1.activeClickableMenu != null)
            return;
          ++this.CurrentCommand;
        }
        else
        {
          Game1.player.Money += 1000;
          Game1.playSound("money");
          this.CurrentCommand += 2;
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1159"));
        }
      }
      else if (split.Length > 1)
      {
        string lower = split[1].ToLower();
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(lower))
        {
          case 456875097:
            if (!(lower == "hero"))
              break;
            Game1.getSteamAchievement("Achievement_LocalLegend");
            Game1.player.addItemByMenuIfNecessary((Item) new Object(Vector2.Zero, 116));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 511772136:
            if (!(lower == "samboombox"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new Furniture(1309, Vector2.Zero));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 659772054:
            if (!(lower == "sword"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new MeleeWeapon(0));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 718310629:
            if (!(lower == "marniepainting"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new Furniture(1802, Vector2.Zero));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 890111454:
            if (!(lower == "rod"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new FishingRod());
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 1092255867:
            if (!(lower == "birdiereward"))
              break;
            List<Item> objList = new List<Item>();
            Game1.player.team.RequestLimitedNutDrops("Birdie", (GameLocation) null, 0, 0, 5, 5);
            if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotBirdieReward"))
              Game1.addMailForTomorrow("gotBirdieReward", true, true);
            ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 1331031788:
            if (!(lower == "pan"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new Pan());
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 1429431836:
            if (!(lower == "pot"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new Object(Vector2.Zero, 62));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 1716282848:
            if (!(lower == "slimeegg"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new Object(680, 1));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 2172047277:
            if (!(lower == "jukebox"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new Object(Vector2.Zero, 209));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 2264215128:
            if (!(lower == "qimilk"))
              break;
            if (!Game1.player.mailReceived.Contains("qiCave"))
            {
              Game1.player.maxHealth += 25;
              Game1.player.mailReceived.Add("qiCave");
            }
            ++this.CurrentCommand;
            break;
          case 3052429132:
            if (!(lower == "memento"))
              break;
            Object object1 = new Object(864, 1);
            object1.specialItem = true;
            Object object2 = object1;
            object2.questItem.Value = true;
            Game1.player.addItemByMenuIfNecessary((Item) object2);
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 3564172623:
            if (!(lower == "emilyclothes"))
              break;
            Clothing clothing = new Clothing(8);
            clothing.Dye(new Microsoft.Xna.Framework.Color(0, 143, 239), 1f);
            Game1.player.addItemsByMenuIfNecessary(new List<Item>()
            {
              (Item) new Boots(804),
              (Item) new StardewValley.Objects.Hat(41),
              (Item) new Clothing(1127),
              (Item) clothing
            });
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 3793392911:
            if (!(lower == "joja"))
              break;
            Game1.getSteamAchievement("Achievement_Joja");
            Game1.player.addItemByMenuIfNecessary((Item) new Object(Vector2.Zero, 117));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
          case 3820005428:
            if (!(lower == "sculpture"))
              break;
            Game1.player.addItemByMenuIfNecessary((Item) new Furniture(1306, Vector2.Zero));
            if (Game1.activeClickableMenu == null)
              ++this.CurrentCommand;
            ++this.CurrentCommand;
            break;
        }
      }
      else
        this.CurrentCommand += 2;
    }

    public virtual void command_attachCharacterToTempSprite(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      TemporaryAnimatedSprite temporaryAnimatedSprite = location.temporarySprites.Last<TemporaryAnimatedSprite>();
      if (temporaryAnimatedSprite == null)
        return;
      temporaryAnimatedSprite.attachedCharacter = (Character) this.getActorByName(split[1]);
    }

    public virtual void command_fork(GameLocation location, GameTime time, string[] split)
    {
      if (split.Length > 2)
      {
        int result;
        if (Game1.player.mailReceived.Contains(split[1]) || int.TryParse(split[1], out result) && Game1.player.dialogueQuestionsAnswered.Contains(result))
        {
          this.eventCommands = split.Length <= 3 ? Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + Game1.currentLocation.Name)[split[2]].Split('/') : Game1.content.LoadString(split[2]).Split('/');
          this.CurrentCommand = 0;
          this.forked = !this.forked;
        }
        else
          ++this.CurrentCommand;
      }
      else if (this.specialEventVariable1)
      {
        this.eventCommands = this.isFestival ? this.festivalData[split[1]].Split('/') : Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + Game1.currentLocation.Name)[split[1]].Split('/');
        this.CurrentCommand = 0;
        this.forked = !this.forked;
      }
      else
        ++this.CurrentCommand;
    }

    public virtual void command_switchEvent(GameLocation location, GameTime time, string[] split)
    {
      this.eventCommands = !this.isFestival ? Game1.content.Load<Dictionary<string, string>>("Data\\Events\\" + Game1.currentLocation.Name)[split[1]].Split('/') : this.festivalData[split[1]].Split('/');
      this.CurrentCommand = 0;
      this.eventSwitched = true;
    }

    public virtual void command_globalFade(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.globalFade)
        return;
      if (split.Length > 2)
      {
        Game1.globalFadeToBlack(fadeSpeed: (split.Length > 1 ? (float) Convert.ToDouble(split[1]) : 0.007f));
        ++this.CurrentCommand;
      }
      else
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.incrementCommandAfterFade), split.Length > 1 ? (float) Convert.ToDouble(split[1]) : 0.007f);
    }

    public virtual void command_globalFadeToClear(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (Game1.globalFade)
        return;
      if (split.Length > 2)
      {
        Game1.globalFadeToClear(fadeSpeed: (split.Length > 1 ? (float) Convert.ToDouble(split[1]) : 0.007f));
        ++this.CurrentCommand;
      }
      else
        Game1.globalFadeToClear(new Game1.afterFadeFunction(this.incrementCommandAfterFade), split.Length > 1 ? (float) Convert.ToDouble(split[1]) : 0.007f);
    }

    public virtual void command_cutscene(GameLocation location, GameTime time, string[] split)
    {
      if (this.currentCustomEventScript != null)
      {
        if (!this.currentCustomEventScript.update(time, this))
          return;
        this.currentCustomEventScript = (ICustomEventScript) null;
        ++this.CurrentCommand;
      }
      else
      {
        if (Game1.currentMinigame != null)
          return;
        switch (split[1])
        {
          case "AbigailGame":
            Game1.currentMinigame = (IMinigame) new AbigailGame(true);
            break;
          case "addSecretSantaItem":
            Game1.player.addItemByMenuIfNecessaryElseHoldUp(Utility.getGiftFromNPC(this.mySecretSanta));
            ++this.currentCommand;
            return;
          case "balloonChangeMap":
            this.eventPositionTileOffset = Vector2.Zero;
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1183, 84, 160), 10000f, 1, 99999, this.OffsetPosition(new Vector2(22f, 36f) * 64f + new Vector2(-23f, 0.0f) * 4f), false, false, 2E-05f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, -2f),
              yStopCoordinate = (int) this.OffsetPositionY(576f),
              reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.balloonInSky),
              attachedCharacter = (Character) this.farmer,
              id = 1f
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(84, 1205, 38, 26), 10000f, 1, 99999, this.OffsetPosition(new Vector2(22f, 36f) * 64f + new Vector2(0.0f, 134f) * 4f), false, false, 0.2625f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, -2f),
              id = 2f,
              attachedCharacter = (Character) this.getActorByName("Harvey")
            });
            ++this.CurrentCommand;
            break;
          case "balloonDepart":
            TemporaryAnimatedSprite temporarySpriteById1 = location.getTemporarySpriteByID(1);
            temporarySpriteById1.attachedCharacter = (Character) this.farmer;
            temporarySpriteById1.motion = new Vector2(0.0f, -2f);
            TemporaryAnimatedSprite temporarySpriteById2 = location.getTemporarySpriteByID(2);
            temporarySpriteById2.attachedCharacter = (Character) this.getActorByName("Harvey");
            temporarySpriteById2.motion = new Vector2(0.0f, -2f);
            location.getTemporarySpriteByID(3).scaleChange = -0.01f;
            ++this.CurrentCommand;
            return;
          case "bandFork":
            int answerChoice = 76;
            if (Game1.player.dialogueQuestionsAnswered.Contains(77))
              answerChoice = 77;
            else if (Game1.player.dialogueQuestionsAnswered.Contains(78))
              answerChoice = 78;
            else if (Game1.player.dialogueQuestionsAnswered.Contains(79))
              answerChoice = 79;
            this.answerDialogue("bandFork", answerChoice);
            ++this.CurrentCommand;
            return;
          case "boardGame":
            Game1.currentMinigame = (IMinigame) new FantasyBoardGame();
            ++this.CurrentCommand;
            break;
          case "clearTempSprites":
            location.temporarySprites.Clear();
            ++this.CurrentCommand;
            break;
          case "eggHuntWinner":
            this.eggHuntWinner();
            ++this.CurrentCommand;
            return;
          case "governorTaste":
            this.governorTaste();
            ++this.currentCommand;
            return;
          case "greenTea":
            this.currentCustomEventScript = (ICustomEventScript) new EventScript_GreenTea(new Vector2(-64000f, -64000f), this);
            break;
          case "haleyCows":
            Game1.currentMinigame = (IMinigame) new HaleyCowPictures();
            break;
          case "iceFishingWinner":
            this.iceFishingWinner();
            ++this.currentCommand;
            return;
          case "iceFishingWinnerMP":
            this.iceFishingWinnerMP();
            ++this.currentCommand;
            return;
          case "linusMoneyGone":
            foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
            {
              temporarySprite.alphaFade = 0.01f;
              temporarySprite.motion = new Vector2(0.0f, -1f);
            }
            ++this.CurrentCommand;
            return;
          case "marucomet":
            Game1.currentMinigame = (IMinigame) new MaruComet();
            break;
          case "plane":
            Game1.currentMinigame = (IMinigame) new PlaneFlyBy();
            break;
          case "robot":
            Game1.currentMinigame = (IMinigame) new RobotBlastoff();
            break;
        }
        Game1.globalFadeToClear(fadeSpeed: 0.01f);
      }
    }

    public virtual void command_grabObject(GameLocation location, GameTime time, string[] split)
    {
      this.farmer.grabObject(new Object(Vector2.Zero, Convert.ToInt32(split[1]), (string) null, false, true, false, false));
      this.showActiveObject = true;
      ++this.CurrentCommand;
    }

    public virtual void command_addTool(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Equals("Sword"))
      {
        if (!Game1.player.addItemToInventoryBool((Item) new Sword("Battered Sword", 67)))
        {
          Game1.player.addItemToInventoryBool((Item) new Sword("Battered Sword", 67));
          Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1209")));
        }
        else
        {
          for (int index = 0; index < Game1.player.Items.Count<Item>(); ++index)
          {
            if (Game1.player.Items[index] != null && Game1.player.Items[index] is Tool && Game1.player.Items[index].Name.Contains("Sword"))
            {
              Game1.player.CurrentToolIndex = index;
              Game1.switchToolAnimation();
              break;
            }
          }
        }
      }
      else if (split[1].Equals("Wand") && !Game1.player.addItemToInventoryBool((Item) new Wand()))
      {
        Game1.player.addItemToInventoryBool((Item) new Wand());
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1212")));
      }
      ++this.CurrentCommand;
    }

    public virtual void command_waitForTempSprite(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (Game1.currentLocation.getTemporarySpriteByID(int.Parse(split[1])) == null)
        return;
      ++this.CurrentCommand;
    }

    public virtual void command_waitForKey(GameLocation location, GameTime time, string[] split)
    {
      string str = split[1];
      KeyboardState keyboardState = Game1.GetKeyboardState();
      bool flag = false;
      if (!this.farmer.UsingTool && !Game1.pickingTool)
      {
        foreach (Keys pressedKey in keyboardState.GetPressedKeys())
        {
          if (Enum.GetName(pressedKey.GetType(), (object) pressedKey).Equals(str.ToUpper()))
          {
            flag = true;
            switch (pressedKey)
            {
              case Keys.C:
                Game1.pressUseToolButton();
                this.farmer.EndUsingTool();
                goto label_9;
              case Keys.S:
                Game1.pressAddItemToInventoryButton();
                this.showActiveObject = false;
                this.farmer.showNotCarrying();
                goto label_9;
              case Keys.Z:
                Game1.pressSwitchToolButton();
                goto label_9;
              default:
                goto label_9;
            }
          }
        }
      }
label_9:
      this.messageToScreen = this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf('"') + 1, this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf('"') + 1).IndexOf('"'));
      if (!flag)
        return;
      this.messageToScreen = (string) null;
      ++this.CurrentCommand;
    }

    public virtual void command_cave(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.activeClickableMenu != null)
        return;
      Response[] answerChoices = new Response[2]
      {
        new Response("Mushrooms", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1220")),
        new Response("Bats", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1222"))
      };
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1223"), answerChoices, "cave");
      Game1.dialogueTyping = false;
    }

    public virtual void command_updateMinigame(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (Game1.currentMinigame != null)
        Game1.currentMinigame.receiveEventPoke(Convert.ToInt32(split[1]));
      ++this.CurrentCommand;
    }

    public virtual void command_startJittering(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.farmer.jitterStrength = 1f;
      ++this.CurrentCommand;
    }

    public virtual void command_money(GameLocation location, GameTime time, string[] split)
    {
      this.farmer.Money += Convert.ToInt32(split[1]);
      if (this.farmer.Money < 0)
        this.farmer.Money = 0;
      ++this.CurrentCommand;
    }

    public virtual void command_stopJittering(GameLocation location, GameTime time, string[] split)
    {
      this.farmer.stopJittering();
      ++this.CurrentCommand;
    }

    public virtual void command_addLantern(GameLocation location, GameTime time, string[] split)
    {
      location.TemporarySprites.Add(new TemporaryAnimatedSprite(Convert.ToInt32(split[1]), 999999f, 1, 0, this.OffsetPosition(new Vector2((float) Convert.ToInt32(split[2]), (float) Convert.ToInt32(split[3])) * 64f), false, false)
      {
        light = true,
        lightRadius = (float) Convert.ToInt32(split[4])
      });
      ++this.CurrentCommand;
    }

    public virtual void command_rustyKey(GameLocation location, GameTime time, string[] split)
    {
      Game1.player.hasRustyKey = true;
      ++this.CurrentCommand;
    }

    public virtual void command_swimming(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Equals("farmer"))
      {
        this.farmer.bathingClothes.Value = true;
        this.farmer.swimming.Value = true;
      }
      else
        this.getActorByName(split[1]).swimming.Value = true;
      ++this.CurrentCommand;
    }

    public virtual void command_stopSwimming(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Equals("farmer"))
      {
        this.farmer.bathingClothes.Value = location is BathHousePool;
        this.farmer.swimming.Value = false;
      }
      else
        this.getActorByName(split[1]).swimming.Value = false;
      ++this.CurrentCommand;
    }

    public virtual void command_tutorialMenu(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.activeClickableMenu != null)
        return;
      Game1.activeClickableMenu = (IClickableMenu) new TutorialMenu();
    }

    public virtual void command_animalNaming(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.activeClickableMenu != null)
        return;
      Game1.activeClickableMenu = (IClickableMenu) new NamingMenu((NamingMenu.doneNamingBehavior) (animal_name =>
      {
        (Game1.currentLocation as AnimalHouse).addNewHatchedAnimal(animal_name);
        ++this.CurrentCommand;
      }), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"));
    }

    public virtual void command_splitSpeak(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.dialogueUp)
        return;
      this.timeAccumulator += (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.timeAccumulator < 500.0)
        return;
      this.timeAccumulator = 0.0f;
      string[] strArray = this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf('"') + 1, this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf('"') + 1).IndexOf('"')).Split('~');
      NPC speaker = this.getActorByName(split[1]) ?? Game1.getCharacterFromName(split[1].Equals("rival") ? Utility.getOtherFarmerNames()[0] : split[1]);
      if (speaker == null || this.previousAnswerChoice < 0 || this.previousAnswerChoice >= strArray.Length)
      {
        ++this.CurrentCommand;
      }
      else
      {
        speaker.CurrentDialogue.Push(new Dialogue(strArray[this.previousAnswerChoice], speaker));
        Game1.drawDialogue(speaker);
      }
    }

    public virtual void command_catQuestion(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.isQuestion || Game1.activeClickableMenu != null)
        return;
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1241") + (Game1.player.catPerson ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1242") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1243")) + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1244"), Game1.currentLocation.createYesNoResponses(), "pet");
    }

    public virtual void command_ambientLight(GameLocation location, GameTime time, string[] split)
    {
      if (((IEnumerable<string>) split).Count<string>() > 4)
      {
        int r = (int) Game1.ambientLight.R;
        int g = (int) Game1.ambientLight.G;
        int b = (int) Game1.ambientLight.B;
        this.float_useMeForAnything += (float) time.ElapsedGameTime.Milliseconds;
        if ((double) this.float_useMeForAnything <= 10.0)
          return;
        bool flag = true;
        if (r != Convert.ToInt32(split[1]))
        {
          r += Math.Sign(Convert.ToInt32(split[1]) - r);
          flag = false;
        }
        if (g != Convert.ToInt32(split[2]))
        {
          g += Math.Sign(Convert.ToInt32(split[2]) - g);
          flag = false;
        }
        if (b != Convert.ToInt32(split[3]))
        {
          b += Math.Sign(Convert.ToInt32(split[3]) - b);
          flag = false;
        }
        this.float_useMeForAnything = 0.0f;
        Game1.ambientLight = new Microsoft.Xna.Framework.Color(r, g, b);
        if (!flag)
          return;
        ++this.CurrentCommand;
      }
      else
      {
        Game1.ambientLight = new Microsoft.Xna.Framework.Color(Convert.ToInt32(split[1]), Convert.ToInt32(split[2]), Convert.ToInt32(split[3]));
        ++this.CurrentCommand;
      }
    }

    public virtual void command_bgColor(GameLocation location, GameTime time, string[] split)
    {
      Game1.setBGColor(Convert.ToByte(split[1]), Convert.ToByte(split[2]), Convert.ToByte(split[3]));
      ++this.CurrentCommand;
    }

    public virtual void command_elliottbooktalk(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (Game1.dialogueUp)
        return;
      string masterDialogue = !Game1.player.dialogueQuestionsAnswered.Contains(958699) ? (!Game1.player.dialogueQuestionsAnswered.Contains(958700) ? (!Game1.player.dialogueQuestionsAnswered.Contains(9586701) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1260") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1259")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1258")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1257");
      NPC speaker = this.getActorByName("Elliott") ?? Game1.getCharacterFromName("Elliott");
      speaker.CurrentDialogue.Push(new Dialogue(masterDialogue, speaker));
      Game1.drawDialogue(speaker);
    }

    public virtual void command_removeItem(GameLocation location, GameTime time, string[] split)
    {
      Game1.player.removeFirstOfThisItemFromInventory(Convert.ToInt32(split[1]));
      ++this.CurrentCommand;
    }

    public virtual void command_friendship(GameLocation location, GameTime time, string[] split)
    {
      NPC characterFromName = Game1.getCharacterFromName(split[1]);
      if (characterFromName != null)
        Game1.player.changeFriendship(Convert.ToInt32(split[2]), characterFromName);
      ++this.CurrentCommand;
    }

    public virtual void command_setRunning(GameLocation location, GameTime time, string[] split)
    {
      this.farmer.setRunning(true);
      ++this.CurrentCommand;
    }

    public virtual void command_extendSourceRect(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (split[2].Equals("reset"))
      {
        this.getActorByName(split[1]).reloadSprite();
        this.getActorByName(split[1]).Sprite.SpriteWidth = 16;
        this.getActorByName(split[1]).Sprite.SpriteHeight = 32;
        this.getActorByName(split[1]).HideShadow = false;
      }
      else
        this.getActorByName(split[1]).extendSourceRect(Convert.ToInt32(split[2]), Convert.ToInt32(split[3]), split.Length <= 4);
      ++this.CurrentCommand;
    }

    public virtual void command_waitForOtherPlayers(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (Game1.IsMultiplayer)
      {
        Game1.player.team.SetLocalReady(split[1], true);
        if (Game1.player.team.IsReady(split[1]))
        {
          if (Game1.activeClickableMenu is ReadyCheckDialog)
            Game1.exitActiveMenu();
          ++this.CurrentCommand;
        }
        else
        {
          if (Game1.activeClickableMenu != null)
            return;
          Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog(split[1], false);
        }
      }
      else
        ++this.CurrentCommand;
    }

    public virtual void command_requestMovieEnd(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      Game1.player.team.requestMovieEndEvent.Fire(Game1.player.UniqueMultiplayerID);
    }

    public virtual void command_restoreStashedItem(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      Game1.player.TemporaryItem = (Item) null;
      ++this.CurrentCommand;
    }

    public virtual void command_advancedMove(GameLocation location, GameTime time, string[] split)
    {
      this.setUpAdvancedMove(split);
      ++this.CurrentCommand;
    }

    public virtual void command_stopRunning(GameLocation location, GameTime time, string[] split)
    {
      this.farmer.setRunning(false);
      ++this.CurrentCommand;
    }

    public virtual void command_eyes(GameLocation location, GameTime time, string[] split)
    {
      this.farmer.currentEyes = Convert.ToInt32(split[1]);
      this.farmer.blinkTimer = Convert.ToInt32(split[2]);
      ++this.CurrentCommand;
    }

    public virtual void command_addMailReceived(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      Game1.player.mailReceived.Add(split[1]);
      ++this.CurrentCommand;
    }

    public virtual void command_addWorldState(GameLocation location, GameTime time, string[] split)
    {
      Game1.worldStateIDs.Add(split[1]);
      Game1.netWorldState.Value.addWorldStateID(split[1]);
      ++this.CurrentCommand;
    }

    public virtual void command_fade(GameLocation location, GameTime time, string[] split)
    {
      if (((IEnumerable<string>) split).Count<string>() > 1 && split[1].Equals("unfade"))
      {
        Game1.fadeIn = false;
        Game1.fadeToBlack = false;
        ++this.CurrentCommand;
      }
      else
      {
        Game1.fadeToBlack = true;
        Game1.fadeIn = true;
        if ((double) Game1.fadeToBlackAlpha < 0.970000028610229)
          return;
        if (split.Length == 1)
          Game1.fadeIn = false;
        ++this.CurrentCommand;
      }
    }

    public virtual void command_changeMapTile(GameLocation location, GameTime time, string[] split)
    {
      string layerId = split[1];
      int x = this.OffsetTileX(Convert.ToInt32(split[2]));
      int y = this.OffsetTileY(Convert.ToInt32(split[3]));
      int int32 = Convert.ToInt32(split[4]);
      location.map.GetLayer(layerId).Tiles[x, y].TileIndex = int32;
      ++this.CurrentCommand;
    }

    public virtual void command_changeSprite(GameLocation location, GameTime time, string[] split)
    {
      this.getActorByName(split[1]).Sprite.LoadTexture("Characters\\" + NPC.getTextureNameForCharacter(split[1]) + "_" + split[2]);
      ++this.CurrentCommand;
    }

    public virtual void command_waitForAllStationary(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      bool flag = false;
      if (this.npcControllers != null && this.npcControllers.Count > 0)
        flag = true;
      if (!flag)
      {
        foreach (Character actor in this.actors)
        {
          if (actor.isMoving())
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
      {
        foreach (Character farmerActor in this.farmerActors)
        {
          if (farmerActor.isMoving())
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
        return;
      ++this.CurrentCommand;
    }

    public virtual void command_proceedPosition(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.continueAfterMove = true;
      try
      {
        Character characterByName = this.getCharacterByName(split[1]);
        if (characterByName.isMoving() && (this.npcControllers == null || this.npcControllers.Count != 0))
          return;
        characterByName.Halt();
        ++this.CurrentCommand;
      }
      catch (Exception ex)
      {
        ++this.CurrentCommand;
      }
    }

    public virtual void command_changePortrait(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      NPC npc = this.getActorByName(split[1]) ?? Game1.getCharacterFromName(split[1]);
      npc.Portrait = Game1.content.Load<Texture2D>("Portraits\\" + split[1] + "_" + split[2]);
      npc.uniquePortraitActive = true;
      this.npcsWithUniquePortraits.Add(npc);
      ++this.CurrentCommand;
    }

    public virtual void command_changeYSourceRectOffset(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      NPC actorByName = this.getActorByName(split[1]);
      if (actorByName != null)
        actorByName.ySourceRectOffset = Convert.ToInt32(split[2]);
      ++this.CurrentCommand;
    }

    public virtual void command_changeName(GameLocation location, GameTime time, string[] split)
    {
      NPC actorByName = this.getActorByName(split[1]);
      if (actorByName != null)
        actorByName.displayName = split[2].Replace('_', ' ');
      ++this.CurrentCommand;
    }

    public virtual void command_playFramesAhead(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      int int32 = Convert.ToInt32(split[1]);
      ++this.CurrentCommand;
      for (int index = 0; index < int32; ++index)
        this.checkForNextCommand(location, time);
    }

    public virtual void command_showKissFrame(GameLocation location, GameTime time, string[] split)
    {
      bool flip = true;
      NPC actorByName = this.getActorByName(split[1]);
      bool flag = ((IEnumerable<string>) split).Count<string>() > 2 && Convert.ToBoolean(split[2]);
      int frame = 28;
      switch (actorByName.Name)
      {
        case "Abigail":
          frame = 33;
          flip = false;
          break;
        case "Alex":
          frame = 42;
          flip = true;
          break;
        case "Elliott":
          frame = 35;
          flip = false;
          break;
        case "Emily":
          frame = 33;
          flip = false;
          break;
        case "Harvey":
          frame = 31;
          flip = false;
          break;
        case "Krobus":
          frame = 16;
          flip = true;
          break;
        case "Leah":
          frame = 25;
          flip = true;
          break;
        case "Maru":
          frame = 28;
          flip = false;
          break;
        case "Penny":
          frame = 35;
          flip = true;
          break;
        case "Sam":
          frame = 36;
          flip = true;
          break;
        case "Sebastian":
          frame = 40;
          flip = false;
          break;
        case "Shane":
          frame = 34;
          flip = false;
          break;
      }
      if (flag)
        flip = !flip;
      actorByName.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(frame, 1000, false, flip)
      });
      ++this.CurrentCommand;
    }

    public virtual void command_addTemporaryActor(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      string str = "Characters\\";
      bool flag = true;
      if (split.Length > 8 && split[8].ToLower().Equals("animal"))
        str = "Animals\\";
      else if (split.Length > 8 && split[8].ToLower().Equals("monster"))
        str = "Characters\\Monsters\\";
      else if (split.Length <= 8 || !split[8].ToLower().Equals("character"))
        flag = false;
      NPC speaker = new NPC(new AnimatedSprite((ContentManager) this.festivalContent, str + split[1].Replace('_', ' '), 0, Convert.ToInt32(split[2]), Convert.ToInt32(split[3])), this.OffsetPosition(new Vector2((float) Convert.ToInt32(split[4]), (float) Convert.ToInt32(split[5])) * 64f), Convert.ToInt32(split[6]), split[1].Replace('_', ' '), this.festivalContent);
      if (split.Length > 7)
        speaker.Breather = Convert.ToBoolean(split[7]);
      if (!flag && split.Length > 8)
        speaker.displayName = split[8].Replace('_', ' ');
      if (this.isFestival)
      {
        try
        {
          speaker.CurrentDialogue.Push(new Dialogue(this.festivalData[speaker.Name], speaker));
        }
        catch (Exception ex)
        {
        }
      }
      if (str.Contains("Animals") && split.Length > 9)
        speaker.Name = split[9];
      if (speaker.Sprite.SpriteWidth >= 32)
        speaker.HideShadow = true;
      speaker.eventActor = true;
      this.actors.Add(speaker);
      ++this.CurrentCommand;
    }

    public virtual void command_changeToTemporaryMap(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      this.temporaryLocation = !split[1].Contains("Town") ? new GameLocation("Maps\\" + split[1], "Temp") : (GameLocation) new Town("Maps\\" + split[1], "Temp");
      this.temporaryLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
      Event currentEvent = Game1.currentLocation.currentEvent;
      Game1.currentLocation.cleanupBeforePlayerExit();
      Game1.currentLocation.currentEvent = (Event) null;
      Game1.currentLightSources.Clear();
      Game1.currentLocation = this.temporaryLocation;
      Game1.currentLocation.resetForPlayerEntry();
      Game1.currentLocation.currentEvent = currentEvent;
      ++this.CurrentCommand;
      Game1.player.currentLocation = Game1.currentLocation;
      this.farmer.currentLocation = Game1.currentLocation;
      if (split.Length >= 3)
        return;
      Game1.panScreen(0, 0);
    }

    public virtual void command_positionOffset(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (split[1].Contains("farmer"))
      {
        Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(split[1], this.farmer);
        if (farmerNumberString != null)
        {
          farmerNumberString.position.X += (float) Convert.ToInt32(split[2]);
          farmerNumberString.position.Y += (float) Convert.ToInt32(split[3]);
        }
      }
      else
      {
        NPC actorByName = this.getActorByName(split[1]);
        if (actorByName != null)
        {
          actorByName.position.X += (float) Convert.ToInt32(split[2]);
          actorByName.position.Y += (float) Convert.ToInt32(split[3]);
        }
      }
      ++this.CurrentCommand;
      if (split.Length <= 4)
        return;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_question(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.isQuestion || Game1.activeClickableMenu != null)
        return;
      string[] strArray = this.eventCommands[Math.Min(this.eventCommands.Length - 1, this.CurrentCommand)].Split('"')[1].Split('#');
      string question = strArray[0];
      Response[] answerChoices = new Response[strArray.Length - 1];
      for (int index = 1; index < strArray.Length; ++index)
        answerChoices[index - 1] = new Response((index - 1).ToString(), strArray[index]);
      Game1.currentLocation.createQuestionDialogue(question, answerChoices, split[1]);
    }

    public virtual void command_quickQuestion(GameLocation location, GameTime time, string[] split)
    {
      if (Game1.isQuestion || Game1.activeClickableMenu != null)
        return;
      string eventCommand = this.eventCommands[Math.Min(this.eventCommands.Length - 1, this.CurrentCommand)];
      string[] strArray = eventCommand.Substring(eventCommand.IndexOf(' ') + 1).Split(new string[1]
      {
        "(break)"
      }, StringSplitOptions.None)[0].Split('#');
      string question = strArray[0];
      Response[] answerChoices = new Response[strArray.Length - 1];
      for (int index = 1; index < strArray.Length; ++index)
        answerChoices[index - 1] = new Response((index - 1).ToString(), strArray[index]);
      Game1.currentLocation.createQuestionDialogue(question, answerChoices, "quickQuestion");
    }

    public virtual void command_drawOffset(GameLocation location, GameTime time, string[] split)
    {
      int int32_1 = Convert.ToInt32(split[2]);
      float int32_2 = (float) Convert.ToInt32(split[3]);
      (!split[1].Equals("farmer") ? (Character) this.getActorByName(split[1]) : (Character) this.farmer).drawOffset.Value = new Vector2((float) int32_1, int32_2) * 4f;
      ++this.CurrentCommand;
    }

    public virtual void command_hideShadow(GameLocation location, GameTime time, string[] split)
    {
      bool flag = split[2].Equals("true");
      this.getActorByName(split[1]).HideShadow = flag;
      ++this.CurrentCommand;
    }

    public virtual void command_animateHeight(GameLocation location, GameTime time, string[] split)
    {
      int? nullable1 = new int?();
      float? nullable2 = new float?();
      float? nullable3 = new float?();
      if (split[2] != "keep")
        nullable1 = new int?(Convert.ToInt32(split[2]));
      if (split[3] != "keep")
        nullable2 = new float?((float) Convert.ToDouble(split[3]));
      if (split[4] != "keep")
        nullable3 = new float?((float) Convert.ToInt32(split[4]));
      Character character = !split[1].Equals("farmer") ? (Character) this.getActorByName(split[1]) : (Character) this.farmer;
      if (nullable1.HasValue)
        character.yJumpOffset = -nullable1.Value;
      if (nullable2.HasValue)
        character.yJumpGravity = nullable2.Value;
      if (nullable3.HasValue)
        character.yJumpVelocity = nullable3.Value;
      ++this.CurrentCommand;
    }

    public virtual void command_jump(GameLocation location, GameTime time, string[] split)
    {
      float jumpVelocity = split.Length > 2 ? (float) Convert.ToDouble(split[2]) : 8f;
      if (split[1].Equals("farmer"))
        this.farmer.jump(jumpVelocity);
      else
        this.getActorByName(split[1]).jump(jumpVelocity);
      ++this.CurrentCommand;
      this.checkForNextCommand(location, time);
    }

    public virtual void command_farmerEat(GameLocation location, GameTime time, string[] split)
    {
      this.farmer.eatObject(new Object(Convert.ToInt32(split[1]), 1), true);
      ++this.CurrentCommand;
    }

    public virtual void command_spriteText(GameLocation location, GameTime time, string[] split)
    {
      int startIndex = this.eventCommands[this.CurrentCommand].IndexOf('"') + 1;
      int num = this.eventCommands[this.CurrentCommand].LastIndexOf('"');
      this.int_useMeForAnything2 = Convert.ToInt32(split[1]);
      if (num <= 0 || num <= startIndex)
        return;
      string str = this.eventCommands[this.CurrentCommand].Substring(startIndex, num - startIndex);
      this.float_useMeForAnything += (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.float_useMeForAnything > 80.0)
      {
        if (this.int_useMeForAnything >= str.Length - 1)
        {
          if ((double) this.float_useMeForAnything >= 2500.0)
          {
            this.int_useMeForAnything = 0;
            this.float_useMeForAnything = 0.0f;
            this.spriteTextToDraw = "";
            ++this.CurrentCommand;
          }
        }
        else
        {
          ++this.int_useMeForAnything;
          this.float_useMeForAnything = 0.0f;
          Game1.playSound("dialogueCharacter");
        }
      }
      this.spriteTextToDraw = str;
    }

    public virtual void command_ignoreCollisions(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (split[1].Contains("farmer"))
      {
        Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(split[1], this.farmer);
        if (farmerNumberString != null)
          farmerNumberString.ignoreCollisions = true;
      }
      else
      {
        NPC actorByName = this.getActorByName(split[1]);
        if (actorByName != null)
          actorByName.isCharging = true;
      }
      ++this.CurrentCommand;
    }

    public virtual void command_screenFlash(GameLocation location, GameTime time, string[] split)
    {
      Game1.flashAlpha = (float) Convert.ToDouble(split[1]);
      ++this.CurrentCommand;
    }

    public virtual void command_grandpaCandles(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      int candlesFromScore = Utility.getGrandpaCandlesFromScore(Utility.getGrandpaScore());
      Game1.getFarm().grandpaScore.Value = candlesFromScore;
      for (int index = 0; index < candlesFromScore; ++index)
        DelayedAction.playSoundAfterDelay("fireball", 100 * index);
      Game1.getFarm().addGrandpaCandles();
      ++this.CurrentCommand;
    }

    public virtual void command_grandpaEvaluation2(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      switch (Utility.getGrandpaCandlesFromScore(Utility.getGrandpaScore()))
      {
        case 1:
          this.eventCommands[this.currentCommand] = "speak Grandpa \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1306") + "\"";
          break;
        case 2:
          this.eventCommands[this.currentCommand] = "speak Grandpa \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1307") + "\"";
          break;
        case 3:
          this.eventCommands[this.currentCommand] = "speak Grandpa \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1308") + "\"";
          break;
        case 4:
          this.eventCommands[this.currentCommand] = "speak Grandpa \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1309") + "\"";
          break;
      }
      Game1.player.eventsSeen.Remove(2146991);
    }

    public virtual void command_grandpaEvaluation(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      switch (Utility.getGrandpaCandlesFromScore(Utility.getGrandpaScore()))
      {
        case 1:
          this.eventCommands[this.currentCommand] = "speak Grandpa \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1315") + "\"";
          break;
        case 2:
          this.eventCommands[this.currentCommand] = "speak Grandpa \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1316") + "\"";
          break;
        case 3:
          this.eventCommands[this.currentCommand] = "speak Grandpa \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1317") + "\"";
          break;
        case 4:
          this.eventCommands[this.currentCommand] = "speak Grandpa \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1318") + "\"";
          break;
      }
    }

    public virtual void command_loadActors(GameLocation location, GameTime time, string[] split)
    {
      if (this.temporaryLocation != null && this.temporaryLocation.map.GetLayer(split[1]) != null)
      {
        this.actors.Clear();
        if (this.npcControllers != null)
          this.npcControllers.Clear();
        Dictionary<string, string> source = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
        List<string> stringList = new List<string>();
        for (int x = 0; x < this.temporaryLocation.map.GetLayer(split[1]).LayerWidth; ++x)
        {
          for (int y = 0; y < this.temporaryLocation.map.GetLayer(split[1]).LayerHeight; ++y)
          {
            if (this.temporaryLocation.map.GetLayer(split[1]).Tiles[x, y] != null)
            {
              int index = this.temporaryLocation.map.GetLayer(split[1]).Tiles[x, y].TileIndex / 4;
              int facingDirection = this.temporaryLocation.map.GetLayer(split[1]).Tiles[x, y].TileIndex % 4;
              string key = source.ElementAt<KeyValuePair<string, string>>(index).Key;
              if (key != null && Game1.getCharacterFromName(key) != null && (!(key == "Leo") || Game1.MasterPlayer.mailReceived.Contains("leoMoved")))
              {
                this.addActor(key, x, y, facingDirection, this.temporaryLocation);
                stringList.Add(key);
              }
            }
          }
        }
        if (this.festivalData != null)
        {
          string key = split[1] + "_additionalCharacters";
          if (this.festivalData.ContainsKey(key))
          {
            foreach (string str in this.festivalData[key].Split('/'))
            {
              if (!string.IsNullOrEmpty(str))
              {
                string[] strArray = str.Split(' ');
                if (strArray.Length >= 4)
                {
                  bool flag = false;
                  int result1 = 0;
                  int result2 = 0;
                  int result3 = 2;
                  if (!flag && !int.TryParse(strArray[1], out result1))
                    flag = true;
                  if (!flag && !int.TryParse(strArray[2], out result2))
                    flag = true;
                  if (!flag)
                  {
                    string lowerInvariant = strArray[3].ToLowerInvariant();
                    if (lowerInvariant == "up")
                      result3 = 0;
                    else if (lowerInvariant == "down")
                      result3 = 2;
                    else if (lowerInvariant == "left")
                      result3 = 3;
                    else if (lowerInvariant == "right")
                      result3 = 1;
                    else if (!int.TryParse(lowerInvariant, out result3))
                      flag = true;
                  }
                  if (flag)
                  {
                    Console.WriteLine("Warning: Failed to load additional festival character: " + str);
                  }
                  else
                  {
                    string name = strArray[0];
                    if (name != null && Game1.getCharacterFromName(name) != null)
                    {
                      if (!(name == "Leo") || Game1.MasterPlayer.mailReceived.Contains("leoMoved"))
                      {
                        this.addActor(name, result1, result2, result3, this.temporaryLocation);
                        stringList.Add(name);
                      }
                    }
                    else
                      Console.WriteLine("Warning: Invalid additional festival character name: " + name);
                  }
                }
              }
            }
          }
        }
        if (split[1] == "Set-Up")
        {
          foreach (string name in stringList)
          {
            NPC characterFromName = Game1.getCharacterFromName(name);
            if (characterFromName.isMarried() && characterFromName.getSpouse() != null && characterFromName.getSpouse().getChildren().Count > 0)
            {
              Farmer parent = Game1.player;
              if (characterFromName.getSpouse() != null)
                parent = characterFromName.getSpouse();
              List<Child> children = parent.getChildren();
              NPC characterByName = this.getCharacterByName(name) as NPC;
              for (int index1 = 0; index1 < children.Count; ++index1)
              {
                Child child1 = children[index1];
                if (child1.Age >= 3)
                {
                  Child child2 = new Child(child1.Name, child1.Gender == 0, (bool) (NetFieldBase<bool, NetBool>) child1.darkSkinned, parent);
                  child2.NetFields.CopyFrom(child1.NetFields);
                  child2.Halt();
                  Point point1 = new Point((int) characterByName.Position.X / 64, (int) characterByName.Position.Y / 64);
                  Point[] pointArray;
                  switch (characterByName.FacingDirection)
                  {
                    case 0:
                      pointArray = new Point[4]
                      {
                        new Point(0, 1),
                        new Point(-1, 0),
                        new Point(1, 0),
                        new Point(0, -1)
                      };
                      break;
                    case 1:
                      pointArray = new Point[4]
                      {
                        new Point(-1, 0),
                        new Point(0, 1),
                        new Point(0, -1),
                        new Point(1, 0)
                      };
                      break;
                    case 2:
                      pointArray = new Point[4]
                      {
                        new Point(0, -1),
                        new Point(1, 0),
                        new Point(-1, 0),
                        new Point(0, 1)
                      };
                      break;
                    case 3:
                      pointArray = new Point[4]
                      {
                        new Point(1, 0),
                        new Point(0, -1),
                        new Point(0, 1),
                        new Point(-1, 0)
                      };
                      break;
                    default:
                      pointArray = new Point[4]
                      {
                        new Point(-1, 0),
                        new Point(1, 0),
                        new Point(0, -1),
                        new Point(0, 1)
                      };
                      break;
                  }
                  Point point2 = new Point(characterByName.getTileX(), characterByName.getTileY());
                  List<Point> pointList1 = new List<Point>();
                  List<Point> pointList2 = new List<Point>();
                  foreach (Point point3 in pointArray)
                    pointList1.Add(new Point(point2.X + point3.X, point2.Y + point3.Y));
                  Func<Point, bool> func1 = (Func<Point, bool>) (point => this.temporaryLocation.isTilePassable(new Location(point.X, point.Y), Game1.viewport));
                  Func<Point, bool> func2 = (Func<Point, bool>) (point =>
                  {
                    int num = 1;
                    for (int x = point.X - num; x <= point.X + num; ++x)
                    {
                      for (int y = point.Y - num; y <= point.Y + num; ++y)
                      {
                        if (this.temporaryLocation.isTileOccupiedForPlacement(new Vector2((float) x, (float) y)))
                          return false;
                        foreach (Character actor in this.actors)
                        {
                          if (!(actor is Child) && actor.getTileX() == x && actor.getTileY() == y)
                            return false;
                        }
                      }
                    }
                    return true;
                  });
                  bool flag = false;
                  for (int index2 = 0; index2 < 5 && !flag; ++index2)
                  {
                    int count = pointList1.Count;
                    for (int index3 = 0; index3 < count; ++index3)
                    {
                      Point point4 = pointList1[0];
                      pointList1.RemoveAt(0);
                      if (func1(point4))
                      {
                        if (func2(point4))
                        {
                          flag = true;
                          point2 = point4;
                          break;
                        }
                        foreach (Point point5 in pointArray)
                          pointList1.Add(new Point(point4.X + point5.X, point4.Y + point5.Y));
                      }
                    }
                  }
                  if (flag)
                  {
                    child2.setTilePosition(point2.X, point2.Y);
                    child2.DefaultPosition = characterByName.DefaultPosition;
                    child2.faceDirection(characterByName.FacingDirection);
                    child2.eventActor = true;
                    child2.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(point2.X * 64, point2.Y * 64, 64, 64);
                    child2.squareMovementFacingPreference = -1;
                    child2.walkInSquare(3, 3, 2000);
                    child2.controller = (PathFindController) null;
                    child2.temporaryController = (PathFindController) null;
                    this.actors.Add((NPC) child2);
                  }
                }
              }
            }
          }
        }
      }
      ++this.CurrentCommand;
    }

    public virtual void command_playerControl(GameLocation location, GameTime time, string[] split)
    {
      if (this.playerControlSequence)
        return;
      this.setUpPlayerControlSequence(split[1]);
    }

    public virtual void command_removeSprite(GameLocation location, GameTime time, string[] split)
    {
      Vector2 other = this.OffsetPosition(new Vector2((float) Convert.ToInt32(split[1]), (float) Convert.ToInt32(split[2])) * 64f);
      for (int index = Game1.currentLocation.temporarySprites.Count - 1; index >= 0; --index)
      {
        if (Game1.currentLocation.temporarySprites[index].position.Equals(other))
          Game1.currentLocation.temporarySprites.RemoveAt(index);
      }
      ++this.CurrentCommand;
    }

    public virtual void command_viewport(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Equals("move"))
      {
        this.viewportTarget = new Vector3((float) Convert.ToInt32(split[2]), (float) Convert.ToInt32(split[3]), (float) Convert.ToInt32(split[4]));
      }
      else
      {
        if (this.aboveMapSprites != null && Convert.ToInt32(split[1]) < 0)
        {
          this.aboveMapSprites.Clear();
          this.aboveMapSprites = (List<TemporaryAnimatedSprite>) null;
        }
        Game1.viewportFreeze = true;
        int num1 = this.OffsetTileX(Convert.ToInt32(split[1]));
        int num2 = this.OffsetTileY(Convert.ToInt32(split[2]));
        if (this.id == 2146991)
        {
          Point grandpaShrinePosition = Game1.getFarm().GetGrandpaShrinePosition();
          num1 = grandpaShrinePosition.X;
          num2 = grandpaShrinePosition.Y;
        }
        Game1.viewport.X = num1 * 64 + 32 - Game1.viewport.Width / 2;
        Game1.viewport.Y = num2 * 64 + 32 - Game1.viewport.Height / 2;
        if (Game1.viewport.X > 0 && Game1.viewport.Width > Game1.currentLocation.Map.DisplayWidth)
          Game1.viewport.X = (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2;
        if (Game1.viewport.Y > 0 && Game1.viewport.Height > Game1.currentLocation.Map.DisplayHeight)
          Game1.viewport.Y = (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2;
        if (split.Length > 3 && split[3].Equals("true"))
        {
          Game1.fadeScreenToBlack();
          Game1.fadeToBlackAlpha = 1f;
          Game1.nonWarpFade = true;
        }
        else if (split.Length > 3 && split[3].Equals("clamp"))
        {
          if (Game1.currentLocation.map.DisplayWidth >= Game1.viewport.Width)
          {
            if (Game1.viewport.X + Game1.viewport.Width > Game1.currentLocation.Map.DisplayWidth)
              Game1.viewport.X = Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width;
            if (Game1.viewport.X < 0)
              Game1.viewport.X = 0;
          }
          else
            Game1.viewport.X = Game1.currentLocation.Map.DisplayWidth / 2 - Game1.viewport.Width / 2;
          if (Game1.currentLocation.map.DisplayHeight >= Game1.viewport.Height)
          {
            if (Game1.viewport.Y + Game1.viewport.Height > Game1.currentLocation.Map.DisplayHeight)
              Game1.viewport.Y = Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height;
          }
          else
            Game1.viewport.Y = Game1.currentLocation.Map.DisplayHeight / 2 - Game1.viewport.Height / 2;
          if (Game1.viewport.Y < 0)
            Game1.viewport.Y = 0;
          if (split.Length > 4 && split[4].Equals("true"))
          {
            Game1.fadeScreenToBlack();
            Game1.fadeToBlackAlpha = 1f;
            Game1.nonWarpFade = true;
          }
        }
        if (split.Length > 4 && split[4].Equals("unfreeze"))
          Game1.viewportFreeze = false;
        if (Game1.gameMode == (byte) 2)
          Game1.viewport.X = Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width;
      }
      ++this.CurrentCommand;
    }

    public virtual void command_broadcastEvent(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (this.farmer == Game1.player)
      {
        bool use_local_farmer = false;
        if (split.Length > 1 && split[1] == "local")
          use_local_farmer = true;
        if (this.id == 558291 || this.id == 558292)
          use_local_farmer = true;
        Game1.multiplayer.broadcastEvent(this, Game1.currentLocation, Game1.player.positionBeforeEvent, use_local_farmer);
      }
      ++this.CurrentCommand;
    }

    public virtual void command_addConversationTopic(
      GameLocation location,
      GameTime time,
      string[] split)
    {
      if (this.isMemory)
      {
        ++this.CurrentCommand;
      }
      else
      {
        if (!Game1.player.activeDialogueEvents.ContainsKey(split[1]))
          Game1.player.activeDialogueEvents.Add(split[1], ((IEnumerable<string>) split).Count<string>() > 2 ? Convert.ToInt32(split[2]) : 4);
        ++this.CurrentCommand;
      }
    }

    public virtual void command_dump(GameLocation location, GameTime time, string[] split)
    {
      if (split[1].Equals("girls"))
      {
        Game1.player.activeDialogueEvents.Add("dumped_Girls", 7);
        Game1.player.activeDialogueEvents.Add("secondChance_Girls", 14);
      }
      else
      {
        Game1.player.activeDialogueEvents.Add("dumped_Guys", 7);
        Game1.player.activeDialogueEvents.Add("secondChance_Guys", 14);
      }
      ++this.CurrentCommand;
    }

    public bool playerControlSequence
    {
      get => this._playerControlSequence;
      set
      {
        if (this._playerControlSequence == value)
          return;
        this._playerControlSequence = value;
        if (this._playerControlSequence)
          return;
        this.OnPlayerControlSequenceEnd(this.playerControlSequenceID);
      }
    }

    public Farmer farmer => this.farmerActors.Count <= 0 ? Game1.player : this.farmerActors[0];

    public Texture2D festivalTexture
    {
      get
      {
        if (this._festivalTexture == null)
          this._festivalTexture = this.festivalContent.Load<Texture2D>("Maps\\Festivals");
        return this._festivalTexture;
      }
    }

    public int CurrentCommand
    {
      get => this.currentCommand;
      set => this.currentCommand = value;
    }

    public Event(string eventString, int eventID = -1, Farmer farmerActor = null)
      : this()
    {
      this.id = eventID;
      this.eventCommands = eventString.Split('/');
      this.actorPositionsAfterMove = new Dictionary<string, Vector3>();
      this.previousAmbientLight = Game1.ambientLight;
      if (farmerActor != null)
        this.farmerActors.Add(farmerActor);
      this.farmer.canOnlyWalk = true;
      this.farmer.showNotCarrying();
      this.drawTool = false;
      if (eventID != -2)
        return;
      this.isWedding = true;
    }

    public Event() => this.setupEventCommands();

    /// <summary>
    /// returns false if it's not the right place/time for a festival, or not all players are present
    /// </summary>
    /// <returns></returns>
    public bool tryToLoadFestival(string festival)
    {
      if (Event.invalidFestivals.Contains(festival))
        return false;
      Game1.player.festivalScore = 0;
      try
      {
        this.festivalData = this.festivalContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + festival);
        this.festivalData["file"] = festival;
      }
      catch (Exception ex)
      {
        Event.invalidFestivals.Add(festival);
        return false;
      }
      string str = this.festivalData["conditions"].Split('/')[0];
      int int32_1 = Convert.ToInt32(this.festivalData["conditions"].Split('/')[1].Split(' ')[0]);
      int int32_2 = Convert.ToInt32(this.festivalData["conditions"].Split('/')[1].Split(' ')[1]);
      string name = Game1.currentLocation.Name;
      if (!str.Equals(name) || Game1.timeOfDay < int32_1 || Game1.timeOfDay >= int32_2)
        return false;
      int num1 = 1;
      while (this.festivalData.ContainsKey("set-up_y" + num1.ToString() + 1.ToString()))
        ++num1;
      int num2 = Game1.year % num1;
      if (num2 == 0)
        num2 = num1;
      this.eventCommands = this.festivalData["set-up"].Split('/');
      if (num2 > 1)
      {
        List<string> stringList = new List<string>((IEnumerable<string>) this.eventCommands);
        stringList.AddRange((IEnumerable<string>) this.festivalData["set-up_y" + num2.ToString()].Split('/'));
        this.eventCommands = stringList.ToArray();
      }
      this.actorPositionsAfterMove = new Dictionary<string, Vector3>();
      this.previousAmbientLight = Game1.ambientLight;
      this.isFestival = true;
      Game1.setRichPresence(nameof (festival), (object) festival);
      return true;
    }

    public string GetFestivalDataForYear(string key)
    {
      int num1 = 1;
      while (this.festivalData.ContainsKey(key + "_y" + (num1 + 1).ToString()))
        ++num1;
      int num2 = Game1.year % num1;
      if (num2 == 0)
        num2 = num1;
      return num2 > 1 ? this.festivalData[key + "_y" + num2.ToString()] : this.festivalData[key];
    }

    public void setExitLocation(string location, int x, int y)
    {
      if (Game1.player.locationBeforeForcedEvent.Value != null && !(Game1.player.locationBeforeForcedEvent.Value == ""))
        return;
      this.exitLocation = Game1.getLocationRequest(location);
      Game1.player.positionBeforeEvent = new Vector2((float) x, (float) y);
    }

    public void endBehaviors(string[] split, GameLocation location)
    {
      if (Game1.getMusicTrackName().Contains(Game1.currentSeason) && !this.eventCommands[0].Equals("continue"))
        Game1.stopMusicTrack(Game1.MusicContext.Default);
      if (split != null && split.Length > 1)
      {
        switch (split[1])
        {
          case "Leo":
            if (!this.isMemory)
            {
              Game1.addMailForTomorrow("leoMoved", true, true);
              Game1.player.team.requestLeoMove.Fire();
              break;
            }
            break;
          case "Maru1":
            Game1.getCharacterFromName("Demetrius").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1018"));
            Game1.getCharacterFromName("Maru").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1020"));
            this.setExitLocation(location.warps[0].TargetName, location.warps[0].TargetX, location.warps[0].TargetY);
            Game1.fadeScreenToBlack();
            Game1.eventOver = true;
            this.CurrentCommand += 2;
            break;
          case "bed":
            Game1.player.Position = Game1.player.mostRecentBed + new Vector2(0.0f, 64f);
            break;
          case "beginGame":
            Game1.gameMode = (byte) 3;
            this.setExitLocation("FarmHouse", 9, 9);
            Game1.NewDay(1000f);
            this.exitEvent();
            Game1.eventFinished();
            return;
          case "busIntro":
            Game1.currentMinigame = (IMinigame) new Intro(4);
            break;
          case "credits":
            Game1.debrisWeather.Clear();
            Game1.isDebrisWeather = false;
            Game1.changeMusicTrack("wedding", music_context: Game1.MusicContext.Event);
            Game1.gameMode = (byte) 10;
            this.CurrentCommand += 2;
            break;
          case "dialogue":
            NPC characterFromName1 = Game1.getCharacterFromName(split[2]);
            int startIndex1 = this.eventCommands[this.CurrentCommand].IndexOf('"') + 1;
            int length1 = this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf('"') + 1).IndexOf('"');
            if (characterFromName1 != null)
            {
              characterFromName1.shouldSayMarriageDialogue.Value = false;
              characterFromName1.currentMarriageDialogue.Clear();
              characterFromName1.CurrentDialogue.Clear();
              characterFromName1.CurrentDialogue.Push(new Dialogue(this.eventCommands[this.CurrentCommand].Substring(startIndex1, length1), characterFromName1));
              break;
            }
            break;
          case "dialogueWarpOut":
            int index1 = 0;
            if (location is BathHousePool && Game1.player.IsMale)
              index1 = 1;
            this.setExitLocation(location.warps[index1].TargetName, location.warps[index1].TargetX, location.warps[index1].TargetY);
            NPC characterFromName2 = Game1.getCharacterFromName(split[2]);
            int startIndex2 = this.eventCommands[this.CurrentCommand].IndexOf('"') + 1;
            int length2 = this.eventCommands[this.CurrentCommand].Substring(this.eventCommands[this.CurrentCommand].IndexOf('"') + 1).IndexOf('"');
            characterFromName2.CurrentDialogue.Clear();
            characterFromName2.CurrentDialogue.Push(new Dialogue(this.eventCommands[this.CurrentCommand].Substring(startIndex2, length2), characterFromName2));
            Game1.eventOver = true;
            this.CurrentCommand += 2;
            Game1.screenGlowHold = false;
            break;
          case "invisible":
            if (!this.isMemory)
            {
              Game1.getCharacterFromName(split[2]).IsInvisible = true;
              break;
            }
            break;
          case "invisibleWarpOut":
            Game1.getCharacterFromName(split[2]).IsInvisible = true;
            this.setExitLocation(location.warps[0].TargetName, location.warps[0].TargetX, location.warps[0].TargetY);
            Game1.fadeScreenToBlack();
            Game1.eventOver = true;
            this.CurrentCommand += 2;
            Game1.screenGlowHold = false;
            break;
          case "islandDepart":
            Game1.player.orientationBeforeEvent = 2;
            if (Game1.whereIsTodaysFest != null && Game1.whereIsTodaysFest == "Beach")
            {
              Game1.player.orientationBeforeEvent = 0;
              this.setExitLocation("Town", 54, 109);
            }
            else if (Game1.whereIsTodaysFest != null && Game1.whereIsTodaysFest == "Town")
            {
              Game1.player.orientationBeforeEvent = 3;
              this.setExitLocation("BusStop", 33, 23);
            }
            else
              this.setExitLocation("BoatTunnel", 6, 9);
            GameLocation left_location = Game1.currentLocation;
            this.exitLocation.OnLoad += (LocationRequest.Callback) (() =>
            {
              foreach (NPC actor in this.actors)
              {
                actor.shouldShadowBeOffset = true;
                actor.drawOffset.Y = 0.0f;
              }
              foreach (Farmer farmerActor in this.farmerActors)
              {
                farmerActor.shouldShadowBeOffset = true;
                farmerActor.drawOffset.Y = 0.0f;
              }
              Game1.player.drawOffset.Value = Vector2.Zero;
              Game1.player.shouldShadowBeOffset = false;
              if (!(left_location is IslandSouth))
                return;
              (left_location as IslandSouth).ResetBoat();
            });
            break;
          case "newDay":
            Game1.player.faceDirection(2);
            this.setExitLocation((string) (NetFieldBase<string, NetString>) Game1.player.homeLocation, (int) Game1.player.mostRecentBed.X / 64, (int) Game1.player.mostRecentBed.Y / 64);
            if (!Game1.IsMultiplayer)
              this.exitLocation.OnWarp += (LocationRequest.Callback) (() =>
              {
                Game1.NewDay(0.0f);
                Game1.player.currentLocation.lastTouchActionLocation = new Vector2((float) ((int) Game1.player.mostRecentBed.X / 64), (float) ((int) Game1.player.mostRecentBed.Y / 64));
              });
            Game1.player.completelyStopAnimatingOrDoingAction();
            if ((bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes)
              Game1.player.changeOutOfSwimSuit();
            Game1.player.swimming.Value = false;
            Game1.player.CanMove = false;
            Game1.changeMusicTrack("none");
            break;
          case "position":
            if (Game1.player.locationBeforeForcedEvent.Value == null || Game1.player.locationBeforeForcedEvent.Value == "")
            {
              Game1.player.positionBeforeEvent = new Vector2((float) Convert.ToInt32(split[2]), (float) Convert.ToInt32(split[3]));
              break;
            }
            break;
          case "tunnelDepart":
            if (Game1.player.hasOrWillReceiveMail("seenBoatJourney"))
            {
              Game1.warpFarmer("IslandSouth", 21, 43, 0);
              break;
            }
            break;
          case "warpOut":
            int index2 = 0;
            if (location is BathHousePool && Game1.player.IsMale)
              index2 = 1;
            this.setExitLocation(location.warps[index2].TargetName, location.warps[index2].TargetX, location.warps[index2].TargetY);
            Game1.eventOver = true;
            this.CurrentCommand += 2;
            Game1.screenGlowHold = false;
            break;
          case "wedding":
            if (this.farmer.IsMale)
            {
              this.farmer.changeShirt(-1);
              this.farmer.changePants(this.oldPants);
              this.farmer.changePantStyle(-1);
              Game1.getCharacterFromName("Lewis").CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1025"), Game1.getCharacterFromName("Lewis")));
            }
            FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(Game1.player);
            Point porchStandingSpot = homeOfFarmer.getPorchStandingSpot();
            if (homeOfFarmer is Cabin)
              this.setExitLocation("Farm", porchStandingSpot.X + 1, porchStandingSpot.Y);
            else
              this.setExitLocation("Farm", porchStandingSpot.X - 1, porchStandingSpot.Y);
            if (Game1.IsMasterGame)
            {
              NPC characterFromName3 = Game1.getCharacterFromName(this.farmer.spouse);
              if (characterFromName3 != null)
              {
                characterFromName3.Schedule = (Dictionary<int, SchedulePathDescription>) null;
                characterFromName3.ignoreScheduleToday = true;
                characterFromName3.shouldPlaySpousePatioAnimation.Value = false;
                characterFromName3.controller = (PathFindController) null;
                characterFromName3.temporaryController = (PathFindController) null;
                characterFromName3.currentMarriageDialogue.Clear();
                Game1.warpCharacter(characterFromName3, "Farm", Utility.getHomeOfFarmer(this.farmer).getPorchStandingSpot());
                characterFromName3.faceDirection(2);
                if (Game1.content.LoadStringReturnNullIfNotFound("Strings\\StringsFromCSFiles:" + characterFromName3.Name + "_AfterWedding") != null)
                {
                  characterFromName3.addMarriageDialogue("Strings\\StringsFromCSFiles", characterFromName3.Name + "_AfterWedding", false);
                  break;
                }
                characterFromName3.addMarriageDialogue("Strings\\StringsFromCSFiles", "Game1.cs.2782", false);
                break;
              }
              break;
            }
            break;
        }
      }
      this.exitEvent();
    }

    public void exitEvent()
    {
      if (this.id != -1 && !Game1.player.eventsSeen.Contains(this.id))
        Game1.player.eventsSeen.Add(this.id);
      if (this.id == 1039573)
        Game1.player.team.requestAddCharacterEvent.Fire("Leo");
      Game1.stopMusicTrack(Game1.MusicContext.Event);
      Game1.player.ignoreCollisions = false;
      Game1.player.canOnlyWalk = false;
      Game1.nonWarpFade = true;
      if (!Game1.fadeIn || (double) Game1.fadeToBlackAlpha >= 1.0)
        Game1.fadeScreenToBlack();
      Game1.eventOver = true;
      Game1.fadeToBlack = true;
      Game1.setBGColor((byte) 5, (byte) 3, (byte) 4);
      this.CurrentCommand += 2;
      Game1.screenGlowHold = false;
      if (this.isFestival)
      {
        Game1.timeOfDayAfterFade = 2200;
        string str = this.festivalData["file"];
        if (this.festivalData != null && (this.festivalData["file"].Equals("summer28") || this.festivalData["file"].Equals("fall27")))
          Game1.timeOfDayAfterFade = 2400;
        int minutesBetweenTimes = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, Game1.timeOfDayAfterFade);
        if (Game1.IsMasterGame)
        {
          Point mainFarmHouseEntry = Game1.getFarm().GetMainFarmHouseEntry();
          this.setExitLocation("Farm", mainFarmHouseEntry.X, mainFarmHouseEntry.Y);
        }
        else
        {
          Point porchStandingSpot = Utility.getHomeOfFarmer(Game1.player).getPorchStandingSpot();
          this.setExitLocation("Farm", porchStandingSpot.X, porchStandingSpot.Y);
        }
        Game1.player.toolOverrideFunction = (AnimatedSprite.endOfAnimationBehavior) null;
        this.isFestival = false;
        foreach (NPC actor in this.actors)
        {
          if (actor != null)
            this.resetDialogueIfNecessary(actor);
        }
        if (Game1.IsMasterGame)
        {
          foreach (NPC allCharacter in Utility.getAllCharacters())
          {
            if (allCharacter.isVillager())
            {
              if (allCharacter.getSpouse() != null)
              {
                Farmer spouse = allCharacter.getSpouse();
                if (spouse.isMarried())
                {
                  allCharacter.controller = (PathFindController) null;
                  allCharacter.temporaryController = (PathFindController) null;
                  FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(spouse);
                  allCharacter.Halt();
                  Game1.warpCharacter(allCharacter, (GameLocation) homeOfFarmer, Utility.PointToVector2(homeOfFarmer.getSpouseBedSpot(spouse.spouse)));
                  if (homeOfFarmer.GetSpouseBed() != null)
                    FarmHouse.spouseSleepEndFunction((Character) allCharacter, (GameLocation) Utility.getHomeOfFarmer(spouse));
                  allCharacter.ignoreScheduleToday = true;
                  if (Game1.timeOfDayAfterFade >= 1800)
                  {
                    allCharacter.currentMarriageDialogue.Clear();
                    allCharacter.checkForMarriageDialogue(1800, (GameLocation) Utility.getHomeOfFarmer(spouse));
                    continue;
                  }
                  if (Game1.timeOfDayAfterFade >= 1100)
                  {
                    allCharacter.currentMarriageDialogue.Clear();
                    allCharacter.checkForMarriageDialogue(1100, (GameLocation) Utility.getHomeOfFarmer(spouse));
                    continue;
                  }
                  continue;
                }
              }
              if (allCharacter.currentLocation != null && allCharacter.defaultMap.Value != null)
              {
                allCharacter.doingEndOfRouteAnimation.Value = false;
                allCharacter.nextEndOfRouteMessage = (string) null;
                allCharacter.endOfRouteMessage.Value = (string) null;
                allCharacter.controller = (PathFindController) null;
                allCharacter.temporaryController = (PathFindController) null;
                allCharacter.Halt();
                Game1.warpCharacter(allCharacter, (string) (NetFieldBase<string, NetString>) allCharacter.defaultMap, allCharacter.DefaultPosition / 64f);
                allCharacter.ignoreScheduleToday = true;
              }
            }
          }
        }
        foreach (GameLocation location in (IEnumerable<GameLocation>) Game1.locations)
        {
          foreach (Vector2 key in new List<Vector2>((IEnumerable<Vector2>) location.objects.Keys))
          {
            if (location.objects[key].minutesElapsed(minutesBetweenTimes, location))
              location.objects.Remove(key);
          }
          if (location is Farm)
            (location as Farm).timeUpdate(minutesBetweenTimes);
        }
        Game1.player.freezePause = 1500;
      }
      else
        Game1.player.forceCanMove();
    }

    public void resetDialogueIfNecessary(NPC n)
    {
      if (!Game1.player.hasTalkedToFriendToday(n.Name))
      {
        n.resetCurrentDialogue();
      }
      else
      {
        if (n.CurrentDialogue == null)
          return;
        n.CurrentDialogue.Clear();
      }
    }

    public void incrementCommandAfterFade()
    {
      ++this.CurrentCommand;
      Game1.globalFade = false;
    }

    public void cleanup()
    {
      Game1.ambientLight = this.previousAmbientLight;
      foreach (NPC withUniquePortrait in this.npcsWithUniquePortraits)
      {
        withUniquePortrait.Portrait = Game1.content.Load<Texture2D>("Portraits\\" + withUniquePortrait.Name);
        withUniquePortrait.uniquePortraitActive = false;
      }
      if (this._festivalTexture != null)
        this._festivalTexture = (Texture2D) null;
      this.festivalContent.Unload();
    }

    private void changeLocation(
      string locationName,
      int x,
      int y,
      int direction = -1,
      Action onComplete = null)
    {
      if (direction == -1)
        direction = Game1.player.FacingDirection;
      Event e = Game1.currentLocation.currentEvent;
      Game1.currentLocation.currentEvent = (Event) null;
      LocationRequest locationRequest = Game1.getLocationRequest(locationName);
      locationRequest.OnLoad += (LocationRequest.Callback) (() =>
      {
        if (!e.isFestival)
          Game1.currentLocation.currentEvent = e;
        this.temporaryLocation = (GameLocation) null;
        if (onComplete == null)
          return;
        onComplete();
      });
      locationRequest.OnWarp += (LocationRequest.Callback) (() =>
      {
        this.farmer.currentLocation = Game1.currentLocation;
        if (!e.isFestival)
          return;
        Game1.currentLocation.currentEvent = e;
      });
      Game1.warpFarmer(locationRequest, x, y, this.farmer.FacingDirection);
    }

    public void LogErrorAndHalt(Exception e)
    {
      Game1.chatBox.addErrorMessage("Event script error: " + e.Message);
      if (this.eventCommands == null || this.eventCommands.Length == 0 || this.CurrentCommand >= this.eventCommands.Length)
        return;
      Game1.chatBox.addErrorMessage("On line #" + this.CurrentCommand.ToString() + ": " + this.eventCommands[this.CurrentCommand]);
      this.skipEvent();
    }

    public void checkForNextCommand(GameLocation location, GameTime time)
    {
      try
      {
        this._checkForNextCommand(location, time);
      }
      catch (Exception ex)
      {
        this.LogErrorAndHalt(ex);
      }
    }

    protected void _checkForNextCommand(GameLocation location, GameTime time)
    {
      if (this.skipped || Game1.farmEvent != null)
        return;
      foreach (NPC actor in this.actors)
      {
        actor.update(time, Game1.currentLocation);
        if (actor.Sprite.CurrentAnimation != null)
          actor.Sprite.animateOnce(time);
      }
      if (this.aboveMapSprites != null)
      {
        for (int index = this.aboveMapSprites.Count - 1; index >= 0; --index)
        {
          if (this.aboveMapSprites[index].update(time))
            this.aboveMapSprites.RemoveAt(index);
        }
      }
      if (this.underwaterSprites != null)
      {
        foreach (TemporaryAnimatedSprite underwaterSprite in this.underwaterSprites)
          underwaterSprite.update(time);
      }
      if (!this.playerControlSequence)
        this.farmer.setRunning(false);
      if (this.npcControllers != null)
      {
        for (int index = this.npcControllers.Count - 1; index >= 0; --index)
        {
          this.npcControllers[index].puppet.isCharging = !this.isFestival;
          if (this.npcControllers[index].update(time, location, this.npcControllers))
            this.npcControllers.RemoveAt(index);
        }
      }
      if (this.isFestival)
        this.festivalUpdate(time);
      string[] split = this.eventCommands[Math.Min(this.eventCommands.Length - 1, this.CurrentCommand)].Split(' ');
      if (this.temporaryLocation != null && !Game1.currentLocation.Equals(this.temporaryLocation))
        this.temporaryLocation.updateEvenIfFarmerIsntHere(time, true);
      if (split.Length != 0 && split[0].StartsWith("--"))
        ++this.CurrentCommand;
      else if (this.CurrentCommand == 0 && !this.forked && !this.eventSwitched)
      {
        this.farmer.speed = 2;
        this.farmer.running = false;
        Game1.eventOver = false;
        if (this.eventCommands.Length > 3 && this.eventCommands[3] == "ignoreEventTileOffset")
          this.ignoreTileOffsets = true;
        if ((!this.eventCommands[0].Equals("none") || !Game1.isRaining) && !this.eventCommands[0].Equals("continue") && !this.eventCommands[0].Contains("pause"))
          Game1.changeMusicTrack(this.eventCommands[0], music_context: Game1.MusicContext.Event);
        if (location is Farm && Convert.ToInt32(this.eventCommands[1].Split(' ')[0]) >= -1000 && this.id != -2 && !this.ignoreTileOffsets)
        {
          Point positionForFarmer = Farm.getFrontDoorPositionForFarmer(this.farmer);
          positionForFarmer.X *= 64;
          positionForFarmer.Y *= 64;
          Game1.viewport.X = Game1.currentLocation.IsOutdoors ? Math.Max(0, Math.Min(positionForFarmer.X - Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.currentLocation.Map.DisplayWidth - Game1.graphics.GraphicsDevice.Viewport.Width)) : positionForFarmer.X - Game1.graphics.GraphicsDevice.Viewport.Width / 2;
          Game1.viewport.Y = Game1.currentLocation.IsOutdoors ? Math.Max(0, Math.Min(positionForFarmer.Y - Game1.graphics.GraphicsDevice.Viewport.Height / 2, Game1.currentLocation.Map.DisplayHeight - Game1.graphics.GraphicsDevice.Viewport.Height)) : positionForFarmer.Y - Game1.graphics.GraphicsDevice.Viewport.Height / 2;
        }
        else if (!this.eventCommands[1].Equals("follow"))
        {
          try
          {
            string[] strArray = this.eventCommands[1].Split(' ');
            Game1.viewportFreeze = true;
            int num1 = this.OffsetTileX(Convert.ToInt32(strArray[0])) * 64 + 32;
            int num2 = this.OffsetTileY(Convert.ToInt32(strArray[1])) * 64 + 32;
            if (strArray[0][0] == '-')
            {
              Game1.viewport.X = num1;
              Game1.viewport.Y = num2;
            }
            else
            {
              Game1.viewport.X = Game1.currentLocation.IsOutdoors ? Math.Max(0, Math.Min(num1 - Game1.viewport.Width / 2, Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width)) : num1 - Game1.viewport.Width / 2;
              Game1.viewport.Y = Game1.currentLocation.IsOutdoors ? Math.Max(0, Math.Min(num2 - Game1.viewport.Height / 2, Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height)) : num2 - Game1.viewport.Height / 2;
            }
            Viewport viewport;
            if (num1 > 0)
            {
              viewport = Game1.graphics.GraphicsDevice.Viewport;
              if (viewport.Width > Game1.currentLocation.Map.DisplayWidth)
                Game1.viewport.X = (Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width) / 2;
            }
            if (num2 > 0)
            {
              viewport = Game1.graphics.GraphicsDevice.Viewport;
              if (viewport.Height > Game1.currentLocation.Map.DisplayHeight)
                Game1.viewport.Y = (Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height) / 2;
            }
          }
          catch (Exception ex)
          {
            this.forked = true;
            return;
          }
        }
        this.setUpCharacters(this.eventCommands[2], location);
        this.trySpecialSetUp(location);
        this.populateWalkLocationsList();
        this.CurrentCommand = 3;
      }
      else
      {
        if (!Game1.fadeToBlack || this.actorPositionsAfterMove.Count > 0 || this.CurrentCommand > 3 || this.forked)
        {
          if (this.eventCommands.Length <= this.CurrentCommand)
            return;
          Vector3 viewportTarget = this.viewportTarget;
          if (!this.viewportTarget.Equals(Vector3.Zero))
          {
            int speed = this.farmer.speed;
            this.farmer.speed = (int) this.viewportTarget.X;
            Game1.viewport.X += (int) this.viewportTarget.X;
            if ((double) this.viewportTarget.X != 0.0)
              Game1.updateRainDropPositionForPlayerMovement((double) this.viewportTarget.X < 0.0 ? 3 : 1, true, Math.Abs(this.viewportTarget.X + (!this.farmer.isMoving() || this.farmer.FacingDirection != 3 ? (!this.farmer.isMoving() || this.farmer.FacingDirection != 1 ? 0.0f : (float) this.farmer.speed) : (float) -this.farmer.speed)));
            Game1.viewport.Y += (int) this.viewportTarget.Y;
            this.farmer.speed = (int) this.viewportTarget.Y;
            if ((double) this.viewportTarget.Y != 0.0)
              Game1.updateRainDropPositionForPlayerMovement((double) this.viewportTarget.Y < 0.0 ? 0 : 2, true, Math.Abs(this.viewportTarget.Y - (!this.farmer.isMoving() || this.farmer.FacingDirection != 0 ? (!this.farmer.isMoving() || this.farmer.FacingDirection != 2 ? 0.0f : (float) this.farmer.speed) : (float) -this.farmer.speed)));
            this.farmer.speed = speed;
            this.viewportTarget.Z -= (float) time.ElapsedGameTime.Milliseconds;
            if ((double) this.viewportTarget.Z <= 0.0)
              this.viewportTarget = Vector3.Zero;
          }
          if (this.actorPositionsAfterMove.Count > 0)
          {
            foreach (string str in this.actorPositionsAfterMove.Keys.ToArray<string>())
            {
              Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) this.actorPositionsAfterMove[str].X * 64, (int) this.actorPositionsAfterMove[str].Y * 64, 64, 64);
              rectangle.Inflate(-4, 0);
              if (this.getActorByName(str) != null && this.getActorByName(str).GetBoundingBox().Width > 64)
              {
                rectangle.Inflate(4, 0);
                rectangle.Width = this.getActorByName(str).GetBoundingBox().Width + 4;
                rectangle.Height = this.getActorByName(str).GetBoundingBox().Height + 4;
                rectangle.X += 8;
                rectangle.Y += 16;
              }
              if (str.Contains("farmer"))
              {
                Farmer farmerNumberString = this.getFarmerFromFarmerNumberString(str, this.farmer);
                if (farmerNumberString != null && rectangle.Contains(farmerNumberString.GetBoundingBox()) && ((double) (farmerNumberString.GetBoundingBox().Y - rectangle.Top) <= 16.0 + (double) farmerNumberString.getMovementSpeed() && farmerNumberString.FacingDirection != 2 || (double) (rectangle.Bottom - farmerNumberString.GetBoundingBox().Bottom) <= 16.0 + (double) farmerNumberString.getMovementSpeed() && farmerNumberString.FacingDirection == 2))
                {
                  farmerNumberString.showNotCarrying();
                  farmerNumberString.Halt();
                  farmerNumberString.faceDirection((int) this.actorPositionsAfterMove[str].Z);
                  farmerNumberString.FarmerSprite.StopAnimation();
                  farmerNumberString.Halt();
                  this.actorPositionsAfterMove.Remove(str);
                }
                else if (farmerNumberString != null)
                {
                  farmerNumberString.canOnlyWalk = false;
                  farmerNumberString.setRunning(false, true);
                  farmerNumberString.canOnlyWalk = true;
                  farmerNumberString.lastPosition = this.farmer.Position;
                  farmerNumberString.MovePosition(time, Game1.viewport, location);
                }
              }
              else
              {
                foreach (NPC actor in this.actors)
                {
                  Microsoft.Xna.Framework.Rectangle boundingBox = actor.GetBoundingBox();
                  if (actor.Name.Equals(str) && rectangle.Contains(boundingBox) && actor.GetBoundingBox().Y - rectangle.Top <= 16)
                  {
                    actor.Halt();
                    actor.faceDirection((int) this.actorPositionsAfterMove[str].Z);
                    this.actorPositionsAfterMove.Remove(str);
                    break;
                  }
                  if (actor.Name.Equals(str))
                  {
                    if (actor is Monster)
                    {
                      actor.MovePosition(time, Game1.viewport, location);
                      break;
                    }
                    actor.MovePosition(time, Game1.viewport, (GameLocation) null);
                    break;
                  }
                }
              }
            }
            if (this.actorPositionsAfterMove.Count == 0)
            {
              if (this.continueAfterMove)
                this.continueAfterMove = false;
              else
                ++this.CurrentCommand;
            }
            if (!this.continueAfterMove)
              return;
          }
        }
        this.tryEventCommand(location, time, split);
      }
    }

    public bool isTileWalkedOn(int x, int y) => this.characterWalkLocations.Contains(new Vector2((float) x, (float) y));

    private void populateWalkLocationsList()
    {
      Vector2 tileLocation1 = this.farmer.getTileLocation();
      this.characterWalkLocations.Add(tileLocation1);
      for (int index1 = 2; index1 < this.eventCommands.Length; ++index1)
      {
        string[] strArray = this.eventCommands[index1].Split(' ');
        if (strArray[0] == "move" && strArray[1].Equals("farmer"))
        {
          for (int index2 = 0; index2 < Math.Abs(Convert.ToInt32(strArray[2])); ++index2)
          {
            tileLocation1.X += (float) Math.Sign(Convert.ToInt32(strArray[2]));
            this.characterWalkLocations.Add(tileLocation1);
          }
          for (int index3 = 0; index3 < Math.Abs(Convert.ToInt32(strArray[3])); ++index3)
          {
            tileLocation1.Y += (float) Math.Sign(Convert.ToInt32(strArray[3]));
            this.characterWalkLocations.Add(tileLocation1);
          }
        }
      }
      foreach (NPC actor in this.actors)
      {
        Vector2 tileLocation2 = actor.getTileLocation();
        this.characterWalkLocations.Add(tileLocation2);
        for (int index4 = 2; index4 < this.eventCommands.Length; ++index4)
        {
          string[] strArray = this.eventCommands[index4].Split(' ');
          if (strArray[0] == "move" && strArray[1].Equals(actor.Name))
          {
            for (int index5 = 0; index5 < Math.Abs(Convert.ToInt32(strArray[2])); ++index5)
            {
              tileLocation2.X += (float) Math.Sign(Convert.ToInt32(strArray[2]));
              this.characterWalkLocations.Add(tileLocation2);
            }
            for (int index6 = 0; index6 < Math.Abs(Convert.ToInt32(strArray[3])); ++index6)
            {
              tileLocation2.Y += (float) Math.Sign(Convert.ToInt32(strArray[3]));
              this.characterWalkLocations.Add(tileLocation2);
            }
          }
        }
      }
    }

    public NPC getActorByName(string name)
    {
      if (name.Equals("rival"))
        name = Utility.getOtherFarmerNames()[0];
      if (name.Equals("spouse"))
        name = this.farmer.spouse;
      foreach (NPC actor in this.actors)
      {
        if (actor.Name.Equals(name))
          return actor;
      }
      return (NPC) null;
    }

    public void applyToAllFarmersByFarmerString(string farmer_string, Action<Farmer> function)
    {
      List<Farmer> farmerList = new List<Farmer>();
      if (farmer_string.Equals("farmer"))
        farmerList.Add(this.farmer);
      else if (farmer_string.StartsWith("farmer"))
        farmerList.Add(this.getFarmerFromFarmerNumberString(farmer_string, this.farmer));
      foreach (Farmer farmer in farmerList)
      {
        bool flag = false;
        foreach (Farmer farmerActor in this.farmerActors)
        {
          if (farmerActor.UniqueMultiplayerID == farmer.UniqueMultiplayerID)
          {
            flag = true;
            function(farmerActor);
            break;
          }
        }
        if (!flag)
          function(farmer);
      }
    }

    private void addActor(string name, int x, int y, int facingDirection, GameLocation location)
    {
      string nameForCharacter = NPC.getTextureNameForCharacter(name);
      if (name.Equals("Krobus_Trenchcoat"))
        name = "Krobus";
      Texture2D portrait = (Texture2D) null;
      try
      {
        portrait = Game1.content.Load<Texture2D>("Portraits\\" + (nameForCharacter.Equals("WeddingOutfits") ? this.farmer.spouse : nameForCharacter));
      }
      catch (Exception ex)
      {
      }
      int num = name.Contains("Dwarf") || name.Equals("Krobus") ? 96 : 128;
      NPC npc = new NPC(new AnimatedSprite("Characters\\" + nameForCharacter, 0, 16, num / 4), new Vector2((float) (x * 64), (float) (y * 64)), location.Name, facingDirection, name.Contains("Rival") ? Utility.getOtherFarmerNames()[0] : name, (Dictionary<int, int[]>) null, portrait, true);
      npc.eventActor = true;
      if (this.isFestival)
      {
        try
        {
          npc.setNewDialogue(this.GetFestivalDataForYear(npc.Name));
        }
        catch (Exception ex)
        {
        }
      }
      if (npc.name.Equals((object) "MrQi"))
        npc.displayName = Game1.content.LoadString("Strings\\NPCNames:MisterQi");
      npc.eventActor = true;
      this.actors.Add(npc);
    }

    public Farmer getFarmerFromFarmerNumberString(string name, Farmer defaultFarmer)
    {
      Farmer farmerNumberString = Utility.getFarmerFromFarmerNumberString(name, defaultFarmer);
      if (farmerNumberString == null)
        return (Farmer) null;
      foreach (Farmer farmerActor in this.farmerActors)
      {
        if (farmerNumberString.UniqueMultiplayerID == farmerActor.UniqueMultiplayerID)
          return farmerActor;
      }
      return farmerNumberString;
    }

    public Character getCharacterByName(string name)
    {
      if (name.Equals("rival"))
        name = Utility.getOtherFarmerNames()[0];
      if (name.Contains("farmer"))
        return (Character) this.getFarmerFromFarmerNumberString(name, this.farmer);
      foreach (NPC actor in this.actors)
      {
        if (actor.Name.Equals(name))
          return (Character) actor;
      }
      return (Character) null;
    }

    public Vector3 getPositionAfterMove(
      Character c,
      int xMove,
      int yMove,
      int facingDirection)
    {
      Vector2 tileLocation = c.getTileLocation();
      return new Vector3(tileLocation.X + (float) xMove, tileLocation.Y + (float) yMove, (float) facingDirection);
    }

    private void trySpecialSetUp(GameLocation location)
    {
      switch (this.id)
      {
        case 739330:
          if (!Game1.player.friendshipData.ContainsKey("Willy"))
            Game1.player.friendshipData.Add("Willy", new Friendship(0));
          Game1.player.checkForQuestComplete(Game1.getCharacterFromName("Willy"), -1, -1, (Item) null, (string) null, 5);
          break;
        case 3912132:
          if (!(location is FarmHouse))
            break;
          Point playerBedSpot1 = (location as FarmHouse).GetPlayerBedSpot();
          --playerBedSpot1.X;
          if (!location.isTileLocationTotallyClearAndPlaceable(Utility.PointToVector2(playerBedSpot1) + new Vector2(-2f, 0.0f)))
            ++playerBedSpot1.X;
          this.farmer.setTileLocation(Utility.PointToVector2(playerBedSpot1));
          this.getActorByName("Elliott").setTileLocation(Utility.PointToVector2(playerBedSpot1) + new Vector2(-2f, 0.0f));
          for (int index = 0; index < this.eventCommands.Length; ++index)
          {
            if (this.eventCommands[index].StartsWith("makeInvisible"))
            {
              string[] strArray = this.eventCommands[index].Split(' ');
              strArray[1] = (int.Parse(strArray[1]) - 26 + playerBedSpot1.X).ToString() ?? "";
              strArray[2] = (int.Parse(strArray[2]) - 13 + playerBedSpot1.Y).ToString() ?? "";
              this.eventCommands[index] = location.getObjectAtTile(int.Parse(strArray[1]), int.Parse(strArray[2])) != (location as FarmHouse).GetPlayerBed() ? string.Join(" ", strArray) : "makeInvisible -1000 -1000";
            }
          }
          break;
        case 3917601:
          if (!(location is DecoratableLocation))
            break;
          foreach (Furniture furniture in (location as DecoratableLocation).furniture)
          {
            if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type == 14 && location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(furniture.TileLocation + new Vector2(0.0f, 1f)) && location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(furniture.TileLocation + new Vector2(1f, 1f)))
            {
              this.getActorByName("Emily").setTilePosition((int) furniture.TileLocation.X, (int) furniture.TileLocation.Y + 1);
              this.farmer.Position = new Vector2((float) (((double) furniture.TileLocation.X + 1.0) * 64.0), (float) (((double) furniture.tileLocation.Y + 1.0) * 64.0 + 16.0));
              furniture.isOn.Value = true;
              furniture.setFireplace(location, false);
              return;
            }
          }
          if (!(location is FarmHouse) || (location as FarmHouse).upgradeLevel != 1)
            break;
          this.getActorByName("Emily").setTilePosition(4, 5);
          this.farmer.Position = new Vector2(320f, 336f);
          break;
        case 3917666:
          if (!(location is FarmHouse) || (location as FarmHouse).upgradeLevel != 1)
            break;
          this.getActorByName("Maru").setTilePosition(4, 5);
          this.farmer.Position = new Vector2(320f, 336f);
          break;
        case 4324303:
          if (!(location is FarmHouse))
            break;
          Point playerBedSpot2 = (location as FarmHouse).GetPlayerBedSpot();
          --playerBedSpot2.X;
          this.farmer.Position = new Vector2((float) (playerBedSpot2.X * 64), (float) (playerBedSpot2.Y * 64 + 16));
          this.getActorByName("Penny").setTilePosition(playerBedSpot2.X - 1, playerBedSpot2.Y);
          Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(23, 12, 10, 10);
          if ((location as FarmHouse).upgradeLevel == 1)
            rectangle = new Microsoft.Xna.Framework.Rectangle(20, 3, 8, 7);
          Point center = rectangle.Center;
          int num1;
          if (!rectangle.Contains(Game1.player.getTileLocationPoint()))
          {
            List<string> stringList1 = new List<string>((IEnumerable<string>) this.eventCommands);
            int index1 = 56;
            stringList1.Insert(index1, "globalFade 0.03");
            int index2 = index1 + 1;
            stringList1.Insert(index2, "beginSimultaneousCommand");
            int index3 = index2 + 1;
            stringList1.Insert(index3, "viewport " + center.X.ToString() + " " + center.Y.ToString());
            int index4 = index3 + 1;
            stringList1.Insert(index4, "globalFadeToClear 0.03");
            int index5 = index4 + 1;
            stringList1.Insert(index5, "endSimultaneousCommand");
            int index6 = index5 + 1;
            stringList1.Insert(index6, "pause 2000");
            int index7 = index6 + 1;
            stringList1.Insert(index7, "globalFade 0.03");
            int index8 = index7 + 1;
            stringList1.Insert(index8, "beginSimultaneousCommand");
            int num2 = index8 + 1;
            List<string> stringList2 = stringList1;
            int index9 = num2;
            num1 = Game1.player.getTileX();
            string str1 = num1.ToString();
            num1 = Game1.player.getTileY();
            string str2 = num1.ToString();
            string str3 = "viewport " + str1 + " " + str2;
            stringList2.Insert(index9, str3);
            int index10 = num2 + 1;
            stringList1.Insert(index10, "globalFadeToClear 0.03");
            int index11 = index10 + 1;
            stringList1.Insert(index11, "endSimultaneousCommand");
            int num3 = index11 + 1;
            this.eventCommands = stringList1.ToArray();
          }
          for (int index = 0; index < this.eventCommands.Length; ++index)
          {
            if (this.eventCommands[index].StartsWith("makeInvisible"))
            {
              string[] strArray1 = this.eventCommands[index].Split(' ');
              string[] strArray2 = strArray1;
              num1 = int.Parse(strArray1[1]) - 26 + playerBedSpot2.X;
              string str4 = num1.ToString() ?? "";
              strArray2[1] = str4;
              string[] strArray3 = strArray1;
              num1 = int.Parse(strArray1[2]) - 13 + playerBedSpot2.Y;
              string str5 = num1.ToString() ?? "";
              strArray3[2] = str5;
              this.eventCommands[index] = location.getObjectAtTile(int.Parse(strArray1[1]), int.Parse(strArray1[2])) != (location as FarmHouse).GetPlayerBed() ? string.Join(" ", strArray1) : "makeInvisible -1000 -1000";
            }
          }
          break;
        case 4325434:
          if (!(location is FarmHouse) || (location as FarmHouse).upgradeLevel != 1)
            break;
          this.farmer.Position = new Vector2(512f, 336f);
          this.getActorByName("Penny").setTilePosition(5, 5);
          break;
        case 8675611:
          if (!(location is FarmHouse) || (location as FarmHouse).upgradeLevel != 1)
            break;
          this.getActorByName("Haley").setTilePosition(4, 5);
          this.farmer.Position = new Vector2(320f, 336f);
          break;
        case 9333220:
          if (!(location is FarmHouse) || (location as FarmHouse).upgradeLevel != 1)
            break;
          this.farmer.Position = new Vector2(1920f, 400f);
          this.getActorByName("Sebastian").setTilePosition(31, 6);
          break;
      }
    }

    private void setUpCharacters(string description, GameLocation location)
    {
      this.farmer.Halt();
      if ((Game1.player.locationBeforeForcedEvent.Value == null || Game1.player.locationBeforeForcedEvent.Value == "") && !this.isMemory)
      {
        Game1.player.positionBeforeEvent = Game1.player.getTileLocation();
        Game1.player.orientationBeforeEvent = Game1.player.FacingDirection;
      }
      string[] strArray = description.Split(' ');
      for (int index = 0; index < strArray.Length; index += 4)
      {
        if (strArray[index + 1].Equals("-1") && !strArray[index].Equals("farmer"))
        {
          foreach (NPC character in location.getCharacters())
          {
            if (character.Name.Equals(strArray[index]))
              this.actors.Add(character);
          }
        }
        else if (!strArray[index].Equals("farmer"))
        {
          if (strArray[index].Equals("otherFarmers"))
          {
            int x = this.OffsetTileX(Convert.ToInt32(strArray[index + 1]));
            int y = this.OffsetTileY(Convert.ToInt32(strArray[index + 2]));
            int int32 = Convert.ToInt32(strArray[index + 3]);
            foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
            {
              if (onlineFarmer.UniqueMultiplayerID != this.farmer.UniqueMultiplayerID)
              {
                Farmer fakeEventFarmer = onlineFarmer.CreateFakeEventFarmer();
                fakeEventFarmer.completelyStopAnimatingOrDoingAction();
                fakeEventFarmer.hidden.Value = false;
                fakeEventFarmer.faceDirection(int32);
                fakeEventFarmer.setTileLocation(new Vector2((float) x, (float) y));
                fakeEventFarmer.currentLocation = Game1.currentLocation;
                ++x;
                this.farmerActors.Add(fakeEventFarmer);
              }
            }
          }
          else if (strArray[index].Contains("farmer"))
          {
            int x = this.OffsetTileX(Convert.ToInt32(strArray[index + 1]));
            int y = this.OffsetTileY(Convert.ToInt32(strArray[index + 2]));
            int int32 = Convert.ToInt32(strArray[index + 3]);
            Farmer fromFarmerNumber = Utility.getFarmerFromFarmerNumber(Convert.ToInt32(strArray[index].Last<char>().ToString() ?? ""));
            if (fromFarmerNumber != null)
            {
              Farmer fakeEventFarmer = fromFarmerNumber.CreateFakeEventFarmer();
              fakeEventFarmer.completelyStopAnimatingOrDoingAction();
              fakeEventFarmer.hidden.Value = false;
              fakeEventFarmer.faceDirection(int32);
              fakeEventFarmer.setTileLocation(new Vector2((float) x, (float) y));
              fakeEventFarmer.currentLocation = Game1.currentLocation;
              fakeEventFarmer.isFakeEventActor = true;
              this.farmerActors.Add(fakeEventFarmer);
            }
          }
          else
          {
            string name = strArray[index];
            if (strArray[index].Equals("spouse"))
              name = this.farmer.spouse;
            if (strArray[index].Equals("rival"))
              name = this.farmer.IsMale ? "maleRival" : "femaleRival";
            if (strArray[index].Equals("cat"))
            {
              this.actors.Add((NPC) new Cat(this.OffsetTileX(Convert.ToInt32(strArray[index + 1])), this.OffsetTileY(Convert.ToInt32(strArray[index + 2])), Game1.player.whichPetBreed));
              this.actors.Last<NPC>().Name = "Cat";
              this.actors.Last<NPC>().position.X -= 32f;
            }
            else if (strArray[index].Equals("dog"))
            {
              this.actors.Add((NPC) new Dog(this.OffsetTileX(Convert.ToInt32(strArray[index + 1])), this.OffsetTileY(Convert.ToInt32(strArray[index + 2])), Game1.player.whichPetBreed));
              this.actors.Last<NPC>().Name = "Dog";
              this.actors.Last<NPC>().position.X -= 42f;
            }
            else if (strArray[index].Equals("golem"))
              this.actors.Add(new NPC(new AnimatedSprite("Characters\\Monsters\\Wilderness Golem", 0, 16, 24), this.OffsetPosition(new Vector2((float) Convert.ToInt32(strArray[index + 1]), (float) Convert.ToInt32(strArray[index + 2])) * 64f), 0, "Golem"));
            else if (strArray[index].Equals("Junimo"))
            {
              List<NPC> actors = this.actors;
              Junimo junimo = new Junimo(this.OffsetPosition(new Vector2((float) (Convert.ToInt32(strArray[index + 1]) * 64), (float) (Convert.ToInt32(strArray[index + 2]) * 64 - 32))), Game1.currentLocation.Name.Equals("AbandonedJojaMart") ? 6 : -1);
              junimo.Name = "Junimo";
              junimo.EventActor = true;
              actors.Add((NPC) junimo);
            }
            else
            {
              int x = this.OffsetTileX(Convert.ToInt32(strArray[index + 1]));
              int y = this.OffsetTileY(Convert.ToInt32(strArray[index + 2]));
              int facingDirection = Convert.ToInt32(strArray[index + 3]);
              if (location is Farm && this.id != -2 && !this.ignoreTileOffsets)
              {
                x = Farm.getFrontDoorPositionForFarmer(this.farmer).X;
                y = Farm.getFrontDoorPositionForFarmer(this.farmer).Y + 2;
                facingDirection = 0;
              }
              this.addActor(name, x, y, facingDirection, location);
            }
          }
        }
        else if (!strArray[index + 1].Equals("-1"))
        {
          this.farmer.position.X = this.OffsetPositionX((float) (Convert.ToInt32(strArray[index + 1]) * 64));
          this.farmer.position.Y = this.OffsetPositionY((float) (Convert.ToInt32(strArray[index + 2]) * 64 + 16));
          this.farmer.faceDirection(Convert.ToInt32(strArray[index + 3]));
          if (location is Farm && this.id != -2 && !this.ignoreTileOffsets)
          {
            this.farmer.position.X = (float) (Farm.getFrontDoorPositionForFarmer(this.farmer).X * 64);
            this.farmer.position.Y = (float) ((Farm.getFrontDoorPositionForFarmer(this.farmer).Y + 1) * 64);
            this.farmer.faceDirection(2);
          }
          this.farmer.FarmerSprite.StopAnimation();
        }
      }
    }

    private void beakerSmashEndFunction(int extraInfo)
    {
      Game1.playSound("breakingGlass");
      Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(47, new Vector2(9f, 16f) * 64f, Microsoft.Xna.Framework.Color.LightBlue, 10));
      Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(400, 3008, 64, 64), 99999f, 2, 0, new Vector2(9f, 16f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.LightBlue, 1f, 0.0f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = 700
      });
      Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(46, new Vector2(9f, 16f) * 64f, Microsoft.Xna.Framework.Color.White * 0.75f, 10)
      {
        motion = new Vector2(0.0f, -1f)
      });
    }

    private void eggSmashEndFunction(int extraInfo)
    {
      Game1.playSound("slimedead");
      Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(47, new Vector2(9f, 16f) * 64f, Microsoft.Xna.Framework.Color.White, 10));
      Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(177, 99999f, 9999, 0, new Vector2(6f, 5f) * 64f, false, false)
      {
        layerDepth = 1E-06f
      });
    }

    private void balloonInSky(int extraInfo)
    {
      TemporaryAnimatedSprite temporarySpriteById1 = Game1.currentLocation.getTemporarySpriteByID(2);
      if (temporarySpriteById1 != null)
        temporarySpriteById1.motion = Vector2.Zero;
      TemporaryAnimatedSprite temporarySpriteById2 = Game1.currentLocation.getTemporarySpriteByID(1);
      if (temporarySpriteById2 == null)
        return;
      temporarySpriteById2.motion = Vector2.Zero;
    }

    private void marcelloBalloonLand(int extraInfo)
    {
      Game1.playSound("thudStep");
      Game1.playSound("dirtyHit");
      TemporaryAnimatedSprite temporarySpriteById1 = Game1.currentLocation.getTemporarySpriteByID(2);
      if (temporarySpriteById1 != null)
        temporarySpriteById1.motion = Vector2.Zero;
      TemporaryAnimatedSprite temporarySpriteById2 = Game1.currentLocation.getTemporarySpriteByID(3);
      if (temporarySpriteById2 != null)
        temporarySpriteById2.scaleChange = 0.0f;
      Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 2944, 64, 64), 120f, 8, 1, (new Vector2(25f, 39f) + this.eventPositionTileOffset) * 64f + new Vector2(-32f, 32f), false, true, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f));
      Game1.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 2944, 64, 64), 120f, 8, 1, (new Vector2(27f, 39f) + this.eventPositionTileOffset) * 64f + new Vector2(0.0f, 48f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
      {
        delayBeforeAnimationStart = 300
      });
      ++this.CurrentCommand;
    }

    private void samPreOllie(int extraInfo)
    {
      this.getActorByName("Sam").Sprite.currentFrame = 27;
      this.farmer.faceDirection(0);
      TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
      temporarySpriteById.xStopCoordinate = 1408;
      temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.samOllie);
      temporarySpriteById.motion = new Vector2(2f, 0.0f);
    }

    private void samOllie(int extraInfo)
    {
      Game1.playSound("crafting");
      this.getActorByName("Sam").Sprite.currentFrame = 26;
      TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 1;
      temporarySpriteById.motion.Y = -9f;
      temporarySpriteById.motion.X = 2f;
      temporarySpriteById.acceleration = new Vector2(0.0f, 0.4f);
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.interval = 530f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.endFunction = new TemporaryAnimatedSprite.endBehavior(this.samGrind);
      temporarySpriteById.destroyable = false;
    }

    private void samGrind(int extraInfo)
    {
      Game1.playSound("hammer");
      this.getActorByName("Sam").Sprite.currentFrame = 28;
      TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
      temporarySpriteById.currentNumberOfLoops = 0;
      temporarySpriteById.totalNumberOfLoops = 9999;
      temporarySpriteById.motion.Y = 0.0f;
      temporarySpriteById.motion.X = 2f;
      temporarySpriteById.acceleration = new Vector2(0.0f, 0.0f);
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.interval = 99999f;
      temporarySpriteById.timer = 0.0f;
      temporarySpriteById.xStopCoordinate = 1664;
      temporarySpriteById.yStopCoordinate = -1;
      temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.samDropOff);
    }

    private void samDropOff(int extraInfo)
    {
      NPC actorByName = this.getActorByName("Sam");
      actorByName.Sprite.currentFrame = 31;
      TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
      temporarySpriteById.currentNumberOfLoops = 9999;
      temporarySpriteById.totalNumberOfLoops = 0;
      temporarySpriteById.motion.Y = 0.0f;
      temporarySpriteById.motion.X = 2f;
      temporarySpriteById.acceleration = new Vector2(0.0f, 0.4f);
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.interval = 99999f;
      temporarySpriteById.yStopCoordinate = 5760;
      temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.samGround);
      temporarySpriteById.endFunction = (TemporaryAnimatedSprite.endBehavior) null;
      actorByName.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(29, 100),
        new FarmerSprite.AnimationFrame(30, 100),
        new FarmerSprite.AnimationFrame(31, 100),
        new FarmerSprite.AnimationFrame(32, 100)
      });
      actorByName.Sprite.loop = false;
    }

    private void samGround(int extraInfo)
    {
      TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(92473);
      Game1.playSound("thudStep");
      temporarySpriteById.attachedCharacter = (Character) null;
      temporarySpriteById.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) null;
      temporarySpriteById.totalNumberOfLoops = -1;
      temporarySpriteById.interval = 0.0f;
      temporarySpriteById.destroyable = true;
      ++this.CurrentCommand;
    }

    private void catchFootball(int extraInfo)
    {
      TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(56232);
      Game1.playSound("fishSlap");
      temporarySpriteById.motion = new Vector2(2f, -8f);
      temporarySpriteById.rotationChange = 0.1308997f;
      temporarySpriteById.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.footballLand);
      temporarySpriteById.yStopCoordinate = 1088;
      this.farmer.jump();
    }

    private void footballLand(int extraInfo)
    {
      TemporaryAnimatedSprite temporarySpriteById = Game1.currentLocation.getTemporarySpriteByID(56232);
      Game1.playSound("sandyStep");
      temporarySpriteById.motion = new Vector2(0.0f, 0.0f);
      temporarySpriteById.rotationChange = 0.0f;
      temporarySpriteById.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) null;
      temporarySpriteById.animationLength = 1;
      temporarySpriteById.interval = 999999f;
      ++this.CurrentCommand;
    }

    private void parrotSplat(int extraInfo)
    {
      Game1.playSound("drumkit0");
      DelayedAction.playSoundAfterDelay("drumkit5", 100);
      Game1.playSound("slimeHit");
      foreach (TemporaryAnimatedSprite aboveMapSprite in this.aboveMapSprites)
        aboveMapSprite.alpha = 0.0f;
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(174, 168, 4, 11), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.02f, 0.01f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 1.570796f, (float) Math.PI / 64f)
      {
        motion = new Vector2(2f, -2f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(174, 168, 4, 11), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.02f, 0.01f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.7853982f, (float) Math.PI / 64f)
      {
        motion = new Vector2(-2f, -1f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(174, 168, 4, 11), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.02f, 0.01f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 3.141593f, (float) Math.PI / 64f)
      {
        motion = new Vector2(1f, 1f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(174, 168, 4, 11), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.02f, 0.01f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, (float) Math.PI / 64f)
      {
        motion = new Vector2(-2f, -2f),
        acceleration = new Vector2(0.0f, 0.1f)
      });
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(148, 165, 25, 23), 99999f, 1, 99999, new Vector2(1504f, 5568f), false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        id = 666f
      });
      ++this.CurrentCommand;
    }

    public virtual Vector2 OffsetPosition(Vector2 original) => new Vector2(this.OffsetPositionX(original.X), this.OffsetPositionY(original.Y));

    public virtual Vector2 OffsetTile(Vector2 original) => new Vector2((float) this.OffsetTileX((int) original.X), (float) this.OffsetTileY((int) original.Y));

    public virtual float OffsetPositionX(float original) => (double) original < 0.0 || this.ignoreTileOffsets ? original : original + this.eventPositionTileOffset.X * 64f;

    public virtual float OffsetPositionY(float original) => (double) original < 0.0 || this.ignoreTileOffsets ? original : original + this.eventPositionTileOffset.Y * 64f;

    public virtual int OffsetTileX(int original) => original < 0 || this.ignoreTileOffsets ? original : (int) ((double) original + (double) this.eventPositionTileOffset.X);

    public virtual int OffsetTileY(int original) => original < 0 || this.ignoreTileOffsets ? original : (int) ((double) original + (double) this.eventPositionTileOffset.Y);

    private void addSpecificTemporarySprite(string key, GameLocation location, string[] split)
    {
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(key))
      {
        case 37764568:
          if (!(key == "JoshMom"))
            break;
          TemporaryAnimatedSprite temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(416, 1931, 58, 65), 750f, 2, 99999, new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            alpha = 0.6f,
            local = true,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 32f,
            motion = new Vector2(0.0f, -1.25f),
            initialPosition = new Vector2((float) (Game1.viewport.Width / 2), (float) Game1.viewport.Height)
          };
          location.temporarySprites.Add(temporaryAnimatedSprite1);
          for (int index = 0; index < 19; ++index)
            location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(516, 1916, 7, 10), 99999f, 1, 99999, new Vector2(64f, 32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              alphaFade = 0.01f,
              local = true,
              motion = new Vector2(-1f, -1f),
              parentSprite = temporaryAnimatedSprite1,
              delayBeforeAnimationStart = (index + 1) * 1000
            });
          break;
        case 84876044:
          if (!(key == "pennyFieldTrip"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1813, 86, 54), 999999f, 1, 0, new Vector2(68f, 44f) * 64f, false, false, 0.0001f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 109320040:
          if (!(key == "junimoShow"))
            break;
          Texture2D texture2D1 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D1,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(393, 350, 19, 14),
            animationLength = 6,
            sourceRectStartingPos = new Vector2(393f, 350f),
            interval = 90f,
            totalNumberOfLoops = 86,
            position = new Vector2(37f, 14f) * 64f + new Vector2(7f, -2f) * 4f,
            scale = 4f,
            layerDepth = 0.95f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D1,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(393, 364, 19, 14),
            animationLength = 4,
            sourceRectStartingPos = new Vector2(393f, 364f),
            interval = 90f,
            totalNumberOfLoops = 31,
            position = new Vector2(37f, 14f) * 64f + new Vector2(7f, -2f) * 4f,
            scale = 4f,
            layerDepth = 0.97f,
            delayBeforeAnimationStart = 11034
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D1,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(393, 378, 19, 14),
            animationLength = 6,
            sourceRectStartingPos = new Vector2(393f, 378f),
            interval = 90f,
            totalNumberOfLoops = 21,
            position = new Vector2(37f, 14f) * 64f + new Vector2(7f, -2f) * 4f,
            scale = 4f,
            layerDepth = 1f,
            delayBeforeAnimationStart = 22069
          });
          break;
        case 172887865:
          if (!(key == "dropEgg"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(176, 800f, 1, 0, new Vector2(6f, 4f) * 64f + new Vector2(0.0f, 32f), false, false)
          {
            rotationChange = 0.1308997f,
            motion = new Vector2(0.0f, -7f),
            acceleration = new Vector2(0.0f, 0.3f),
            endFunction = new TemporaryAnimatedSprite.endBehavior(this.eggSmashEndFunction),
            layerDepth = 1f
          });
          break;
        case 199811880:
          if (!(key == "shaneCliffProps"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(549, 1891, 19, 12), 99999f, 1, 99999, new Vector2(104f, 96f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          break;
        case 237142214:
          if (!(key == "trashBearMagic"))
            break;
          Utility.addStarsAndSpirals(location, 95, 103, 24, 12, 2000, 10, Microsoft.Xna.Framework.Color.Lime);
          (location as Forest).removeSewerTrash();
          Game1.flashAlpha = 0.75f;
          Game1.screenGlowOnce(Microsoft.Xna.Framework.Color.Lime, false, 0.25f, 1f);
          break;
        case 239359994:
          if (!(key == "leahHoldPainting"))
            break;
          Texture2D texture2D2 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.getTemporarySpriteByID(999).sourceRect.X += 15;
          location.getTemporarySpriteByID(999).sourceRectStartingPos.X += 15f;
          int num1 = Game1.netWorldState.Value.hasWorldStateID("m_painting0") ? 0 : (Game1.netWorldState.Value.hasWorldStateID("m_painting1") ? 1 : 2);
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D2,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(400 + num1 * 25, 394, 25, 23),
            animationLength = 1,
            sourceRectStartingPos = new Vector2((float) (400 + num1 * 25), 394f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(73f, 38f) * 64f + new Vector2(-2f, -16f) * 4f,
            scale = 4f,
            layerDepth = 1f,
            id = 777f
          });
          break;
        case 246031843:
          if (!(key == "heart"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0, this.OffsetPosition(new Vector2((float) Convert.ToInt32(split[2]), (float) Convert.ToInt32(split[3]))) * 64f + new Vector2(-16f, -16f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -0.5f),
            alphaFade = 0.01f
          });
          break;
        case 262838603:
          if (!(key == "EmilyBoomBox"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(586, 1871, 24, 14), 99999f, 1, 99999, new Vector2(15f, 4f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          break;
        case 264360623:
          if (!(key == "haleyRoomDark"))
            break;
          Game1.currentLightSources.Clear();
          Game1.ambientLight = new Microsoft.Xna.Framework.Color(200, 200, 100);
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(743, 999999f, 1, 0, new Vector2(4f, 1f) * 64f, false, false)
          {
            light = true,
            lightcolor = new Microsoft.Xna.Framework.Color(0, (int) byte.MaxValue, (int) byte.MaxValue),
            lightRadius = 2f
          });
          break;
        case 299483063:
          if (!(key == "wed"))
            break;
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          Game1.flashAlpha = 1f;
          for (int index = 0; index < 150; ++index)
          {
            Vector2 position = new Vector2((float) Game1.random.Next(Game1.viewport.Width - 128), (float) Game1.random.Next(Game1.viewport.Height));
            int scale = Game1.random.Next(2, 5);
            this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(424, 1266, 8, 8), 60f + (float) Game1.random.Next(-10, 10), 7, 999999, position, false, false, 0.99f, 0.0f, Microsoft.Xna.Framework.Color.White, (float) scale, 0.0f, 0.0f, 0.0f)
            {
              local = true,
              motion = new Vector2(0.1625f, -0.25f) * (float) scale
            });
          }
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(558, 1425, 20, 26), 400f, 3, 99999, new Vector2(26f, 64f) * 64f, false, false, 0.416f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            pingPong = true
          });
          Game1.changeMusicTrack("wedding", music_context: Game1.MusicContext.Event);
          Game1.musicPlayerVolume = 0.0f;
          break;
        case 321043561:
          if (!(key == "sebastianOnBike"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 1600, 64, 128), 80f, 8, 9999, new Vector2(19f, 27f) * 64f + new Vector2(32f, -16f), false, true, 0.1792f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(405, 1854, 47, 33), 9999f, 1, 999, new Vector2(17f, 27f) * 64f + new Vector2(0.0f, -8f), false, false, 0.1792f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 354301824:
          if (!(key == "sebastianGarage"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1843, 48, 42), 9999f, 1, 999, new Vector2(17f, 23f) * 64f + new Vector2(0.0f, 8f), false, false, 0.1472f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          this.getActorByName("Sebastian").HideShadow = true;
          break;
        case 367105806:
          if (!(key == "leahStopHoldingPainting"))
            break;
          location.getTemporarySpriteByID(999).sourceRect.X -= 15;
          location.getTemporarySpriteByID(999).sourceRectStartingPos.X -= 15f;
          location.removeTemporarySpritesWithIDLocal(777f);
          Game1.playSound("thudStep");
          break;
        case 390240131:
          if (!(key == "shaneCliffs"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(533, 1864, 19, 27), 99999f, 1, 99999, new Vector2(83f, 98f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(552, 1862, 31, 21), 99999f, 1, 99999, new Vector2(83f, 98f) * 64f + new Vector2(-16f, 0.0f), false, false, 0.0001f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(549, 1891, 19, 12), 99999f, 1, 99999, new Vector2(84f, 99f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(549, 1891, 19, 12), 99999f, 1, 99999, new Vector2(82f, 98f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(542, 1893, 4, 6), 99999f, 1, 99999, new Vector2(83f, 99f) * 64f + new Vector2(-8f, 4f) * 4f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 416176087:
          if (!(key == "abbyOuija"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 960, 128, 128), 60f, 4, 0, new Vector2(6f, 9f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f));
          break;
        case 428895204:
          if (!(key == "balloonBirds"))
            break;
          int num2 = 0;
          if (split != null && split.Length > 2)
            num2 = Convert.ToInt32(split[2]);
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(48f, (float) (num2 + 12)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = 1500
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(47f, (float) (num2 + 13)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = 1250
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num2 + 14)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = 1100
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(45f, (float) (num2 + 15)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = 1000
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num2 + 16)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = 1080
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(47f, (float) (num2 + 17)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = 1300
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(48f, (float) (num2 + 18)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            delayBeforeAnimationStart = 1450
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num2 + 15)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-4f, 0.0f),
            delayBeforeAnimationStart = 5450
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(48f, (float) (num2 + 10)) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, 0.0f),
            delayBeforeAnimationStart = 500
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(47f, (float) (num2 + 11)) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, 0.0f),
            delayBeforeAnimationStart = 250
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num2 + 12)) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, 0.0f),
            delayBeforeAnimationStart = 100
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(45f, (float) (num2 + 13)) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, 0.0f)
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(46f, (float) (num2 + 14)) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, 0.0f),
            delayBeforeAnimationStart = 80
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(47f, (float) (num2 + 15)) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, 0.0f),
            delayBeforeAnimationStart = 300
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1894, 24, 22), 100f, 6, 9999, new Vector2(48f, (float) (num2 + 16)) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, 0.0f),
            delayBeforeAnimationStart = 450
          });
          break;
        case 477416675:
          if (!(key == "EmilyBoomBoxStop"))
            break;
          location.getTemporarySpriteByID(999).pulse = false;
          location.getTemporarySpriteByID(999).scale = 4f;
          break;
        case 498391140:
          if (!(key == "swordswipe"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 960, 128, 128), 60f, 4, 0, new Vector2((float) Convert.ToInt32(split[2]), (float) Convert.ToInt32(split[3])) * 64f + new Vector2(0.0f, -32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f));
          break;
        case 519182718:
          if (!(key == "sunroom"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(304, 486, 24, 26),
            sourceRectStartingPos = new Vector2(304f, 486f),
            animationLength = 1,
            totalNumberOfLoops = 997,
            interval = 99999f,
            scale = 4f,
            position = new Vector2(4f, 8f) * 64f + new Vector2(8f, -8f) * 4f,
            layerDepth = 0.0512f,
            id = 996f
          });
          location.addCritter((Critter) new Butterfly(location.getRandomTile()).setStayInbounds(true));
          while (Game1.random.NextDouble() < 0.5)
            location.addCritter((Critter) new Butterfly(location.getRandomTile()).setStayInbounds(true));
          break;
        case 528954373:
          if (!(key == "sauceGood"))
            break;
          Utility.addSprinklesToLocation(location, this.OffsetTileX(64), this.OffsetTileY(16), 3, 1, 800, 200, Microsoft.Xna.Framework.Color.White);
          break;
        case 542343536:
          if (!(key == "getEndSlideshow"))
            break;
          Summit summit = location as Summit;
          string[] collection = summit.getEndSlideshow().Split('/');
          List<string> list1 = ((IEnumerable<string>) this.eventCommands).ToList<string>();
          list1.InsertRange(this.CurrentCommand + 1, (IEnumerable<string>) collection);
          this.eventCommands = list1.ToArray();
          summit.isShowingEndSlideshow = true;
          break;
        case 591728060:
          if (!(key == "krobusBeach"))
            break;
          for (int index = 0; index < 8; ++index)
            location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 150f, 4, 0, new Vector2((float) (84.0 + (index % 2 == 0 ? 0.25 : -0.0500000007450581)), 41f) * 64f, false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.02f, Microsoft.Xna.Framework.Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 500 + index * 1000,
              startSound = "waterSlosh"
            });
          this.underwaterSprites = new List<TemporaryAnimatedSprite>();
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(82f, 52f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2688,
            delayBeforeAnimationStart = 0,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(82f, 52f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 3008,
            delayBeforeAnimationStart = 2000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(88f, 52f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2688,
            delayBeforeAnimationStart = 150,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(88f, 52f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 3008,
            delayBeforeAnimationStart = 2000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(90f, 52f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2816,
            delayBeforeAnimationStart = 300,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(79f, 52f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2816,
            delayBeforeAnimationStart = 1000,
            pingPong = true
          });
          break;
        case 599588800:
          if (!(key == "LeoWillyFishing"))
            break;
          for (int index = 0; index < 20; ++index)
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(0, new Vector2(42.5f, 38f) * 64f + new Vector2((float) Game1.random.Next(64), (float) Game1.random.Next(64)), Microsoft.Xna.Framework.Color.White * 0.7f)
            {
              layerDepth = (float) (1280 + index) / 10000f,
              delayBeforeAnimationStart = index * 150
            });
          break;
        case 655907427:
          if (!(key == "jojaCeremony"))
            break;
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          for (int index = 0; index < 16; ++index)
          {
            Vector2 position = new Vector2((float) Game1.random.Next(Game1.viewport.Width - 128), (float) (Game1.viewport.Height + index * 64));
            this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(534, 1413, 11, 16), 99999f, 1, 99999, position, false, false, 0.99f, 0.0f, Microsoft.Xna.Framework.Color.DeepSkyBlue, 4f, 0.0f, 0.0f, 0.0f)
            {
              local = true,
              motion = new Vector2(0.25f, -1.5f),
              acceleration = new Vector2(0.0f, -1f / 1000f)
            });
            this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(545, 1413, 11, 34), 99999f, 1, 99999, position + new Vector2(0.0f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              local = true,
              motion = new Vector2(0.25f, -1.5f),
              acceleration = new Vector2(0.0f, -1f / 1000f)
            });
          }
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1363, 114, 58), 99999f, 1, 99999, new Vector2(50f, 20f) * 64f, false, false, 0.1472f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(595, 1387, 14, 34), 200f, 3, 99999, new Vector2(48f, 20f) * 64f, false, false, 0.1572f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            pingPong = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(595, 1387, 14, 34), 200f, 3, 99999, new Vector2(49f, 20f) * 64f, false, false, 0.1572f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            pingPong = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(595, 1387, 14, 34), 210f, 3, 99999, new Vector2(62f, 20f) * 64f, false, false, 0.1572f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            pingPong = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(595, 1387, 14, 34), 190f, 3, 99999, new Vector2(60f, 20f) * 64f, false, false, 0.1572f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            pingPong = true
          });
          break;
        case 658062023:
          if (!(key == "BoatParrot"))
            break;
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Microsoft.Xna.Framework.Rectangle(48, 0, 24, 24), 100f, 3, 99999, new Vector2((float) (Game1.viewport.X - 64), 2112f), false, true, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f,
            motion = new Vector2(6f, 1f),
            delayBeforeAnimationStart = 0,
            pingPong = true,
            xStopCoordinate = 1040,
            reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param2 =>
            {
              TemporaryAnimatedSprite temporaryAnimatedSprite3 = this.aboveMapSprites.First<TemporaryAnimatedSprite>();
              if (temporaryAnimatedSprite3 == null)
                return;
              temporaryAnimatedSprite3.motion = new Vector2(0.0f, 2f);
              temporaryAnimatedSprite3.yStopCoordinate = 2336;
              temporaryAnimatedSprite3.reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param3 =>
              {
                TemporaryAnimatedSprite temporaryAnimatedSprite4 = this.aboveMapSprites.First<TemporaryAnimatedSprite>();
                temporaryAnimatedSprite4.animationLength = 1;
                temporaryAnimatedSprite4.pingPong = false;
                temporaryAnimatedSprite4.sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 24, 24);
                temporaryAnimatedSprite4.sourceRectStartingPos = Vector2.Zero;
              });
            })
          });
          break;
        case 704306452:
          if (!(key == "parrotPerchHut"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\parrots", new Microsoft.Xna.Framework.Rectangle(0, 0, 24, 24), new Vector2(7f, 4f) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            animationLength = 1,
            interval = 999999f,
            scale = 4f,
            layerDepth = 1f,
            id = 999f
          });
          break;
        case 750296570:
          if (!(key == "dickBag"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(528, 1435, 16, 16), 99999f, 1, 99999, new Vector2(48f, 7f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 762742231:
          if (!(key == "curtainClose"))
            break;
          location.getTemporarySpriteByID(999).sourceRect.X = 644;
          Game1.playSound("shwip");
          break;
        case 789298023:
          if (!(key == "leahLaptop"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(130, 1849, 19, 19), 9999f, 1, 999, new Vector2(12f, 10f) * 64f + new Vector2(0.0f, 24f), false, false, 0.1856f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 791054291:
          if (!(key == "junimoSpotlight"))
            break;
          this.actors.First<NPC>().drawOnTop = true;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(316, 123, 67, 43),
            sourceRectStartingPos = new Vector2(316f, 123f),
            animationLength = 1,
            interval = 5000f,
            totalNumberOfLoops = 9999,
            scale = 4f,
            position = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 268, 172, yOffset: -20),
            layerDepth = 0.0001f,
            local = true,
            id = 999f
          });
          break;
        case 813315091:
          if (!(key == "frogJump"))
            break;
          TemporaryAnimatedSprite temporarySpriteById1 = location.getTemporarySpriteByID(777);
          temporarySpriteById1.motion = new Vector2(-2f, 0.0f);
          temporarySpriteById1.animationLength = 4;
          temporarySpriteById1.interval = 150f;
          break;
        case 820334071:
          if (!(key == "leahTree"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(744, 999999f, 1, 0, new Vector2(42f, 8f) * 64f, false, false));
          break;
        case 840661922:
          if (!(key == "staticSprite"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite(split[2], new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(split[3]), Convert.ToInt32(split[4]), Convert.ToInt32(split[5]), Convert.ToInt32(split[6])), new Vector2((float) Convert.ToDouble(split[7]), (float) Convert.ToDouble(split[8])) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            animationLength = 1,
            interval = 999999f,
            scale = 4f,
            layerDepth = split.Length > 10 ? (float) Convert.ToDouble(split[10]) : 1f,
            id = split.Length > 9 ? (float) Convert.ToInt32(split[9]) : 999f
          });
          break;
        case 888455659:
          if (!(key == "WizardPromise"))
            break;
          Utility.addSprinklesToLocation(location, 16, 15, 9, 9, 2000, 50, Microsoft.Xna.Framework.Color.White);
          break;
        case 1006429515:
          if (!(key == "sebastianRide"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(405, 1843, 14, 9), 40f, 4, 999, new Vector2(19f, 8f) * 64f + new Vector2(0.0f, 28f), false, false, 0.1792f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, 0.0f)
          });
          break;
        case 1042337593:
          if (!(key == "iceFishingCatch"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160, 368, 16, 32), 500f, 3, 99999, new Vector2(68f, 30f) * 64f, false, false, 0.1984f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160, 368, 16, 32), 510f, 3, 99999, new Vector2(74f, 30f) * 64f, false, false, 0.1984f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160, 368, 16, 32), 490f, 3, 99999, new Vector2(67f, 36f) * 64f, false, false, 148f / 625f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160, 368, 16, 32), 500f, 3, 99999, new Vector2(76f, 35f) * 64f, false, false, 0.2304f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 1057059047:
          if (!(key == "waterShane"))
            break;
          this.drawTool = true;
          this.farmer.TemporaryItem = (Item) new WateringCan();
          this.farmer.CurrentTool.Update(1, 0, this.farmer);
          this.farmer.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[4]
          {
            new FarmerSprite.AnimationFrame(58, 0, false, false),
            new FarmerSprite.AnimationFrame(58, 75, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.showToolSwipeEffect)),
            new FarmerSprite.AnimationFrame(59, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.useTool), true),
            new FarmerSprite.AnimationFrame(45, 500, true, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true)
          });
          break;
        case 1120390711:
          if (!(key == "harveyKitchenFlame"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.mouseCursors,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11),
            animationLength = 4,
            sourceRectStartingPos = new Vector2(276f, 1985f),
            interval = 100f,
            totalNumberOfLoops = 6,
            position = new Vector2(7f, 12f) * 64f + new Vector2(8f, 5f) * 4f,
            scale = 4f,
            layerDepth = 0.09184f
          });
          break;
        case 1139299912:
          if (!(key == "moonlightJellies"))
            break;
          if (this.npcControllers != null)
            this.npcControllers.Clear();
          this.underwaterSprites = new List<TemporaryAnimatedSprite>();
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(26f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2560,
            delayBeforeAnimationStart = 10000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(29f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2560,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(31f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2624,
            delayBeforeAnimationStart = 12000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(20f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 1728,
            delayBeforeAnimationStart = 14000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(17f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 1856,
            delayBeforeAnimationStart = 19500,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(16f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2048,
            delayBeforeAnimationStart = 20300,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(17f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2496,
            delayBeforeAnimationStart = 21500,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(16f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2816,
            delayBeforeAnimationStart = 22400,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(12f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2688,
            delayBeforeAnimationStart = 23200,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(9f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2752,
            delayBeforeAnimationStart = 24000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(18f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 1920,
            delayBeforeAnimationStart = 24600,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(33f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2560,
            delayBeforeAnimationStart = 25600,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(36f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2496,
            delayBeforeAnimationStart = 26900,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16, 16, 16), 200f, 3, 9999, new Vector2(21f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 2500f,
            xPeriodicRange = 10f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2176,
            delayBeforeAnimationStart = 28000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16, 16, 16), 200f, 3, 9999, new Vector2(20f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 2500f,
            xPeriodicRange = 10f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2240,
            delayBeforeAnimationStart = 28500,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16, 16, 16), 200f, 3, 9999, new Vector2(22f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 2500f,
            xPeriodicRange = 10f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2304,
            delayBeforeAnimationStart = 28500,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16, 16, 16), 200f, 3, 9999, new Vector2(33f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 2500f,
            xPeriodicRange = 10f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2752,
            delayBeforeAnimationStart = 29000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16, 16, 16), 200f, 3, 9999, new Vector2(36f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 2500f,
            xPeriodicRange = 10f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2752,
            delayBeforeAnimationStart = 30000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 32, 16, 16), 250f, 3, 9999, new Vector2(28f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-0.5f, -0.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 4000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 2f,
            xStopCoordinate = 1216,
            yStopCoordinate = 2432,
            delayBeforeAnimationStart = 32000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(40f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2560,
            delayBeforeAnimationStart = 10000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(42f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2752,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(43f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2624,
            delayBeforeAnimationStart = 12000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(45f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2496,
            delayBeforeAnimationStart = 14000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(46f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 1856,
            delayBeforeAnimationStart = 19500,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(48f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2240,
            delayBeforeAnimationStart = 20300,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(49f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2560,
            delayBeforeAnimationStart = 21500,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(50f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 1920,
            delayBeforeAnimationStart = 22400,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(51f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2112,
            delayBeforeAnimationStart = 23200,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(52f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2432,
            delayBeforeAnimationStart = 24000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(53f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2240,
            delayBeforeAnimationStart = 24600,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(54f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 1920,
            delayBeforeAnimationStart = 25600,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(55f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2560,
            delayBeforeAnimationStart = 26900,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(4f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 1920,
            delayBeforeAnimationStart = 24000,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(5f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2560,
            delayBeforeAnimationStart = 24600,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(3f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2176,
            delayBeforeAnimationStart = 25600,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(6f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2368,
            delayBeforeAnimationStart = 26900,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(256, 16, 16, 16), 250f, 3, 9999, new Vector2(8f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1f),
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2688,
            delayBeforeAnimationStart = 26900,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16, 16, 16), 200f, 3, 9999, new Vector2(50f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 2500f,
            xPeriodicRange = 10f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2688,
            delayBeforeAnimationStart = 28500,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16, 16, 16), 200f, 3, 9999, new Vector2(51f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 2500f,
            xPeriodicRange = 10f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2752,
            delayBeforeAnimationStart = 28500,
            pingPong = true
          });
          this.underwaterSprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(304, 16, 16, 16), 200f, 3, 9999, new Vector2(52f, 49f) * 64f, false, false, 0.1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -1.5f),
            xPeriodic = true,
            xPeriodicLoopTime = 2500f,
            xPeriodicRange = 10f,
            light = true,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            lightRadius = 1f,
            yStopCoordinate = 2816,
            delayBeforeAnimationStart = 29000,
            pingPong = true
          });
          break;
        case 1171179551:
          if (!(key == "trashBearTown"))
            break;
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(46, 80, 46, 56), new Vector2(43f, 64f) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            animationLength = 1,
            interval = 999999f,
            motion = new Vector2(4f, 0.0f),
            scale = 4f,
            layerDepth = 1f,
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 32f,
            id = 777f,
            xStopCoordinate = 3392,
            reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param =>
            {
              this.aboveMapSprites.First<TemporaryAnimatedSprite>().xStopCoordinate = -1;
              this.aboveMapSprites.First<TemporaryAnimatedSprite>().motion = new Vector2(4f, 0.0f);
              location.ApplyMapOverride("Town-TrashGone", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(57, 68, 17, 5)));
              location.ApplyMapOverride("Town-DogHouse", destination_rect: new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(51, 65, 5, 6)));
              Game1.flashAlpha = 0.75f;
              Game1.screenGlowOnce(Microsoft.Xna.Framework.Color.Lime, false, 0.25f, 1f);
              location.playSound("yoba");
              TemporaryAnimatedSprite temporaryAnimatedSprite5 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(497, 1918, 11, 11), new Vector2(3456f, 4160f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
              {
                yStopCoordinate = 4372,
                motion = new Vector2(-0.5f, -10f),
                acceleration = new Vector2(0.0f, 0.25f),
                scale = 4f,
                alphaFade = 0.0f,
                extraInfoForEndBehavior = -777
              };
              temporaryAnimatedSprite5.reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(temporaryAnimatedSprite5.bounce);
              temporaryAnimatedSprite5.initialPosition.Y = 4372f;
              this.aboveMapSprites.Add(temporaryAnimatedSprite5);
              this.aboveMapSprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.getStarsAndSpirals(location, 54, 69, 6, 5, 1000, 10, Microsoft.Xna.Framework.Color.Lime));
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1936, 12, 20), 80f, 4, 99999, new Vector2(53f, 67f) * 64f + new Vector2(3f, 3f) * 4f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                id = 1f,
                delayBeforeAnimationStart = 3000,
                startSound = "dogWhining"
              });
            })
          });
          break;
        case 1215102451:
          if (!(key == "waterShaneDone"))
            break;
          this.farmer.completelyStopAnimatingOrDoingAction();
          this.farmer.TemporaryItem = (Item) null;
          this.drawTool = false;
          location.removeTemporarySpritesWithID(999);
          break;
        case 1266391031:
          if (!(key == "wedding"))
            break;
          if (this.farmer.IsMale)
          {
            this.oldShirt = (int) (NetFieldBase<int, NetInt>) this.farmer.shirt;
            this.farmer.changeShirt(10);
            this.oldPants = (Microsoft.Xna.Framework.Color) (NetFieldBase<Microsoft.Xna.Framework.Color, NetColor>) this.farmer.pantsColor;
            this.farmer.changePantStyle(0);
            this.farmer.changePants(new Microsoft.Xna.Framework.Color(49, 49, 49));
          }
          foreach (Farmer farmerActor in this.farmerActors)
          {
            if (farmerActor.IsMale)
            {
              farmerActor.changeShirt(10);
              farmerActor.changePants(new Microsoft.Xna.Framework.Color(49, 49, 49));
              farmerActor.changePantStyle(0);
            }
          }
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 1196, 98, 54), 99999f, 1, 99999, new Vector2(25f, 60f) * 64f + new Vector2(0.0f, -64f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 1250, 98, 25), 99999f, 1, 99999, new Vector2(25f, 60f) * 64f + new Vector2(0.0f, 54f) * 4f + new Vector2(0.0f, -64f), false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(24f, 62f) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(32f, 62f) * 64f, false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(24f, 69f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(527, 1249, 12, 25), 99999f, 1, 99999, new Vector2(32f, 69f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 1277310209:
          if (!(key == "krobusraven"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), 100f, 5, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(-2f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 3000f,
            yPeriodicRange = 16f,
            startSound = "shadowpeep"
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(0, 32, 32, 32), 30f, 5, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-2.5f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 2800f,
            yPeriodicRange = 16f,
            delayBeforeAnimationStart = 8000
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(0, 64, 32, 39), 100f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            pingPong = true,
            motion = new Vector2(-3f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 16f,
            delayBeforeAnimationStart = 15000,
            startSound = "fireball"
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1886, 35, 29), 9999f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 2200f,
            yPeriodicRange = 32f,
            local = true,
            delayBeforeAnimationStart = 20000
          });
          for (int index = 0; index < 12; ++index)
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(16, 594, 16, 12), 100f, 2, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f + (float) Game1.random.Next((int) sbyte.MinValue, 128)), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(-2f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = (float) Game1.random.Next(1500, 2000),
              yPeriodicRange = 32f,
              local = true,
              delayBeforeAnimationStart = 24000 + index * 200,
              startSound = index == 0 ? "yoba" : (string) null
            });
          int num3 = 0;
          if (Game1.player.mailReceived.Contains("Capsule_Broken"))
          {
            for (int index = 0; index < 3; ++index)
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(639, 785, 16, 16), 100f, 4, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f + (float) Game1.random.Next((int) sbyte.MinValue, 128)), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                motion = new Vector2(-2f, 0.0f),
                yPeriodic = true,
                yPeriodicLoopTime = (float) Game1.random.Next(1500, 2000),
                yPeriodicRange = 16f,
                local = true,
                delayBeforeAnimationStart = 30000 + index * 500,
                startSound = index == 0 ? "UFO" : (string) null
              });
            num3 += 5000;
          }
          if (Game1.year <= 2)
          {
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(150, 259, 9, 9), 10f, 4, 9999999, new Vector2((float) (Game1.viewport.Width + 4), (float) ((double) Game1.viewport.Height * 0.330000013113022 + 44.0)), false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-2f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 3000f,
              yPeriodicRange = 8f,
              delayBeforeAnimationStart = 30000 + num3
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(2, 129, 120, 27), 1090f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-2f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 3000f,
              yPeriodicRange = 8f,
              startSound = "discoverMineral",
              delayBeforeAnimationStart = 30000 + num3
            });
            num3 += 5000;
          }
          else if (Game1.year <= 3)
          {
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(150, 259, 9, 9), 10f, 4, 9999999, new Vector2((float) (Game1.viewport.Width + 4), (float) ((double) Game1.viewport.Height * 0.330000013113022 + 44.0)), false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-2f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 3000f,
              yPeriodicRange = 8f,
              delayBeforeAnimationStart = 30000 + num3
            });
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(1, 104, 100, 24), 1090f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              motion = new Vector2(-2f, 0.0f),
              yPeriodic = true,
              yPeriodicLoopTime = 3000f,
              yPeriodicRange = 8f,
              startSound = "newArtifact",
              delayBeforeAnimationStart = 30000 + num3
            });
            num3 += 5000;
          }
          if (Game1.MasterPlayer.totalMoneyEarned < 100000000U)
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Characters\\KrobusRaven", new Microsoft.Xna.Framework.Rectangle(125, 108, 34, 50), 1090f, 1, 999999, new Vector2((float) Game1.viewport.Width, (float) Game1.viewport.Height * 0.33f), false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            motion = new Vector2(-2f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 3000f,
            yPeriodicRange = 8f,
            startSound = "discoverMineral",
            delayBeforeAnimationStart = 30000 + num3
          });
          int num4 = num3 + 5000;
          break;
        case 1289885890:
          if (!(key == "movieTheater_setup"))
            break;
          Game1.currentLightSources.Add(new LightSource(7, new Vector2(192f, 64f) + new Vector2(64f, 80f) * 4f, 4f));
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("Maps\\MovieTheaterScreen_TileSheet"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(224, 0, 96, 96),
            sourceRectStartingPos = new Vector2(224f, 0.0f),
            animationLength = 1,
            interval = 5000f,
            totalNumberOfLoops = 9999,
            scale = 4f,
            position = new Vector2(4f, 5f) * 64f,
            layerDepth = 1f,
            id = 999f,
            delayBeforeAnimationStart = 7950
          });
          break;
        case 1382680148:
          if (!(key == "EmilySleeping"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(574, 1892, 11, 11), 1000f, 2, 99999, new Vector2(20f, 3f) * 64f + new Vector2(8f, 32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          break;
        case 1540074856:
          if (!(key == "pamYobaStatue"))
            break;
          location.objects.Remove(new Vector2(26f, 9f));
          location.objects.Add(new Vector2(26f, 9f), new Object(Vector2.Zero, 34));
          Game1.getLocationFromName("Trailer_Big").objects.Remove(new Vector2(26f, 9f));
          Game1.getLocationFromName("Trailer_Big").objects.Add(new Vector2(26f, 9f), new Object(Vector2.Zero, 34));
          break;
        case 1550137454:
          if (!(key == "springOnionDemo"))
            break;
          Texture2D texture2D3 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D3,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(144, 215, 112, 112),
            animationLength = 2,
            sourceRectStartingPos = new Vector2(144f, 215f),
            interval = 200f,
            totalNumberOfLoops = 99999,
            id = 777f,
            position = new Vector2((float) (Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 264), (float) (Game1.graphics.GraphicsDevice.Viewport.Height / 3 - 264)),
            local = true,
            scale = 4f,
            destroyable = false,
            overrideLocationDestroy = true
          });
          break;
        case 1555207748:
          if (!(key == "sauceFire"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.mouseCursors,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11),
            animationLength = 4,
            sourceRectStartingPos = new Vector2(276f, 1985f),
            interval = 100f,
            totalNumberOfLoops = 5,
            position = this.OffsetPosition(new Vector2(64f, 16f) * 64f + new Vector2(3f, -4f) * 4f),
            scale = 4f,
            layerDepth = 1f
          });
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          for (int index = 0; index < 8; ++index)
            this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), this.OffsetPosition(new Vector2(64f, 16f) * 64f) + new Vector2((float) Game1.random.Next(-16, 32), 0.0f), false, 1f / 500f, Microsoft.Xna.Framework.Color.Gray)
            {
              alpha = 0.75f,
              motion = new Vector2(1f, -1f) + new Vector2((float) (Game1.random.Next(100) - 50) / 100f, (float) (Game1.random.Next(100) - 50) / 100f),
              interval = 99999f,
              layerDepth = (float) (0.0384000018239021 + (double) Game1.random.Next(100) / 10000.0),
              scale = 3f,
              scaleChange = 0.01f,
              rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
              delayBeforeAnimationStart = index * 25
            });
          break;
        case 1683745066:
          if (!(key == "LeoLinusCooking"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(240, 128, 16, 16), 9999f, 1, 1, new Vector2(29f, 8.5f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            layerDepth = 1f
          });
          for (int index = 0; index < 10; ++index)
            Utility.addSmokePuff(location, new Vector2(29.5f, 8.6f) * 64f, index * 500);
          break;
        case 1782528198:
          if (!(key == "maruTelescope"))
            break;
          for (int index = 0; index < 9; ++index)
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(256, 1680, 16, 16), 80f, 5, 0, new Vector2((float) Game1.random.Next(1, 28), (float) Game1.random.Next(1, 20)) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 8000 + index * Game1.random.Next(2000),
              motion = new Vector2(4f, 4f)
            });
          break;
        case 1797522365:
          if (!(key == "marcelloLand"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1183, 84, 160), 10000f, 1, 99999, (new Vector2(25f, 19f) + this.eventPositionTileOffset) * 64f + new Vector2(-23f, 0.0f) * 4f, false, false, 2E-05f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, 2f),
            yStopCoordinate = (41 + (int) this.eventPositionTileOffset.Y) * 64 - 640,
            reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.marcelloBalloonLand),
            attachedCharacter = (Character) this.getActorByName("Marcello"),
            id = 1f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(84, 1205, 38, 26), 10000f, 1, 99999, (new Vector2(25f, 19f) + this.eventPositionTileOffset) * 64f + new Vector2(0.0f, 134f) * 4f, false, false, (float) ((41.0 + (double) this.eventPositionTileOffset.Y) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, 2f),
            id = 2f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(24, 1343, 36, 19), 7000f, 1, 99999, (new Vector2(25f, 40f) + this.eventPositionTileOffset) * 64f, false, false, 1E-05f, 0.0f, Microsoft.Xna.Framework.Color.White, 0.0f, 0.0f, 0.0f, 0.0f)
          {
            scaleChange = 0.01f,
            id = 3f
          });
          break;
        case 1834783535:
          if (!(key == "jasGift"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1231, 16, 16), 100f, 6, 1, new Vector2(22f, 16f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f,
            paused = true,
            holdLastFrame = true
          });
          break;
        case 1927366026:
          if (!(key == "elliottBoat"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(461, 1843, 32, 51), 1000f, 2, 9999, new Vector2(15f, 26f) * 64f + new Vector2(-28f, 0.0f), false, false, 0.1664f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 1976122661:
          if (!(key == "curtainOpen"))
            break;
          location.getTemporarySpriteByID(999).sourceRect.X = 672;
          Game1.playSound("shwip");
          break;
        case 1983193104:
          if (!(key == "missingJunimoStars"))
            break;
          location.removeTemporarySpritesWithID(999);
          Texture2D texture2D4 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          for (int index = 0; index < 48; ++index)
            location.TemporarySprites.Add(new TemporaryAnimatedSprite()
            {
              texture = texture2D4,
              sourceRect = new Microsoft.Xna.Framework.Rectangle(477, 306, 28, 28),
              sourceRectStartingPos = new Vector2(477f, 306f),
              animationLength = 1,
              interval = 5000f,
              totalNumberOfLoops = 10,
              scale = (float) Game1.random.Next(1, 5),
              position = Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 84, 84) + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-32, 32)),
              rotationChange = 3.141593f / (float) Game1.random.Next(16, 128),
              motion = new Vector2((float) Game1.random.Next(-30, 40) / 10f, (float) Game1.random.Next(20, 90) * -0.1f),
              acceleration = new Vector2(0.0f, 0.05f),
              local = true,
              layerDepth = (float) index / 100f,
              color = Game1.random.NextDouble() < 0.5 ? Microsoft.Xna.Framework.Color.White : Utility.getRandomRainbowColor()
            });
          break;
        case 1984971571:
          if (!(key == "secretGift"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1231, 16, 16), new Vector2(30f, 70f) * 64f + new Vector2(0.0f, -21f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            animationLength = 1,
            interval = 999999f,
            id = 666f,
            scale = 4f
          });
          break;
        case 1989766461:
          if (!(key == "springOnion"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(1, 129, 16, 16), 200f, 8, 999999, new Vector2(84f, 39f) * 64f, false, false, 0.4736f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          break;
        case 1992359646:
          if (!(key == "shaneThrowCan"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(542, 1893, 4, 6), 99999f, 1, 99999, new Vector2(103f, 95f) * 64f + new Vector2(0.0f, 4f) * 4f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -4f),
            acceleration = new Vector2(0.0f, 0.25f),
            rotationChange = (float) Math.PI / 128f
          });
          Game1.playSound("shwip");
          break;
        case 2082305658:
          if (!(key == "EmilySongBackLights"))
            break;
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          for (int index1 = 0; index1 < 5; ++index1)
          {
            for (int index2 = 0; index2 < Game1.graphics.GraphicsDevice.Viewport.Height + 48; index2 += 48)
              this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(681, 1890, 18, 12), 42241f, 1, 1, new Vector2((float) ((index1 + 1) * Game1.graphics.GraphicsDevice.Viewport.Width / 5 - Game1.graphics.GraphicsDevice.Viewport.Width / 7), (float) (index2 - 24)), false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                xPeriodic = true,
                xPeriodicLoopTime = 1760f,
                xPeriodicRange = (float) (128 + index2 / 12 * 4),
                delayBeforeAnimationStart = index1 * 100 + index2 / 4,
                local = true
              });
          }
          Viewport viewport1;
          for (int index3 = 0; index3 < 27; ++index3)
          {
            int num5 = 0;
            Random random = Game1.random;
            viewport1 = Game1.graphics.GraphicsDevice.Viewport;
            int maxValue = viewport1.Height - 64;
            int y = random.Next(64, maxValue);
            int num6 = Game1.random.Next(800, 2000);
            int num7 = Game1.random.Next(32, 64);
            bool flag = Game1.random.NextDouble() < 0.25;
            int x = Game1.random.Next(-6, -3);
            for (int index4 = 0; index4 < 8; ++index4)
            {
              List<TemporaryAnimatedSprite> aboveMapSprites = this.aboveMapSprites;
              Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(616 + num5 * 10, 1891, 10, 10);
              viewport1 = Game1.graphics.GraphicsDevice.Viewport;
              Vector2 position = new Vector2((float) viewport1.Width, (float) y);
              Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White * (float) (1.0 - (double) index4 * 0.109999999403954);
              aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 42241f, 1, 1, position, false, false, 0.01f, 0.0f, color, 4f, 0.0f, 0.0f, 0.0f)
              {
                yPeriodic = true,
                motion = new Vector2((float) x, 0.0f),
                yPeriodicLoopTime = (float) num6,
                pulse = flag,
                pulseTime = 440f,
                pulseAmount = 1.5f,
                yPeriodicRange = (float) num7,
                delayBeforeAnimationStart = 14000 + index3 * 900 + index4 * 100,
                local = true
              });
            }
          }
          for (int index = 0; index < 15; ++index)
          {
            int num8 = 0;
            Random random = Game1.random;
            viewport1 = Game1.graphics.GraphicsDevice.Viewport;
            int maxValue = viewport1.Width - 128;
            int x = random.Next(maxValue);
            viewport1 = Game1.graphics.GraphicsDevice.Viewport;
            for (int height = viewport1.Height; height >= -64; height -= 48)
            {
              this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(597, 1888, 16, 16), 99999f, 1, 99999, new Vector2((float) x, (float) height), false, false, 1f, 0.02f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, -1.570796f, 0.0f)
              {
                delayBeforeAnimationStart = 27500 + index * 880 + num8 * 25,
                local = true
              });
              ++num8;
            }
          }
          for (int index = 0; index < 120; ++index)
          {
            List<TemporaryAnimatedSprite> aboveMapSprites = this.aboveMapSprites;
            Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(626 + index / 28 * 10, 1891, 10, 10);
            Random random1 = Game1.random;
            viewport1 = Game1.graphics.GraphicsDevice.Viewport;
            int width = viewport1.Width;
            double x = (double) random1.Next(width);
            Random random2 = Game1.random;
            viewport1 = Game1.graphics.GraphicsDevice.Viewport;
            int height = viewport1.Height;
            double y = (double) random2.Next(height);
            Vector2 position = new Vector2((float) x, (float) y);
            Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
            aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", sourceRect, 2000f, 1, 1, position, false, false, 0.01f, 0.0f, white, 0.1f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2(0.0f, -2f),
              alphaFade = 1f / 500f,
              scaleChange = 0.5f,
              scaleChangeChange = -0.0085f,
              delayBeforeAnimationStart = 27500 + index * 110,
              local = true
            });
          }
          break;
        case 2089555403:
          if (!(key == "doneWithSlideShow"))
            break;
          (location as Summit).isShowingEndSlideshow = false;
          break;
        case 2103090580:
          if (!(key == "grandpaNight"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1453, 639, 176), 9999f, 1, 999999, new Vector2(0.0f, 1f) * 64f, false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.Cyan, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.01f,
            alphaFade = -1f / 500f,
            local = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 1453, 639, 176), 9999f, 1, 999999, new Vector2(0.0f, 768f), false, true, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.Blue, 4f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.01f,
            alphaFade = -1f / 500f,
            local = true
          });
          break;
        case 2111807675:
          if (!(key == "woodswalker"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(448, 419, 16, 21),
            sourceRectStartingPos = new Vector2(448f, 419f),
            animationLength = 4,
            totalNumberOfLoops = 7,
            interval = 150f,
            scale = 4f,
            position = new Vector2(4f, 1f) * 64f + new Vector2(5f, 22f) * 4f,
            shakeIntensity = 1f,
            motion = new Vector2(1f, 0.0f),
            xStopCoordinate = 576,
            layerDepth = 1f,
            id = 996f
          });
          break;
        case 2140977878:
          if (!(key == "ClothingTherapy"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(644, 1405, 28, 46), 999999f, 1, 99999, new Vector2(5f, 6f) * 64f + new Vector2(-32f, -144f), false, false, 0.0424f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          break;
        case 2145001354:
          if (!(key == "qiCave"))
            break;
          Texture2D texture2D5 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(415, 216, 96, 89),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(415f, 216f),
            interval = 999999f,
            totalNumberOfLoops = 99999,
            position = new Vector2(2f, 2f) * 64f + new Vector2(112f, 25f) * 4f,
            scale = 4f,
            layerDepth = 1E-07f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(370, 272, 107, 64),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(370f, 216f),
            interval = 999999f,
            totalNumberOfLoops = 99999,
            position = new Vector2(2f, 2f) * 64f + new Vector2(67f, 81f) * 4f,
            scale = 4f,
            layerDepth = 1.1E-07f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.objectSpriteSheet,
            sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 803, 16, 16),
            sourceRectStartingPos = new Vector2((float) Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 803, 16, 16).X, (float) Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 803, 16, 16).Y),
            animationLength = 1,
            interval = 999999f,
            id = 803f,
            totalNumberOfLoops = 99999,
            position = new Vector2(13f, 7f) * 64f + new Vector2(1f, 9f) * 4f,
            scale = 4f,
            layerDepth = 2.1E-06f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16, 30),
            animationLength = 5,
            sourceRectStartingPos = new Vector2(432f, 171f),
            pingPong = true,
            interval = 100f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(8f, 6f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16, 30),
            animationLength = 5,
            sourceRectStartingPos = new Vector2(432f, 171f),
            pingPong = true,
            interval = 90f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(5f, 7f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16, 30),
            animationLength = 5,
            sourceRectStartingPos = new Vector2(432f, 171f),
            pingPong = true,
            interval = 120f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(7f, 10f) * 64f,
            scale = 4f,
            layerDepth = 1f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16, 30),
            animationLength = 5,
            sourceRectStartingPos = new Vector2(432f, 171f),
            pingPong = true,
            interval = 80f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(15f, 7f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16, 30),
            animationLength = 5,
            sourceRectStartingPos = new Vector2(432f, 171f),
            pingPong = true,
            interval = 100f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(12f, 11f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16, 30),
            animationLength = 5,
            sourceRectStartingPos = new Vector2(432f, 171f),
            pingPong = true,
            interval = 105f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(16f, 10f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D5,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(432, 171, 16, 30),
            animationLength = 5,
            sourceRectStartingPos = new Vector2(432f, 171f),
            pingPong = true,
            interval = 85f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(3f, 9f) * 64f,
            scale = 4f
          });
          break;
        case 2197978832:
          if (!(key == "EmilyCamping"))
            break;
          this.showGroundObjects = false;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(644, 1578, 59, 53), 999999f, 1, 99999, new Vector2(26f, 9f) * 64f + new Vector2(-16f, 0.0f), false, false, 0.0788f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(675, 1299, 29, 24), 999999f, 1, 99999, new Vector2(27f, 14f) * 64f, false, false, 1f / 1000f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 99f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), new Vector2(27f, 14f) * 64f + new Vector2(8f, 4f) * 4f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            interval = 50f,
            totalNumberOfLoops = 99999,
            animationLength = 4,
            light = true,
            lightID = 666,
            id = 666f,
            lightRadius = 2f,
            scale = 4f,
            layerDepth = 0.01f
          });
          Game1.currentLightSources.Add(new LightSource(4, new Vector2(27f, 14f) * 64f, 2f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(585, 1846, 26, 22), 999999f, 1, 99999, new Vector2(25f, 12f) * 64f + new Vector2(-32f, 0.0f), false, false, 1f / 1000f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 96f
          });
          AmbientLocationSounds.addSound(new Vector2(27f, 14f), 1);
          break;
        case 2230800038:
          if (!(key == "trashBearUmbrella1"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Microsoft.Xna.Framework.Rectangle(0, 80, 46, 56), new Vector2(102f, 94.5f) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            animationLength = 1,
            interval = 999999f,
            motion = new Vector2(0.0f, -9f),
            acceleration = new Vector2(0.0f, 0.4f),
            scale = 4f,
            layerDepth = 1f,
            id = 777f,
            yStopCoordinate = 6144,
            reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param =>
            {
              location.getTemporarySpriteByID(777).yStopCoordinate = -1;
              location.getTemporarySpriteByID(777).motion = new Vector2(0.0f, (float) param * 0.75f);
              location.getTemporarySpriteByID(777).acceleration = new Vector2(0.04f, -0.19f);
              location.getTemporarySpriteByID(777).accelerationChange = new Vector2(0.0f, 0.0015f);
              location.getTemporarySpriteByID(777).sourceRect.X += 46;
              location.playSound("batFlap");
              location.playSound("tinyWhip");
            })
          });
          break;
        case 2236820530:
          if (!(key == "shaneHospital"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(533, 1864, 19, 10), 99999f, 1, 99999, new Vector2(20f, 3f) * 64f + new Vector2(16f, 12f), false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          break;
        case 2254371385:
          if (!(key == "junimoCage"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(325, 1977, 18, 19), 60f, 3, 999999, new Vector2(10f, 17f) * 64f + new Vector2(0.0f, -4f), false, false, 0.0f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 1f,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            id = 1f,
            shakeIntensity = 0.0f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(379, 1991, 5, 5), 9999f, 1, 999999, new Vector2(10f, 17f) * 64f + new Vector2(0.0f, -4f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 0.5f,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            id = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 24f,
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 24f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(379, 1991, 5, 5), 9999f, 1, 999999, new Vector2(10f, 17f) * 64f + new Vector2(72f, -4f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 0.5f,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            id = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = -24f,
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 24f,
            delayBeforeAnimationStart = 250
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(379, 1991, 5, 5), 9999f, 1, 999999, new Vector2(10f, 17f) * 64f + new Vector2(0.0f, 52f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 0.5f,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            id = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = -24f,
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 24f,
            delayBeforeAnimationStart = 450
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(379, 1991, 5, 5), 9999f, 1, 999999, new Vector2(10f, 17f) * 64f + new Vector2(72f, 52f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 0.5f,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            id = 1f,
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 24f,
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 24f,
            delayBeforeAnimationStart = 650
          });
          break;
        case 2274087936:
          if (!(key == "leahPaintingSetup"))
            break;
          Texture2D texture2D6 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D6,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 393, 15, 28),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(368f, 393f),
            interval = 5000f,
            totalNumberOfLoops = 99999,
            position = new Vector2(72f, 38f) * 64f + new Vector2(3f, -13f) * 4f,
            scale = 4f,
            layerDepth = 0.1f,
            id = 999f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D6,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 393, 15, 28),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(368f, 393f),
            interval = 5000f,
            totalNumberOfLoops = 99999,
            position = new Vector2(74f, 40f) * 64f + new Vector2(3f, -17f) * 4f,
            scale = 4f,
            layerDepth = 0.1f,
            id = 888f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D6,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(369, 424, 11, 15),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(369f, 424f),
            interval = 9999f,
            totalNumberOfLoops = 99999,
            position = new Vector2(75f, 40f) * 64f + new Vector2(-2f, -11f) * 4f,
            scale = 4f,
            layerDepth = 0.01f,
            id = 444f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.mouseCursors,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(96, 1822, 32, 34),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(96f, 1822f),
            interval = 5000f,
            totalNumberOfLoops = 99999,
            position = new Vector2(79f, 36f) * 64f,
            scale = 4f,
            layerDepth = 0.1f
          });
          break;
        case 2279977045:
          if (!(key == "abbyAtLake"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(735, 999999f, 1, 0, new Vector2(48f, 30f) * 64f, false, false)
          {
            light = true,
            lightRadius = 2f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(48f, 30f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2000f,
            yPeriodicLoopTime = 1600f,
            xPeriodicRange = 32f,
            yPeriodicRange = 21f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(48f, 30f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 1000f,
            yPeriodicLoopTime = 1600f,
            xPeriodicRange = 16f,
            yPeriodicRange = 21f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(48f, 30f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2400f,
            yPeriodicLoopTime = 2800f,
            xPeriodicRange = 21f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(48f, 30f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2000f,
            yPeriodicLoopTime = 2400f,
            xPeriodicRange = 16f,
            yPeriodicRange = 16f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(66f, 34f) * 64f + new Vector2(-32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2000f,
            yPeriodicLoopTime = 2600f,
            xPeriodicRange = 21f,
            yPeriodicRange = 48f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(66f, 34f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2000f,
            yPeriodicLoopTime = 2600f,
            xPeriodicRange = 32f,
            yPeriodicRange = 21f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(66f, 34f) * 64f + new Vector2(32f, 32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 4000f,
            yPeriodicLoopTime = 5000f,
            xPeriodicRange = 42f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(66f, 34f) * 64f + new Vector2(0.0f, -32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 4000f,
            yPeriodicLoopTime = 5500f,
            xPeriodicRange = 32f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(69f, 28f) * 64f + new Vector2(-32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2400f,
            yPeriodicLoopTime = 3600f,
            xPeriodicRange = 32f,
            yPeriodicRange = 21f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(69f, 28f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2500f,
            yPeriodicLoopTime = 3600f,
            xPeriodicRange = 42f,
            yPeriodicRange = 51f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(69f, 28f) * 64f + new Vector2(32f, 32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 4500f,
            yPeriodicLoopTime = 3000f,
            xPeriodicRange = 21f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(69f, 28f) * 64f + new Vector2(0.0f, -32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 5000f,
            yPeriodicLoopTime = 4500f,
            xPeriodicRange = 64f,
            yPeriodicRange = 48f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(72f, 33f) * 64f + new Vector2(-32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2000f,
            yPeriodicLoopTime = 3000f,
            xPeriodicRange = 32f,
            yPeriodicRange = 21f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(72f, 33f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 2900f,
            yPeriodicLoopTime = 3200f,
            xPeriodicRange = 21f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(72f, 33f) * 64f + new Vector2(32f, 32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 4200f,
            yPeriodicLoopTime = 3300f,
            xPeriodicRange = 16f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(232, 328, 4, 4), 9999999f, 1, 0, new Vector2(72f, 33f) * 64f + new Vector2(0.0f, -32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 1f, 0.0f, 0.0f, 0.0f)
          {
            lightcolor = Microsoft.Xna.Framework.Color.Orange,
            light = true,
            lightRadius = 0.2f,
            xPeriodic = true,
            yPeriodic = true,
            xPeriodicLoopTime = 5100f,
            yPeriodicLoopTime = 4000f,
            xPeriodicRange = 32f,
            yPeriodicRange = 16f
          });
          break;
        case 2303195829:
          if (!(key == "dickGlitter"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16, 16), 100f, 6, 99999, new Vector2(47f, 8f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16, 16), 100f, 6, 99999, new Vector2(47f, 8f) * 64f + new Vector2(32f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = 200
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16, 16), 100f, 6, 99999, new Vector2(47f, 8f) * 64f + new Vector2(32f, 32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = 300
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16, 16), 100f, 6, 99999, new Vector2(47f, 8f) * 64f + new Vector2(0.0f, 32f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = 100
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1435, 16, 16), 100f, 6, 99999, new Vector2(47f, 8f) * 64f + new Vector2(16f, 16f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 2f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = 400
          });
          break;
        case 2306978800:
          if (!(key == "shakeTent"))
            break;
          location.getTemporarySpriteByID(999).shakeIntensity = 1f;
          break;
        case 2354569370:
          if (!(key == "beachStuff"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1887, 47, 29), 9999f, 1, 999, new Vector2(44f, 21f) * 64f, false, false, 1E-05f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 2358341695:
          if (!(key == "secretGiftOpen"))
            break;
          TemporaryAnimatedSprite temporarySpriteById2 = location.getTemporarySpriteByID(666);
          if (temporarySpriteById2 == null)
            break;
          temporarySpriteById2.animationLength = 6;
          temporarySpriteById2.interval = 100f;
          temporarySpriteById2.totalNumberOfLoops = 1;
          temporarySpriteById2.timer = 0.0f;
          temporarySpriteById2.holdLastFrame = true;
          break;
        case 2366212848:
          if (!(key == "stopShakeTent"))
            break;
          location.getTemporarySpriteByID(999).shakeIntensity = 0.0f;
          break;
        case 2369812317:
          if (!(key == "leahPicnic"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(96, 1808, 32, 48), 9999f, 1, 999, new Vector2(75f, 37f) * 64f, false, false, 0.2496f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          this.actors.Add(new NPC(new AnimatedSprite((ContentManager) this.festivalContent, "Characters\\" + (this.farmer.IsMale ? "LeahExMale" : "LeahExFemale"), 0, 16, 32), new Vector2(-100f, -100f) * 64f, 2, "LeahEx"));
          break;
        case 2407884411:
          if (!(key == "shakeBushStop"))
            break;
          location.getTemporarySpriteByID(777).shakeIntensity = 0.0f;
          break;
        case 2411807913:
          if (!(key == "parrots1"))
            break;
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 165, 24, 22), 100f, 6, 9999, new Vector2((float) Game1.graphics.GraphicsDevice.Viewport.Width, 256f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 32f,
            delayBeforeAnimationStart = 0,
            local = true
          });
          this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 165, 24, 22), 100f, 6, 9999, new Vector2((float) Game1.graphics.GraphicsDevice.Viewport.Width, 192f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 32f,
            delayBeforeAnimationStart = 600,
            local = true
          });
          this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 165, 24, 22), 100f, 6, 9999, new Vector2((float) Game1.graphics.GraphicsDevice.Viewport.Width, 320f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-3f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 32f,
            delayBeforeAnimationStart = 1200,
            local = true
          });
          break;
        case 2463657030:
          if (!(key == "wizardWarp2"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(387, 1965, 16, 31), 9999f, 1, 999999, new Vector2(54f, 34f) * 64f + new Vector2(0.0f, 4f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-1f, 2f),
            acceleration = new Vector2(-0.1f, 0.2f),
            scaleChange = 0.03f,
            alphaFade = 1f / 1000f
          });
          break;
        case 2478151365:
          if (!(key == "samSkate1"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 0, 0, 0), 9999f, 1, 999, new Vector2(12f, 90f) * 64f, false, false, 1E-05f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(4f, 0.0f),
            acceleration = new Vector2(-0.008f, 0.0f),
            xStopCoordinate = 1344,
            reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.samPreOllie),
            attachedCharacter = (Character) this.getActorByName("Sam"),
            id = 92473f
          });
          break;
        case 2610559111:
          if (!(key == "wizardSewerMagic"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), 50f, 4, 20, new Vector2(15f, 13f) * 64f + new Vector2(8f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 1f,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            alphaFade = 0.005f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), 50f, 4, 20, new Vector2(17f, 13f) * 64f + new Vector2(8f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 1f,
            lightcolor = Microsoft.Xna.Framework.Color.Black,
            alphaFade = 0.005f
          });
          break;
        case 2623729097:
          if (!(key == "harveyKitchenSetup"))
            break;
          Texture2D texture2D7 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D7,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(379, 251, 31, 13),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(379f, 251f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(7f, 12f) * 64f + new Vector2(-2f, 6f) * 4f,
            scale = 4f,
            layerDepth = 0.09152f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D7,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(391, 235, 5, 13),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(391f, 235f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(6f, 12f) * 64f + new Vector2(8f, 4f) * 4f,
            scale = 4f,
            layerDepth = 0.09152f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D7,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(399, 229, 11, 21),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(399f, 229f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(4f, 12f) * 64f + new Vector2(8f, -5f) * 4f,
            scale = 4f,
            layerDepth = 0.09152f
          });
          location.temporarySprites.Add(new TemporaryAnimatedSprite(27, new Vector2(6f, 12f) * 64f + new Vector2(0.0f, -5f) * 4f, Microsoft.Xna.Framework.Color.White, 10)
          {
            totalNumberOfLoops = 999,
            layerDepth = 0.09215999f
          });
          location.temporarySprites.Add(new TemporaryAnimatedSprite(27, new Vector2(6f, 12f) * 64f + new Vector2(24f, -5f) * 4f, Microsoft.Xna.Framework.Color.White, 10)
          {
            totalNumberOfLoops = 999,
            flipped = true,
            delayBeforeAnimationStart = 400,
            layerDepth = 0.09215999f
          });
          break;
        case 2633261413:
          if (!(key == "movieBush"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\bushes"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(65, 58, 30, 35),
            sourceRectStartingPos = new Vector2(65f, 58f),
            animationLength = 1,
            totalNumberOfLoops = 999,
            interval = 999f,
            scale = 4f,
            position = new Vector2(4f, 1f) * 64f + new Vector2(33f, 13f) * 4f,
            layerDepth = 0.99f,
            id = 777f
          });
          break;
        case 2646482856:
          if (!(key == "joshFootball"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(405, 1916, 14, 8), 40f, 6, 9999, new Vector2(25f, 16f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            rotation = -0.7853982f,
            rotationChange = (float) Math.PI / 200f,
            motion = new Vector2(6f, -4f),
            acceleration = new Vector2(0.0f, 0.2f),
            xStopCoordinate = 1856,
            reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.catchFootball),
            layerDepth = 1f,
            id = 56232f
          });
          break;
        case 2677303026:
          if (!(key == "BoatParrotLeave"))
            break;
          TemporaryAnimatedSprite temporaryAnimatedSprite6 = this.aboveMapSprites.First<TemporaryAnimatedSprite>();
          temporaryAnimatedSprite6.motion = new Vector2(4f, -6f);
          temporaryAnimatedSprite6.sourceRect.X = 48;
          temporaryAnimatedSprite6.sourceRectStartingPos.X = 48f;
          temporaryAnimatedSprite6.animationLength = 3;
          temporaryAnimatedSprite6.pingPong = true;
          break;
        case 2748349572:
          if (!(key == "abbyManyBats"))
            break;
          for (int index = 0; index < 100; ++index)
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 1664, 16, 16), 80f, 4, 9999, new Vector2(23f, 9f) * 64f, false, false, 1f, 3f / 1000f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              xPeriodic = true,
              xPeriodicLoopTime = (float) Game1.random.Next(1500, 2500),
              xPeriodicRange = (float) Game1.random.Next(64, 192),
              motion = new Vector2((float) Game1.random.Next(-2, 3), (float) Game1.random.Next(-8, -4)),
              delayBeforeAnimationStart = index * 30,
              startSound = index % 10 == 0 || Game1.random.NextDouble() < 0.1 ? "batScreech" : (string) null
            });
          for (int index = 0; index < 100; ++index)
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 1664, 16, 16), 80f, 4, 9999, new Vector2(23f, 9f) * 64f, false, false, 1f, 3f / 1000f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              motion = new Vector2((float) Game1.random.Next(-4, 5), (float) Game1.random.Next(-8, -4)),
              delayBeforeAnimationStart = 10 + index * 30
            });
          break;
        case 2749403796:
          if (!(key == "skateboardFly"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(388, 1875, 16, 6), 9999f, 1, 999, new Vector2(26f, 90f) * 64f, false, false, 1E-05f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            rotationChange = 0.1308997f,
            motion = new Vector2(-8f, -10f),
            acceleration = new Vector2(0.02f, 0.3f),
            yStopCoordinate = 5824,
            xStopCoordinate = 1024,
            layerDepth = 1f
          });
          break;
        case 2805730427:
          if (!(key == "grandpaSpirit"))
            break;
          TemporaryAnimatedSprite temporaryAnimatedSprite7 = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(555, 1956, 18, 35), 9999f, 1, 99999, new Vector2(-1000f, -1010f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            yStopCoordinate = -64128,
            xPeriodic = true,
            xPeriodicLoopTime = 3000f,
            xPeriodicRange = 16f,
            motion = new Vector2(0.0f, 1f),
            overrideLocationDestroy = true,
            id = 77777f
          };
          location.temporarySprites.Add(temporaryAnimatedSprite7);
          for (int index = 0; index < 19; ++index)
            location.temporarySprites.Add(new TemporaryAnimatedSprite(10, new Vector2(32f, 32f), Microsoft.Xna.Framework.Color.White)
            {
              parentSprite = temporaryAnimatedSprite7,
              delayBeforeAnimationStart = (index + 1) * 500,
              overrideLocationDestroy = true,
              scale = 1f,
              alpha = 1f
            });
          break;
        case 2807316816:
          if (!(key == "EmilySign"))
            break;
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          for (int index = 0; index < 10; ++index)
          {
            int num9 = 0;
            Random random = Game1.random;
            Viewport viewport2 = Game1.graphics.GraphicsDevice.Viewport;
            int maxValue = viewport2.Height - 128;
            int y = random.Next(maxValue);
            viewport2 = Game1.graphics.GraphicsDevice.Viewport;
            for (int width = viewport2.Width; width >= -64; width -= 48)
            {
              this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(597, 1888, 16, 16), 99999f, 1, 99999, new Vector2((float) width, (float) y), false, false, 1f, 0.02f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                delayBeforeAnimationStart = index * 600 + num9 * 25,
                startSound = num9 == 0 ? "dwoop" : (string) null,
                local = true
              });
              ++num9;
            }
          }
          break;
        case 2822607183:
          if (!(key == "farmerHoldPainting"))
            break;
          Texture2D texture2D8 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.getTemporarySpriteByID(888).sourceRect.X += 15;
          location.getTemporarySpriteByID(888).sourceRectStartingPos.X += 15f;
          location.removeTemporarySpritesWithID(444);
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D8,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(476, 394, 25, 22),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(476f, 394f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(75f, 40f) * 64f + new Vector2(-4f, -33f) * 4f,
            scale = 4f,
            layerDepth = 1f,
            id = 777f
          });
          break;
        case 2830864500:
          if (!(key == "movieFrame"))
            break;
          int int32_1 = Convert.ToInt32(split[2]);
          int int32_2 = Convert.ToInt32(split[3]);
          int int32_3 = Convert.ToInt32(split[4]);
          int y1 = int32_1 * 128 + int32_2 / 5 * 64;
          int num10 = int32_2 % 5 * 96;
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Movies"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(16 + num10, y1, 90, 61),
            sourceRectStartingPos = new Vector2((float) (16 + num10), (float) y1),
            animationLength = 1,
            totalNumberOfLoops = 1,
            interval = (float) int32_3,
            scale = 4f,
            position = new Vector2(4f, 1f) * 64f + new Vector2(3f, 7f) * 4f,
            shakeIntensity = 0.25f,
            layerDepth = (float) (0.850000023841858 + (double) (int32_1 * int32_2) / 10000.0),
            id = 997f
          });
          break;
        case 2833175679:
          if (!(key == "farmerForestVision"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(393, 1973, 1, 1), 9999f, 1, 999999, new Vector2(0.0f, 0.0f) * 64f, false, false, 0.9f, 0.0f, Microsoft.Xna.Framework.Color.LimeGreen * 0.85f, (float) (Game1.viewport.Width * 2), 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.0f,
            alphaFade = -1f / 500f,
            id = 1f
          });
          Game1.player.mailReceived.Add("canReadJunimoText");
          int x1 = -64;
          int y2 = -64;
          int num11 = 0;
          int num12 = 0;
          while (y2 < Game1.viewport.Height + 128)
          {
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(367 + (num11 % 2 == 0 ? 8 : 0), 1969, 8, 8), 9999f, 1, 999999, new Vector2((float) x1, (float) y2), false, false, 0.99f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f, true)
            {
              alpha = 0.0f,
              alphaFade = -0.0015f,
              xPeriodic = true,
              xPeriodicLoopTime = 4000f,
              xPeriodicRange = 64f,
              yPeriodic = true,
              yPeriodicLoopTime = 5000f,
              yPeriodicRange = 96f,
              rotationChange = (float) ((double) Game1.random.Next(-1, 2) * 3.14159274101257 / 256.0),
              id = 1f,
              delayBeforeAnimationStart = 20 * num11
            });
            x1 += 128;
            if (x1 > Game1.viewport.Width + 64)
            {
              ++num12;
              x1 = num12 % 2 == 0 ? -64 : 64;
              y2 += 128;
            }
            ++num11;
          }
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 2 - 100), (float) (Game1.viewport.Height / 2 - 240)), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.0f,
            alphaFade = -1f / 1000f,
            id = 1f,
            delayBeforeAnimationStart = 6000,
            scaleChange = 0.004f,
            xPeriodic = true,
            xPeriodicLoopTime = 4000f,
            xPeriodicRange = 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 5000f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 4 - 100), (float) (Game1.viewport.Height / 4 - 120)), false, false, 0.99f, 0.0f, Microsoft.Xna.Framework.Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.0f,
            alphaFade = -1f / 1000f,
            id = 1f,
            delayBeforeAnimationStart = 9000,
            scaleChange = 0.004f,
            xPeriodic = true,
            xPeriodicLoopTime = 4000f,
            xPeriodicRange = 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 5000f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width * 3 / 4), (float) (Game1.viewport.Height / 3 - 120)), false, false, 0.98f, 0.0f, Microsoft.Xna.Framework.Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.0f,
            alphaFade = -1f / 1000f,
            id = 1f,
            delayBeforeAnimationStart = 12000,
            scaleChange = 0.004f,
            xPeriodic = true,
            xPeriodicLoopTime = 4000f,
            xPeriodicRange = 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 5000f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 3 - 60), (float) (Game1.viewport.Height * 3 / 4 - 120)), false, false, 0.97f, 0.0f, Microsoft.Xna.Framework.Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.0f,
            alphaFade = -1f / 1000f,
            id = 1f,
            delayBeforeAnimationStart = 15000,
            scaleChange = 0.004f,
            xPeriodic = true,
            xPeriodicLoopTime = 4000f,
            xPeriodicRange = 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 5000f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width * 2 / 3), (float) (Game1.viewport.Height * 2 / 3 - 120)), false, false, 0.96f, 0.0f, Microsoft.Xna.Framework.Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.0f,
            alphaFade = -1f / 1000f,
            id = 1f,
            delayBeforeAnimationStart = 18000,
            scaleChange = 0.004f,
            xPeriodic = true,
            xPeriodicLoopTime = 4000f,
            xPeriodicRange = 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 5000f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width / 8), (float) (Game1.viewport.Height / 5 - 120)), false, false, 0.95f, 0.0f, Microsoft.Xna.Framework.Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.0f,
            alphaFade = -1f / 1000f,
            id = 1f,
            delayBeforeAnimationStart = 19500,
            scaleChange = 0.004f,
            xPeriodic = true,
            xPeriodicLoopTime = 4000f,
            xPeriodicRange = 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 5000f,
            yPeriodicRange = 32f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 895, 51, 101), 9999f, 1, 999999, new Vector2((float) (Game1.viewport.Width * 2 / 3), (float) (Game1.viewport.Height / 5 - 120)), false, false, 0.94f, 0.0f, Microsoft.Xna.Framework.Color.White, 3f, 0.0f, 0.0f, 0.0f, true)
          {
            alpha = 0.0f,
            alphaFade = -1f / 1000f,
            id = 1f,
            delayBeforeAnimationStart = 21000,
            scaleChange = 0.004f,
            xPeriodic = true,
            xPeriodicLoopTime = 4000f,
            xPeriodicRange = 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 5000f,
            yPeriodicRange = 32f
          });
          break;
        case 2858755550:
          if (!(key == "coldstarMiracle"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Movies"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(400, 704, 90, 61),
            sourceRectStartingPos = new Vector2(400f, 704f),
            animationLength = 1,
            totalNumberOfLoops = 1,
            interval = 99999f,
            alpha = 0.01f,
            alphaFade = -0.01f,
            scale = 4f,
            position = new Vector2(4f, 1f) * 64f + new Vector2(3f, 7f) * 4f,
            layerDepth = 0.8535f,
            id = 989f
          });
          break;
        case 2859740261:
          if (!(key == "movieTheater_screen"))
            break;
          int num13 = int.Parse(split[2]);
          int num14 = int.Parse(split[3]);
          bool flag1 = bool.Parse(split[4]);
          int y3 = num13 * 128 + num14 / 5 * 64;
          int num15 = num14 % 5 * 96;
          location.removeTemporarySpritesWithIDLocal(998f);
          if (num14 < 0)
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Movies"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(16 + num15, y3, 90, 61),
            sourceRectStartingPos = new Vector2((float) (16 + num15), (float) y3),
            animationLength = 1,
            totalNumberOfLoops = 9999,
            interval = 5000f,
            scale = 4f,
            position = new Vector2(4f, 1f) * 64f + new Vector2(3f, 7f) * 4f,
            shakeIntensity = flag1 ? 1f : 0.0f,
            layerDepth = (float) (0.100000001490116 + (double) (num13 * num14) / 10000.0),
            id = 998f
          });
          break;
        case 2862938824:
          if (!(key == "witchFlyby"))
            break;
          Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1886, 35, 29), 9999f, 1, 999999, new Vector2((float) Game1.graphics.GraphicsDevice.Viewport.Width, 192f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-4f, 0.0f),
            acceleration = new Vector2(-0.025f, 0.0f),
            yPeriodic = true,
            yPeriodicLoopTime = 2000f,
            yPeriodicRange = 64f,
            local = true
          });
          break;
        case 2871007719:
          if (!(key == "BoatParrotSquawk"))
            break;
          TemporaryAnimatedSprite temporaryAnimatedSprite8 = this.aboveMapSprites.First<TemporaryAnimatedSprite>();
          temporaryAnimatedSprite8.sourceRect.X = 24;
          temporaryAnimatedSprite8.sourceRectStartingPos.X = 24f;
          Game1.playSound("parrot_squawk");
          break;
        case 2881735469:
          if (!(key == "maruTrapdoor"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 1632, 16, 32), 150f, 4, 0, new Vector2(1f, 5f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(688, 1632, 16, 32), 99999f, 1, 0, new Vector2(1f, 5f) * 64f, false, false, 0.99f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            delayBeforeAnimationStart = 500
          });
          break;
        case 2956180802:
          if (!(key == "junimoCageGone"))
            break;
          location.removeTemporarySpritesWithID(1);
          break;
        case 3003121694:
          if (!(key == "morrisFlying"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(105, 1318, 13, 31), 9999f, 1, 99999, new Vector2(32f, 13f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(4f, -8f),
            rotationChange = 0.1963495f,
            shakeIntensity = 1f
          });
          break;
        case 3017460216:
          if (!(key == "shanePassedOut"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(533, 1864, 19, 27), 99999f, 1, 99999, new Vector2(25f, 7f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(552, 1862, 31, 21), 99999f, 1, 99999, new Vector2(25f, 7f) * 64f + new Vector2(-16f, 0.0f), false, false, 0.0001f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 3023463258:
          if (!(key == "arcaneBook"))
            break;
          for (int index = 0; index < 16; ++index)
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(536, 1945, 8, 8), new Vector2(128f, 792f) + new Vector2((float) Game1.random.Next(32), (float) (Game1.random.Next(32) - index * 4)), false, 0.0f, Microsoft.Xna.Framework.Color.White)
            {
              interval = 50f,
              totalNumberOfLoops = 99999,
              animationLength = 7,
              layerDepth = 1f,
              scale = 4f,
              alphaFade = 0.008f,
              motion = new Vector2(0.0f, -0.5f)
            });
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(325, 1977, 18, 18), new Vector2(160f, 800f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            interval = 25f,
            totalNumberOfLoops = 99999,
            animationLength = 3,
            layerDepth = 1f,
            scale = 1f,
            scaleChange = 1f,
            scaleChangeChange = -0.05f,
            alpha = 0.65f,
            alphaFade = 0.005f,
            motion = new Vector2(-8f, -8f),
            acceleration = new Vector2(0.4f, 0.4f)
          });
          for (int index = 0; index < 16; ++index)
            this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(2f, 12f) * 64f + new Vector2((float) Game1.random.Next(-32, 64), 0.0f), false, 1f / 500f, Microsoft.Xna.Framework.Color.Gray)
            {
              alpha = 0.75f,
              motion = new Vector2(1f, -1f) + new Vector2((float) (Game1.random.Next(100) - 50) / 100f, (float) (Game1.random.Next(100) - 50) / 100f),
              interval = 99999f,
              layerDepth = (float) (0.0384000018239021 + (double) Game1.random.Next(100) / 10000.0),
              scale = 3f,
              scaleChange = 0.01f,
              rotationChange = (float) ((double) Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
              delayBeforeAnimationStart = index * 25
            });
          location.setMapTileIndex(2, 12, 2143, "Front", 1);
          break;
        case 3029534325:
          if (!(key == "springOnionRemove"))
            break;
          location.removeTemporarySpritesWithID(777);
          break;
        case 3041866819:
          if (!(key == "evilRabbit"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(264, 209, 19, 16),
            sourceRectStartingPos = new Vector2(264f, 209f),
            animationLength = 1,
            totalNumberOfLoops = 999,
            interval = 999f,
            scale = 4f,
            position = new Vector2(4f, 1f) * 64f + new Vector2(38f, 23f) * 4f,
            layerDepth = 1f,
            motion = new Vector2(-2f, -2f),
            acceleration = new Vector2(0.0f, 0.1f),
            yStopCoordinate = 204,
            xStopCoordinate = 316,
            flipped = true,
            id = 778f
          });
          break;
        case 3065465346:
          if (!(key == "abbyvideoscreen"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(167, 1714, 19, 14), 100f, 3, 9999, new Vector2(2f, 3f) * 64f + new Vector2(7f, 12f) * 4f, false, false, 0.0002f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 3091229030:
          if (!(key == "candleBoat"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(240, 112, 16, 32), 1000f, 2, 99999, new Vector2(22f, 36f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 1f,
            light = true,
            lightRadius = 2f,
            lightcolor = Microsoft.Xna.Framework.Color.Black
          });
          break;
        case 3098278158:
          if (!(key == "leahShow"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(144, 688, 16, 32), 9999f, 1, 999, new Vector2(29f, 59f) * 64f - new Vector2(0.0f, 16f), false, false, 0.3775f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(112, 656, 16, 64), 9999f, 1, 999, new Vector2(29f, 56f) * 64f, false, false, 0.3776f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(144, 688, 16, 32), 9999f, 1, 999, new Vector2(33f, 59f) * 64f - new Vector2(0.0f, 16f), false, false, 0.3775f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(128, 688, 16, 32), 9999f, 1, 999, new Vector2(33f, 58f) * 64f, false, false, 0.3776f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(160, 656, 32, 64), 9999f, 1, 999, new Vector2(29f, 60f) * 64f, false, false, 0.4032f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(144, 688, 16, 32), 9999f, 1, 999, new Vector2(34f, 63f) * 64f, false, false, 0.4031f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(113, 592, 16, 64), 100f, 4, 99999, new Vector2(34f, 60f) * 64f, false, false, 0.4032f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          this.actors.Add(new NPC(new AnimatedSprite((ContentManager) this.festivalContent, "Characters\\" + (this.farmer.IsMale ? "LeahExMale" : "LeahExFemale"), 0, 16, 32), new Vector2(46f, 57f) * 64f, 2, "LeahEx"));
          break;
        case 3181672047:
          if (!(key == "luauShorts"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(336, 512, 16, 16), 9999f, 1, 99999, new Vector2(35f, 10f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(-2f, -8f),
            acceleration = new Vector2(0.0f, 0.25f),
            yStopCoordinate = 704,
            xStopCoordinate = 2112
          });
          break;
        case 3203398193:
          if (!(key == "pennyMess"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(739, 999999f, 1, 0, new Vector2(10f, 5f) * 64f, false, false));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(740, 999999f, 1, 0, new Vector2(15f, 5f) * 64f, false, false));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(741, 999999f, 1, 0, new Vector2(16f, 6f) * 64f, false, false));
          break;
        case 3216534692:
          if (!(key == "abbyOneBat"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(640, 1664, 16, 16), 80f, 4, 9999, new Vector2(23f, 9f) * 64f, false, false, 1f, 3f / 1000f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            xPeriodic = true,
            xPeriodicLoopTime = 2000f,
            xPeriodicRange = 128f,
            motion = new Vector2(0.0f, -8f)
          });
          break;
        case 3264340021:
          if (!(key == "willyCrabExperiment"))
            break;
          Texture2D texture2D9 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
            pingPong = true,
            interval = 250f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(2f, 4f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, 146f),
            pingPong = true,
            interval = 200f,
            totalNumberOfLoops = 99999,
            id = 1f,
            initialPosition = new Vector2(2f, 6f) * 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 8000f,
            yPeriodicRange = 32f,
            position = new Vector2(2f, 6f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
            pingPong = true,
            interval = 100f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(1f, 5.75f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
            pingPong = true,
            interval = 100f,
            totalNumberOfLoops = 99999,
            id = 11f,
            position = new Vector2(5f, 3f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
            pingPong = true,
            interval = 140f,
            totalNumberOfLoops = 99999,
            id = 22f,
            position = new Vector2(4f, 6f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
            pingPong = true,
            interval = 140f,
            totalNumberOfLoops = 99999,
            id = 22f,
            position = new Vector2(8.5f, 5f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, 146f),
            pingPong = true,
            interval = 170f,
            totalNumberOfLoops = 99999,
            id = 222f,
            position = new Vector2(6f, 3.25f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, 146f),
            pingPong = true,
            interval = 190f,
            totalNumberOfLoops = 99999,
            id = 222f,
            position = new Vector2(6f, 6f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, 146f),
            pingPong = true,
            interval = 150f,
            totalNumberOfLoops = 99999,
            id = 222f,
            position = new Vector2(7f, 4f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, 146f),
            pingPong = true,
            interval = 200f,
            totalNumberOfLoops = 99999,
            id = 2f,
            position = new Vector2(4f, 7f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, (int) sbyte.MaxValue, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, (float) sbyte.MaxValue),
            pingPong = true,
            interval = 180f,
            totalNumberOfLoops = 99999,
            id = 3f,
            position = new Vector2(8f, 6f) * 64f,
            yPeriodic = true,
            yPeriodicLoopTime = 10000f,
            yPeriodicRange = 32f,
            initialPosition = new Vector2(8f, 6f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, 146f),
            pingPong = true,
            interval = 220f,
            totalNumberOfLoops = 99999,
            id = 33f,
            position = new Vector2(9f, 6f) * 64f,
            scale = 4f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D9,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(259, 146, 18, 18),
            animationLength = 3,
            sourceRectStartingPos = new Vector2(259f, 146f),
            pingPong = true,
            interval = 150f,
            totalNumberOfLoops = 99999,
            id = 33f,
            position = new Vector2(10f, 5f) * 64f,
            scale = 4f
          });
          break;
        case 3310754603:
          if (!(key == "sebastianFrog"))
            break;
          Texture2D texture2D10 = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D10,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 224, 16, 16),
            animationLength = 4,
            sourceRectStartingPos = new Vector2(0.0f, 224f),
            interval = 120f,
            totalNumberOfLoops = 9999,
            position = new Vector2(45f, 36f) * 64f,
            scale = 4f,
            layerDepth = 0.00064f,
            motion = new Vector2(2f, 0.0f),
            xStopCoordinate = 3136,
            id = 777f,
            reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param1 =>
            {
              ++this.CurrentCommand;
              location.removeTemporarySpritesWithID(777);
            })
          });
          break;
        case 3320176351:
          if (!(key == "springOnionPeel"))
            break;
          TemporaryAnimatedSprite temporarySpriteById3 = location.getTemporarySpriteByID(777);
          temporarySpriteById3.sourceRectStartingPos = new Vector2(144f, 327f);
          temporarySpriteById3.sourceRect = new Microsoft.Xna.Framework.Rectangle(144, 327, 112, 112);
          break;
        case 3339171029:
          if (!(key == "joshSteak"))
            break;
          location.temporarySprites.Clear();
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1936, 12, 20), 80f, 4, 99999, new Vector2(53f, 67f) * 64f + new Vector2(3f, 3f) * 4f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 1f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(497, 1918, 11, 11), 999f, 1, 9999, new Vector2(50f, 68f) * 64f + new Vector2(32f, -8f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 3339287078:
          if (!(key == "parrotSlide"))
            break;
          location.getTemporarySpriteByID(666).yStopCoordinate = 5632;
          location.getTemporarySpriteByID(666).motion.X = 0.0f;
          location.getTemporarySpriteByID(666).motion.Y = 1f;
          break;
        case 3344494886:
          if (!(key == "parrotHutSquawk"))
            break;
          (location as IslandHut).parrotUpgradePerches[0].timeUntilSqwawk = 1f;
          break;
        case 3366575180:
          if (!(key == "trashBearPrelude"))
            break;
          Utility.addStarsAndSpirals(location, 95, 106, 23, 4, 10000, 275, Microsoft.Xna.Framework.Color.Lime);
          break;
        case 3385570989:
          if (!(key == "joshDinner"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(649, 9999f, 1, 9999, new Vector2(6f, 4f) * 64f + new Vector2(8f, 32f), false, false)
          {
            layerDepth = 0.0256f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(664, 9999f, 1, 9999, new Vector2(8f, 4f) * 64f + new Vector2(-8f, 32f), false, false)
          {
            layerDepth = 0.0256f
          });
          break;
        case 3441767033:
          if (!(key == "joshDog"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1916, 12, 20), 500f, 6, 9999, new Vector2(53f, 67f) * 64f + new Vector2(3f, 3f) * 4f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 1f
          });
          break;
        case 3447055920:
          if (!(key == "abbyGraveyard"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(736, 999999f, 1, 0, new Vector2(48f, 86f) * 64f, false, false));
          break;
        case 3448286637:
          if (!(key == "ccCelebration"))
            break;
          this.aboveMapSprites = new List<TemporaryAnimatedSprite>();
          for (int index = 0; index < 32; ++index)
          {
            Vector2 position = new Vector2((float) Game1.random.Next(Game1.viewport.Width - 128), (float) (Game1.viewport.Height + index * 64));
            this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(534, 1413, 11, 16), 99999f, 1, 99999, position, false, false, 1f, 0.0f, Utility.getRandomRainbowColor(), 4f, 0.0f, 0.0f, 0.0f)
            {
              local = true,
              motion = new Vector2(0.25f, -1.5f),
              acceleration = new Vector2(0.0f, -1f / 1000f)
            });
            this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(545, 1413, 11, 34), 99999f, 1, 99999, position + new Vector2(0.0f, 0.0f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
            {
              local = true,
              motion = new Vector2(0.25f, -1.5f),
              acceleration = new Vector2(0.0f, -1f / 1000f)
            });
          }
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(558, 1425, 20, 26), 400f, 3, 99999, new Vector2(53f, 21f) * 64f, false, false, 0.5f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            pingPong = true
          });
          break;
        case 3457429727:
          if (!(key == "robot"))
            break;
          TemporaryAnimatedSprite temporaryAnimatedSprite9 = new TemporaryAnimatedSprite((string) (NetFieldBase<string, NetString>) this.getActorByName("robot").Sprite.textureName, new Microsoft.Xna.Framework.Rectangle(35, 42, 35, 42), 50f, 1, 9999, new Vector2(13f, 27f) * 64f - new Vector2(0.0f, 32f), false, false, 0.98f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            acceleration = new Vector2(0.0f, -0.01f),
            accelerationChange = new Vector2(0.0f, -0.0001f)
          };
          location.temporarySprites.Add(temporaryAnimatedSprite9);
          for (int index = 0; index < 420; ++index)
            location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(Game1.random.Next(4) * 64, 320, 64, 64), new Vector2((float) Game1.random.Next(96), 136f), false, 0.01f, Microsoft.Xna.Framework.Color.White * 0.75f)
            {
              layerDepth = 1f,
              delayBeforeAnimationStart = index * 10,
              animationLength = 1,
              currentNumberOfLoops = 0,
              interval = 9999f,
              motion = new Vector2((float) (Game1.random.Next(-100, 100) / (index + 20)), (float) (0.25 + (double) index / 100.0)),
              parentSprite = temporaryAnimatedSprite9
            });
          break;
        case 3499642143:
          if (!(key == "BoatParrotSquawkStop"))
            break;
          TemporaryAnimatedSprite temporaryAnimatedSprite10 = this.aboveMapSprites.First<TemporaryAnimatedSprite>();
          temporaryAnimatedSprite10.sourceRect.X = 0;
          temporaryAnimatedSprite10.sourceRectStartingPos.X = 0.0f;
          break;
        case 3513988944:
          if (!(key == "junimoCageGone2"))
            break;
          location.removeTemporarySpritesWithID(1);
          Game1.viewportFreeze = true;
          Game1.viewport.X = -1000;
          Game1.viewport.Y = -1000;
          break;
        case 3520623308:
          if (!(key == "gridballGameTV"))
            break;
          Texture2D texture2D11 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D11,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 336, 19, 14),
            animationLength = 7,
            sourceRectStartingPos = new Vector2(368f, 336f),
            interval = 5000f,
            totalNumberOfLoops = 99999,
            position = new Vector2(34f, 3f) * 64f + new Vector2(7f, 13f) * 4f,
            scale = 4f,
            layerDepth = 1f
          });
          break;
        case 3566538117:
          if (!(key == "georgeLeekGift"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(288, 1231, 16, 16), 100f, 6, 1, new Vector2(17f, 19f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f,
            paused = false,
            holdLastFrame = true
          });
          break;
        case 3580969289:
          if (!(key == "EmilyBoomBoxStart"))
            break;
          location.getTemporarySpriteByID(999).pulse = true;
          location.getTemporarySpriteByID(999).pulseTime = 420f;
          break;
        case 3585277845:
          if (!(key == "sebastianFrogHouse"))
            break;
          Point spouseRoomCorner = (location as FarmHouse).GetSpouseRoomCorner();
          ++spouseRoomCorner.X;
          spouseRoomCorner.Y += 6;
          Vector2 vector2_1 = Utility.PointToVector2(spouseRoomCorner);
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.mouseCursors,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(641, 1534, 48, 37),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(641f, 1534f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = vector2_1 * 64f + new Vector2(0.0f, -5f) * 4f,
            scale = 4f,
            layerDepth = (float) (((double) vector2_1.Y + 2.0 + 0.100000001490116) * 64.0 / 10000.0)
          });
          Texture2D texture2D12 = Game1.temporaryContent.Load<Texture2D>("TileSheets\\critters");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D12,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 224, 16, 16),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(0.0f, 224f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = vector2_1 * 64f + new Vector2(25f, 2f) * 4f,
            scale = 4f,
            flipped = true,
            layerDepth = (float) (((double) vector2_1.Y + 2.0 + 0.109999999403954) * 64.0 / 10000.0),
            id = 777f
          });
          break;
        case 3587796569:
          if (!(key == "haleyCakeWalk"))
            break;
          Texture2D texture2D13 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D13,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 400, 144, 112),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(0.0f, 400f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(26f, 65f) * 64f,
            scale = 4f,
            layerDepth = 0.00064f
          });
          break;
        case 3732153936:
          if (!(key == "wizardWarp"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(387, 1965, 16, 31), 9999f, 1, 999999, new Vector2(8f, 16f) * 64f + new Vector2(0.0f, 4f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(2f, -2f),
            acceleration = new Vector2(0.1f, 0.0f),
            scaleChange = -0.02f,
            alphaFade = 1f / 1000f
          });
          break;
        case 3744618201:
          if (!(key == "linusLights"))
            break;
          Game1.currentLightSources.Add(new LightSource(2, new Vector2(55f, 62f) * 64f, 2f));
          Game1.currentLightSources.Add(new LightSource(2, new Vector2(60f, 62f) * 64f, 2f));
          Game1.currentLightSources.Add(new LightSource(2, new Vector2(57f, 60f) * 64f, 3f));
          Game1.currentLightSources.Add(new LightSource(2, new Vector2(57f, 60f) * 64f, 2f));
          Game1.currentLightSources.Add(new LightSource(2, new Vector2(47f, 70f) * 64f, 2f));
          Game1.currentLightSources.Add(new LightSource(2, new Vector2(52f, 63f) * 64f, 2f));
          break;
        case 3760313400:
          if (!(key == "linusMoney"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-1002f, -1000f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            startSound = "money",
            delayBeforeAnimationStart = 10,
            overrideLocationDestroy = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-1003f, -1002f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            startSound = "money",
            delayBeforeAnimationStart = 100,
            overrideLocationDestroy = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-999f, -1000f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            startSound = "money",
            delayBeforeAnimationStart = 200,
            overrideLocationDestroy = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-1004f, -1001f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            startSound = "money",
            delayBeforeAnimationStart = 300,
            overrideLocationDestroy = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-1001f, -998f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            startSound = "money",
            delayBeforeAnimationStart = 400,
            overrideLocationDestroy = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-998f, -999f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            startSound = "money",
            delayBeforeAnimationStart = 500,
            overrideLocationDestroy = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-998f, -1002f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            startSound = "money",
            delayBeforeAnimationStart = 600,
            overrideLocationDestroy = true
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(397, 1941, 19, 20), 9999f, 1, 99999, new Vector2(-997f, -1001f) * 64f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            startSound = "money",
            delayBeforeAnimationStart = 700,
            overrideLocationDestroy = true
          });
          break;
        case 3764783545:
          if (!(key == "shaneSaloonCola"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.mouseCursors,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(552, 1862, 31, 21),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(552f, 1862f),
            interval = 999999f,
            totalNumberOfLoops = 99999,
            position = new Vector2(32f, 17f) * 64f + new Vector2(10f, 3f) * 4f,
            scale = 4f,
            layerDepth = 1E-07f
          });
          break;
        case 3775217944:
          if (!(key == "maruElectrocution"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(432, 1664, 16, 32), 40f, 1, 20, new Vector2(7f, 5f) * 64f - new Vector2(-4f, 8f), true, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 3800171382:
          if (!(key == "grandpaThumbsUp"))
            break;
          TemporaryAnimatedSprite temporarySpriteById4 = location.getTemporarySpriteByID(77777);
          temporarySpriteById4.texture = Game1.mouseCursors2;
          temporarySpriteById4.sourceRect = new Microsoft.Xna.Framework.Rectangle(186, 265, 22, 34);
          temporarySpriteById4.sourceRectStartingPos = new Vector2(186f, 265f);
          temporarySpriteById4.yPeriodic = true;
          temporarySpriteById4.yPeriodicLoopTime = 1000f;
          temporarySpriteById4.yPeriodicRange = 16f;
          temporarySpriteById4.xPeriodicLoopTime = 2500f;
          temporarySpriteById4.xPeriodicRange = 16f;
          temporarySpriteById4.initialPosition = temporarySpriteById4.position;
          break;
        case 3872194338:
          if (!(key == "alexDiningDog"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1936, 12, 20), 80f, 4, 99999, new Vector2(7f, 2f) * 64f + new Vector2(2f, -8f) * 4f, false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 1f
          });
          break;
        case 3876362429:
          if (!(key == "islandFishSplash"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(336, 544, 16, 16), 100000f, 1, 1, new Vector2(81f, 92f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 9999f,
            motion = new Vector2(-2f, -8f),
            acceleration = new Vector2(0.0f, 0.2f),
            flipped = true,
            rotationChange = -0.02f,
            yStopCoordinate = 5952,
            layerDepth = 0.99f,
            reachedStopCoordinate = (TemporaryAnimatedSprite.endBehavior) (param1 =>
            {
              location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(48, 16, 16, 16), 100f, 5, 1, location.getTemporarySpriteByID(9999).position, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
              {
                layerDepth = 1f
              });
              location.removeTemporarySpritesWithID(9999);
              Game1.playSound("waterSlosh");
            })
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", new Microsoft.Xna.Framework.Rectangle(48, 16, 16, 16), 100f, 5, 1, new Vector2(81f, 92f) * 64f, false, false, 0.01f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            layerDepth = 1f
          });
          break;
        case 3902794431:
          if (!(key == "golemDie"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite(46, new Vector2(40f, 11f) * 64f, Microsoft.Xna.Framework.Color.DarkGray, 10));
          Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, new Vector2(40f, 11f) * 64f, Microsoft.Xna.Framework.Color.LimeGreen, 10), location, 2);
          Texture2D texture2D14 = Game1.temporaryContent.Load<Texture2D>("Characters\\Monsters\\Wilderness Golem");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D14,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 24),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(0.0f, 0.0f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = new Vector2(40f, 11f) * 64f + new Vector2(2f, -8f) * 4f,
            scale = 4f,
            layerDepth = 0.01f,
            rotation = 1.570796f,
            motion = new Vector2(0.0f, 4f),
            yStopCoordinate = 832
          });
          break;
        case 3920807573:
          if (!(key == "linusCampfire"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(276, 1985, 12, 11), 50f, 4, 99999, new Vector2(29f, 9f) * 64f + new Vector2(8f, 0.0f), false, false, 0.0576f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            light = true,
            lightRadius = 3f,
            lightcolor = Microsoft.Xna.Framework.Color.Black
          });
          break;
        case 3925840365:
          if (!(key == "pennyCook"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), new Vector2(10f, 6f) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 1f,
            animationLength = 6,
            interval = 75f,
            motion = new Vector2(0.0f, -0.5f)
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), new Vector2(10f, 6f) * 64f + new Vector2(16f, 0.0f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 0.1f,
            animationLength = 6,
            interval = 75f,
            motion = new Vector2(0.0f, -0.5f),
            delayBeforeAnimationStart = 500
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), new Vector2(10f, 6f) * 64f + new Vector2(-16f, 0.0f), false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 1f,
            animationLength = 6,
            interval = 75f,
            motion = new Vector2(0.0f, -0.5f),
            delayBeforeAnimationStart = 750
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), new Vector2(10f, 6f) * 64f, false, 0.0f, Microsoft.Xna.Framework.Color.White)
          {
            layerDepth = 0.1f,
            animationLength = 6,
            interval = 75f,
            motion = new Vector2(0.0f, -0.5f),
            delayBeforeAnimationStart = 1000
          });
          break;
        case 3992366603:
          if (!(key == "parrotSplat"))
            break;
          this.aboveMapSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(0, 165, 24, 22), 100f, 6, 9999, new Vector2((float) (Game1.viewport.X + Game1.graphics.GraphicsDevice.Viewport.Width), (float) (Game1.viewport.Y + 64)), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            id = 999f,
            motion = new Vector2(-2f, 4f),
            acceleration = new Vector2(-0.1f, 0.0f),
            delayBeforeAnimationStart = 0,
            yStopCoordinate = 5568,
            xStopCoordinate = 1504,
            reachedStopCoordinate = new TemporaryAnimatedSprite.endBehavior(this.parrotSplat)
          });
          break;
        case 4027113491:
          if (!(key == "candleBoatMove"))
            break;
          location.getTemporarySpriteByID(1).motion = new Vector2(0.0f, 2f);
          break;
        case 4028797203:
          if (!(key == "abbyOuijaCandles"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(737, 999999f, 1, 0, new Vector2(5f, 9f) * 64f, false, false)
          {
            light = true,
            lightRadius = 1f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(737, 999999f, 1, 0, new Vector2(7f, 8f) * 64f, false, false)
          {
            light = true,
            lightRadius = 1f
          });
          break;
        case 4089652164:
          if (!(key == "removeSprite") || split == null || ((IEnumerable<string>) split).Count<string>() <= 2)
            break;
          location.removeTemporarySpritesWithID(Convert.ToInt32(split[2]));
          break;
        case 4108304842:
          if (!(key == "samTV"))
            break;
          Texture2D texture2D15 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D15,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(368, 350, 25, 29),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(368f, 350f),
            interval = 5000f,
            totalNumberOfLoops = 99999,
            position = new Vector2(37f, 14f) * 64f + new Vector2(4f, -12f) * 4f,
            scale = 4f,
            layerDepth = 0.9f
          });
          break;
        case 4119912933:
          if (!(key == "umbrella"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(324, 1843, 27, 23), 80f, 3, 9999, new Vector2(12f, 39f) * 64f + new Vector2(-20f, -104f), false, false, 1f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
        case 4140496595:
          if (!(key == "shakeBush"))
            break;
          location.getTemporarySpriteByID(777).shakeIntensity = 1f;
          break;
        case 4188226542:
          if (!(key == "parrotGone"))
            break;
          location.removeTemporarySpritesWithID(666);
          break;
        case 4199220531:
          if (!(key == "jasGiftOpen"))
            break;
          location.getTemporarySpriteByID(999).paused = false;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(537, 1850, 11, 10), 1500f, 1, 1, new Vector2(23f, 16f) * 64f + new Vector2(16f, -48f), false, false, 0.99f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -0.25f),
            delayBeforeAnimationStart = 500,
            yStopCoordinate = 928
          });
          location.temporarySprites.AddRange((IEnumerable<TemporaryAnimatedSprite>) Utility.sparkleWithinArea(new Microsoft.Xna.Framework.Rectangle(1440, 992, 128, 64), 5, Microsoft.Xna.Framework.Color.White, 300));
          break;
        case 4215720008:
          if (!(key == "maruBeaker"))
            break;
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(738, 1380f, 1, 0, new Vector2(9f, 14f) * 64f + new Vector2(0.0f, 32f), false, false)
          {
            rotationChange = 0.1308997f,
            motion = new Vector2(0.0f, -7f),
            acceleration = new Vector2(0.0f, 0.2f),
            endFunction = new TemporaryAnimatedSprite.endBehavior(this.beakerSmashEndFunction),
            layerDepth = 1f
          });
          break;
        case 4277499152:
          if (!(key == "harveyDinnerSet"))
            break;
          Vector2 vector2_2 = new Vector2(5f, 16f);
          if (location is DecoratableLocation)
          {
            foreach (Furniture furniture in (location as DecoratableLocation).furniture)
            {
              if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type == 14 && location.getTileIndexAt((int) furniture.tileLocation.X, (int) furniture.tileLocation.Y + 1, "Buildings") == -1 && location.getTileIndexAt((int) furniture.tileLocation.X + 1, (int) furniture.tileLocation.Y + 1, "Buildings") == -1 && location.getTileIndexAt((int) furniture.tileLocation.X + 2, (int) furniture.tileLocation.Y + 1, "Buildings") == -1 && location.getTileIndexAt((int) furniture.tileLocation.X - 1, (int) furniture.tileLocation.Y + 1, "Buildings") == -1)
              {
                vector2_2 = new Vector2((float) (int) furniture.TileLocation.X, (float) ((int) furniture.TileLocation.Y + 1));
                furniture.isOn.Value = true;
                furniture.setFireplace(location, false);
                break;
              }
            }
          }
          location.TemporarySprites.Clear();
          this.getActorByName("Harvey").setTilePosition((int) vector2_2.X + 2, (int) vector2_2.Y);
          this.getActorByName("Harvey").Position = new Vector2(this.getActorByName("Harvey").Position.X - 32f, this.getActorByName("Harvey").Position.Y);
          this.farmer.Position = new Vector2((float) ((double) vector2_2.X * 64.0 - 32.0), (float) ((double) vector2_2.Y * 64.0 + 32.0));
          Object objectAtTile1 = location.getObjectAtTile((int) vector2_2.X, (int) vector2_2.Y);
          if (objectAtTile1 != null)
            objectAtTile1.isTemporarilyInvisible = true;
          Object objectAtTile2 = location.getObjectAtTile((int) vector2_2.X + 1, (int) vector2_2.Y);
          if (objectAtTile2 != null)
            objectAtTile2.isTemporarilyInvisible = true;
          Object objectAtTile3 = location.getObjectAtTile((int) vector2_2.X - 1, (int) vector2_2.Y);
          if (objectAtTile3 != null)
            objectAtTile3.isTemporarilyInvisible = true;
          Object objectAtTile4 = location.getObjectAtTile((int) vector2_2.X + 2, (int) vector2_2.Y);
          if (objectAtTile4 != null)
            objectAtTile4.isTemporarilyInvisible = true;
          Texture2D texture2D16 = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
          location.TemporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = texture2D16,
            sourceRect = new Microsoft.Xna.Framework.Rectangle(385, 423, 48, 32),
            animationLength = 1,
            sourceRectStartingPos = new Vector2(385f, 423f),
            interval = 5000f,
            totalNumberOfLoops = 9999,
            position = vector2_2 * 64f + new Vector2(-8f, -16f) * 4f,
            scale = 4f,
            layerDepth = (float) (((double) vector2_2.Y + 0.200000002980232) * 64.0 / 10000.0),
            light = true,
            lightRadius = 4f,
            lightcolor = Microsoft.Xna.Framework.Color.Black
          });
          List<string> list2 = ((IEnumerable<string>) this.eventCommands).ToList<string>();
          list2.Insert(this.CurrentCommand + 1, "viewport " + ((int) vector2_2.X).ToString() + " " + ((int) vector2_2.Y).ToString() + " true");
          this.eventCommands = list2.ToArray();
          break;
        case 4288562364:
          if (!(key == "WillyWad"))
            break;
          location.temporarySprites.Add(new TemporaryAnimatedSprite()
          {
            texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cursors2"),
            sourceRect = new Microsoft.Xna.Framework.Rectangle(192, 61, 32, 32),
            sourceRectStartingPos = new Vector2(192f, 61f),
            animationLength = 2,
            totalNumberOfLoops = 99999,
            interval = 400f,
            scale = 4f,
            position = new Vector2(50f, 23f) * 64f,
            layerDepth = 0.1536f,
            id = 996f
          });
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(51, new Vector2(3328f, 1728f), Microsoft.Xna.Framework.Color.White, 10, animationInterval: 80f, numberOfLoops: 999999));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(51, new Vector2(3264f, 1792f), Microsoft.Xna.Framework.Color.White, 10, animationInterval: 70f, numberOfLoops: 999999));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite(51, new Vector2(3392f, 1792f), Microsoft.Xna.Framework.Color.White, 10, animationInterval: 85f, numberOfLoops: 999999));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160, 368, 16, 32), 500f, 3, 99999, new Vector2(53f, 24f) * 64f, false, false, 0.1984f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          location.TemporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(160, 368, 16, 32), 510f, 3, 99999, new Vector2(54f, 23f) * 64f, false, false, 0.1984f, 0.0f, Microsoft.Xna.Framework.Color.White, 4f, 0.0f, 0.0f, 0.0f));
          break;
      }
    }

    private Microsoft.Xna.Framework.Rectangle skipBounds()
    {
      int num = 4;
      int width = 22 * num;
      Microsoft.Xna.Framework.Rectangle bounds = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.Width - width - 8, Game1.viewport.Height - 64, width, 15 * num);
      Utility.makeSafe(ref bounds);
      return bounds;
    }

    public void receiveMouseClick(int x, int y)
    {
      if (this.skipped || !this.skippable || !this.skipBounds().Contains(x, y))
        return;
      this.skipped = true;
      this.skipEvent();
      Game1.freezeControls = false;
    }

    public void skipEvent()
    {
      if (this.playerControlSequence)
        this.EndPlayerControlSequence();
      Game1.playSound("drumkit6");
      this.actorPositionsAfterMove.Clear();
      foreach (NPC actor in this.actors)
      {
        bool ignoreStopAnimation = actor.Sprite.ignoreStopAnimation;
        actor.Sprite.ignoreStopAnimation = true;
        actor.Halt();
        actor.Sprite.ignoreStopAnimation = ignoreStopAnimation;
        this.resetDialogueIfNecessary(actor);
      }
      this.farmer.Halt();
      this.farmer.ignoreCollisions = false;
      Game1.exitActiveMenu();
      Game1.dialogueUp = false;
      Game1.dialogueTyping = false;
      Game1.pauseTime = 0.0f;
      switch (this.id)
      {
        case -157039427:
          this.endBehaviors(new string[2]
          {
            "end",
            "islandDepart"
          }, Game1.currentLocation);
          break;
        case -888999:
          Object object1 = new Object(864, 1);
          object1.specialItem = true;
          Object object2 = object1;
          object2.questItem.Value = true;
          Game1.player.addItemByMenuIfNecessary((Item) object2);
          Game1.player.addQuest(130);
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case -666777:
          List<Item> objList = new List<Item>();
          Game1.player.team.RequestLimitedNutDrops("Birdie", (GameLocation) null, 0, 0, 5, 5);
          if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotBirdieReward"))
            Game1.addMailForTomorrow("gotBirdieReward", true, true);
          if (!Game1.player.craftingRecipes.ContainsKey("Fairy Dust"))
            Game1.player.craftingRecipes.Add("Fairy Dust", 0);
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case -78765:
          this.endBehaviors(new string[2]
          {
            "end",
            "tunnelDepart"
          }, Game1.currentLocation);
          break;
        case 19:
          if (!Game1.player.cookingRecipes.ContainsKey("Cookies"))
            Game1.player.cookingRecipes.Add("Cookies", 0);
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 26:
          if (!Game1.player.craftingRecipes.ContainsKey("Wild Bait"))
            Game1.player.craftingRecipes.Add("Wild Bait", 0);
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 112:
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          Game1.player.mailReceived.Add("canReadJunimoText");
          break;
        case 60367:
          this.endBehaviors(new string[2]
          {
            "end",
            "beginGame"
          }, Game1.currentLocation);
          break;
        case 100162:
          if (Game1.player.hasItemWithNameThatContains("Rusty Sword") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new MeleeWeapon(0));
          Game1.player.Position = new Vector2(-9999f, -99999f);
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 191393:
          if (Game1.player.hasItemWithNameThatContains("Stardew Hero Trophy") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new Object(Vector2.Zero, 116));
          this.endBehaviors(new string[4]
          {
            "end",
            "position",
            "52",
            "20"
          }, Game1.currentLocation);
          break;
        case 404798:
          if (Game1.player.hasItemWithNameThatContains("Copper Pan") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new Pan());
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 558292:
          Game1.player.eventsSeen.Remove(2146991);
          this.endBehaviors(new string[2]{ "end", "bed" }, Game1.currentLocation);
          break;
        case 611173:
          if (!Game1.player.activeDialogueEvents.ContainsKey("pamHouseUpgrade") && !Game1.player.activeDialogueEvents.ContainsKey("pamHouseUpgradeAnonymous"))
            Game1.player.activeDialogueEvents.Add("pamHouseUpgrade", 4);
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 690006:
          if (Game1.player.hasItemWithNameThatContains("Green Slime Egg") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new Object(680, 1));
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 739330:
          if (Game1.player.hasItemWithNameThatContains("Bamboo Pole") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new FishingRod());
          this.endBehaviors(new string[4]
          {
            "end",
            "position",
            "43",
            "36"
          }, Game1.currentLocation);
          break;
        case 900553:
          if (!Game1.player.craftingRecipes.ContainsKey("Garden Pot"))
            Game1.player.craftingRecipes.Add("Garden Pot", 0);
          if (Game1.player.hasItemWithNameThatContains("Garden Pot") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new Object(Vector2.Zero, 62));
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 980558:
          if (!Game1.player.craftingRecipes.ContainsKey("Mini-Jukebox"))
            Game1.player.craftingRecipes.Add("Mini-Jukebox", 0);
          if (Game1.player.hasItemWithNameThatContains("Mini-Jukebox") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new Object(Vector2.Zero, 209));
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 992553:
          if (!Game1.player.craftingRecipes.ContainsKey("Furnace"))
            Game1.player.craftingRecipes.Add("Furnace", 0);
          if (!Game1.player.hasQuest(11))
            Game1.player.addQuest(11);
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 2123343:
          this.endBehaviors(new string[2]{ "end", "newDay" }, Game1.currentLocation);
          break;
        case 3091462:
          if (Game1.player.hasItemWithNameThatContains("My First Painting") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new Furniture(1802, Vector2.Zero));
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 3918602:
          if (Game1.player.hasItemWithNameThatContains("Sam's Boombox") == null)
            Game1.player.addItemByMenuIfNecessary((Item) new Furniture(1309, Vector2.Zero));
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
        case 6497428:
          this.endBehaviors(new string[2]{ "end", "Leo" }, Game1.currentLocation);
          break;
        default:
          this.endBehaviors(new string[1]{ "end" }, Game1.currentLocation);
          break;
      }
    }

    public void receiveKeyPress(Keys k)
    {
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void receiveActionPress(int xTile, int yTile)
    {
      if (xTile != this.playerControlTargetTile.X || yTile != this.playerControlTargetTile.Y)
        return;
      string controlSequenceId = this.playerControlSequenceID;
      if (!(controlSequenceId == "haleyBeach"))
      {
        if (!(controlSequenceId == "haleyBeach2"))
          return;
        this.EndPlayerControlSequence();
        ++this.CurrentCommand;
      }
      else
      {
        this.props.Clear();
        Game1.playSound("coin");
        this.playerControlTargetTile = new Point(35, 11);
        this.playerControlSequenceID = "haleyBeach2";
      }
    }

    public void startSecretSantaEvent()
    {
      this.playerControlSequence = false;
      this.playerControlSequenceID = (string) null;
      this.eventCommands = this.festivalData["secretSanta"].Split('/');
      this.doingSecretSanta = true;
      this.setUpSecretSantaCommands();
      this.currentCommand = 0;
    }

    public void festivalUpdate(GameTime time)
    {
      Game1.player.team.festivalScoreStatus.UpdateState(Game1.player.festivalScore.ToString() ?? "");
      if (this.festivalTimer > 0)
      {
        this.oldTime = this.festivalTimer;
        this.festivalTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.playerControlSequenceID == "iceFishing")
        {
          if (!Game1.player.UsingTool)
            Game1.player.forceCanMove();
          if (this.oldTime % 500 < this.festivalTimer % 500)
          {
            NPC actorByName1 = this.getActorByName("Pam");
            actorByName1.Sprite.sourceRect.Offset(actorByName1.Sprite.SourceRect.Width, 0);
            if (actorByName1.Sprite.sourceRect.X >= actorByName1.Sprite.Texture.Width)
              actorByName1.Sprite.sourceRect.Offset(-actorByName1.Sprite.Texture.Width, 0);
            NPC actorByName2 = this.getActorByName("Elliott");
            actorByName2.Sprite.sourceRect.Offset(actorByName2.Sprite.SourceRect.Width, 0);
            if (actorByName2.Sprite.sourceRect.X >= actorByName2.Sprite.Texture.Width)
              actorByName2.Sprite.sourceRect.Offset(-actorByName2.Sprite.Texture.Width, 0);
            NPC actorByName3 = this.getActorByName("Willy");
            actorByName3.Sprite.sourceRect.Offset(actorByName3.Sprite.SourceRect.Width, 0);
            if (actorByName3.Sprite.sourceRect.X >= actorByName3.Sprite.Texture.Width)
              actorByName3.Sprite.sourceRect.Offset(-actorByName3.Sprite.Texture.Width, 0);
          }
          if (this.oldTime % 29900 < this.festivalTimer % 29900)
          {
            this.getActorByName("Willy").shake(500);
            Game1.playSound("dwop");
            this.temporaryLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(112, 432, 16, 16), this.getActorByName("Willy").Position + new Vector2(0.0f, -96f), false, 0.015f, Microsoft.Xna.Framework.Color.White)
            {
              layerDepth = 1f,
              scale = 4f,
              interval = 9999f,
              motion = new Vector2(0.0f, -1f)
            });
          }
          if (this.oldTime % 45900 < this.festivalTimer % 45900)
          {
            this.getActorByName("Pam").shake(500);
            Game1.playSound("dwop");
            this.temporaryLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(112, 432, 16, 16), this.getActorByName("Pam").Position + new Vector2(0.0f, -96f), false, 0.015f, Microsoft.Xna.Framework.Color.White)
            {
              layerDepth = 1f,
              scale = 4f,
              interval = 9999f,
              motion = new Vector2(0.0f, -1f)
            });
          }
          if (this.oldTime % 59900 < this.festivalTimer % 59900)
          {
            this.getActorByName("Elliott").shake(500);
            Game1.playSound("dwop");
            this.temporaryLocation.temporarySprites.Add(new TemporaryAnimatedSprite("Maps\\Festivals", new Microsoft.Xna.Framework.Rectangle(112, 432, 16, 16), this.getActorByName("Elliott").Position + new Vector2(0.0f, -96f), false, 0.015f, Microsoft.Xna.Framework.Color.White)
            {
              layerDepth = 1f,
              scale = 4f,
              interval = 9999f,
              motion = new Vector2(0.0f, -1f)
            });
          }
        }
        if (this.festivalTimer <= 0)
        {
          Game1.player.Halt();
          string controlSequenceId = this.playerControlSequenceID;
          if (!(controlSequenceId == "eggHunt"))
          {
            if (controlSequenceId == "iceFishing")
            {
              this.EndPlayerControlSequence();
              this.eventCommands = this.festivalData["afterIceFishing"].Split('/');
              this.currentCommand = 0;
              if (Game1.activeClickableMenu != null)
                Game1.activeClickableMenu.emergencyShutDown();
              Game1.activeClickableMenu = (IClickableMenu) null;
              if (Game1.player.UsingTool && Game1.player.CurrentTool != null && Game1.player.CurrentTool is FishingRod)
                (Game1.player.CurrentTool as FishingRod).doneFishing(Game1.player);
              Game1.screenOverlayTempSprites.Clear();
              Game1.player.forceCanMove();
            }
          }
          else
          {
            this.EndPlayerControlSequence();
            this.eventCommands = this.festivalData["afterEggHunt"].Split('/');
            this.currentCommand = 0;
          }
        }
      }
      if (this.startSecretSantaAfterDialogue && !Game1.dialogueUp)
      {
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.startSecretSantaEvent), 0.01f);
        this.startSecretSantaAfterDialogue = false;
      }
      Game1.player.festivalScore = Math.Min(Game1.player.festivalScore, 9999);
      if (!this.waitingForMenuClose || Game1.activeClickableMenu != null)
        return;
      int num = this.festivalData["file"] == "fall16" ? 1 : 0;
      this.waitingForMenuClose = false;
    }

    private void setUpSecretSantaCommands()
    {
      int tileX;
      int tileY;
      try
      {
        tileX = this.getActorByName(this.mySecretSanta.Name).getTileX();
        tileY = this.getActorByName(this.mySecretSanta.Name).getTileY();
      }
      catch (Exception ex)
      {
        this.mySecretSanta = this.getActorByName("Lewis");
        tileX = this.getActorByName(this.mySecretSanta.Name).getTileX();
        tileY = this.getActorByName(this.mySecretSanta.Name).getTileY();
      }
      string newValue1 = "";
      string newValue2 = "";
      switch (this.mySecretSanta.Age)
      {
        case 0:
        case 1:
          switch (this.mySecretSanta.Manners)
          {
            case 0:
            case 1:
              newValue1 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.mySecretSanta.gender, "Strings\\StringsFromCSFiles:Event.cs.1499");
              newValue2 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.mySecretSanta.gender, "Strings\\StringsFromCSFiles:Event.cs.1500");
              break;
            case 2:
              newValue1 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.mySecretSanta.gender, "Strings\\StringsFromCSFiles:Event.cs.1501");
              newValue2 = this.mySecretSanta.Name.Equals("George") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1503") : Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.mySecretSanta.gender, "Strings\\StringsFromCSFiles:Event.cs.1504");
              break;
          }
          break;
        case 2:
          newValue1 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.mySecretSanta.gender, "Strings\\StringsFromCSFiles:Event.cs.1497");
          newValue2 = Game1.LoadStringByGender((int) (NetFieldBase<int, NetInt>) this.mySecretSanta.gender, "Strings\\StringsFromCSFiles:Event.cs.1498");
          break;
      }
      for (int index = 0; index < this.eventCommands.Length; ++index)
      {
        this.eventCommands[index] = this.eventCommands[index].Replace("secretSanta", this.mySecretSanta.Name);
        this.eventCommands[index] = this.eventCommands[index].Replace("warpX", tileX.ToString() ?? "");
        this.eventCommands[index] = this.eventCommands[index].Replace("warpY", tileY.ToString() ?? "");
        this.eventCommands[index] = this.eventCommands[index].Replace("dialogue1", newValue1);
        this.eventCommands[index] = this.eventCommands[index].Replace("dialogue2", newValue2);
      }
    }

    public void drawFarmers(SpriteBatch b)
    {
      foreach (Character farmerActor in this.farmerActors)
        farmerActor.draw(b);
    }

    public virtual bool ShouldHideCharacter(NPC n) => n is Child && this.doingSecretSanta;

    public void draw(SpriteBatch b)
    {
      if (this.currentCustomEventScript != null)
      {
        this.currentCustomEventScript.draw(b);
      }
      else
      {
        foreach (NPC actor in this.actors)
        {
          if (!this.ShouldHideCharacter(actor))
          {
            actor.Name.Equals("Marcello");
            if (actor.ySourceRectOffset == 0)
              actor.draw(b);
            else
              actor.draw(b, actor.ySourceRectOffset, 1f);
          }
        }
        foreach (Object prop in this.props)
          prop.drawAsProp(b);
        foreach (Prop festivalProp in this.festivalProps)
          festivalProp.draw(b);
        if (this.isFestival && this.festivalData["file"] == "fall16")
        {
          Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(37f, 56f) * 64f);
          local.X += 4f;
          int num = (int) local.X + 168;
          local.Y += 8f;
          for (int index = 0; index < Game1.player.team.grangeDisplay.Count; ++index)
          {
            if (Game1.player.team.grangeDisplay[index] != null)
            {
              local.Y += 42f;
              local.X += 4f;
              b.Draw(Game1.shadowTexture, local, new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
              local.Y -= 42f;
              local.X -= 4f;
              Game1.player.team.grangeDisplay[index].drawInMenu(b, local, 1f, 1f, (float) ((double) index / 1000.0 + 1.0 / 1000.0), StackDrawType.Hide);
            }
            local.X += 60f;
            if ((double) local.X >= (double) num)
            {
              local.X = (float) (num - 168);
              local.Y += 64f;
            }
          }
        }
        if (!this.drawTool)
          return;
        Game1.drawTool(this.farmer);
      }
    }

    public void drawUnderWater(SpriteBatch b)
    {
      if (this.underwaterSprites == null)
        return;
      foreach (TemporaryAnimatedSprite underwaterSprite in this.underwaterSprites)
        underwaterSprite.draw(b);
    }

    public void drawAfterMap(SpriteBatch b)
    {
      if (this.aboveMapSprites != null)
      {
        foreach (TemporaryAnimatedSprite aboveMapSprite in this.aboveMapSprites)
          aboveMapSprite.draw(b);
      }
      if (!Game1.game1.takingMapScreenshot && this.playerControlSequenceID != null)
      {
        string controlSequenceId = this.playerControlSequenceID;
        if (!(controlSequenceId == "eggHunt"))
        {
          if (!(controlSequenceId == "fair"))
          {
            if (controlSequenceId == "iceFishing")
            {
              b.End();
              b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(16, 16, 128 + (Game1.player.festivalScore > 999 ? 16 : 0), 128), Microsoft.Xna.Framework.Color.Black * 0.75f);
              b.Draw(this.festivalTexture, new Vector2(32f, 16f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(112, 432, 16, 16)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
              Game1.drawWithBorder(Game1.player.festivalScore.ToString() ?? "", Microsoft.Xna.Framework.Color.Black, Microsoft.Xna.Framework.Color.White, new Vector2(96f, (float) (21 + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? 8 : (LocalizedContentManager.CurrentLanguageLatin ? 16 : 8)))), 0.0f, 1f, 1f, false);
              Game1.drawWithBorder(Utility.getMinutesSecondsStringFromMilliseconds(this.festivalTimer), Microsoft.Xna.Framework.Color.Black, Microsoft.Xna.Framework.Color.White, new Vector2(32f, 93f), 0.0f, 1f, 1f, false);
              b.End();
              b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
              if (Game1.IsMultiplayer)
                Game1.player.team.festivalScoreStatus.Draw(b, new Vector2(32f, (float) (Game1.viewport.Height - 32)), draw_layer: 0.99f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
            }
          }
          else
          {
            b.End();
            Game1.PushUIMode();
            b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(16, 16, 128 + (Game1.player.festivalScore > 999 ? 16 : 0), 64), Microsoft.Xna.Framework.Color.Black * 0.75f);
            b.Draw(Game1.mouseCursors, new Vector2(32f, 32f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(338, 400, 8, 8)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            Game1.drawWithBorder(Game1.player.festivalScore.ToString() ?? "", Microsoft.Xna.Framework.Color.Black, Microsoft.Xna.Framework.Color.White, new Vector2(72f, (float) (21 + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? 8 : (LocalizedContentManager.CurrentLanguageLatin ? 16 : 8)))), 0.0f, 1f, 1f, false);
            if (Game1.activeClickableMenu == null)
              Game1.dayTimeMoneyBox.drawMoneyBox(b, Game1.dayTimeMoneyBox.xPositionOnScreen, 4);
            b.End();
            Game1.PopUIMode();
            b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
            if (Game1.IsMultiplayer)
              Game1.player.team.festivalScoreStatus.Draw(b, new Vector2(32f, (float) (Game1.viewport.Height - 32)), draw_layer: 0.99f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
          }
        }
        else
        {
          b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(32, 32, 224, 160), Microsoft.Xna.Framework.Color.Black * 0.5f);
          Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1514", (object) (this.festivalTimer / 1000)), Microsoft.Xna.Framework.Color.Black, Microsoft.Xna.Framework.Color.Yellow, new Vector2(64f, 64f), 0.0f, 1f, 1f, false);
          Game1.drawWithBorder(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1515", (object) Game1.player.festivalScore), Microsoft.Xna.Framework.Color.Black, Microsoft.Xna.Framework.Color.Pink, new Vector2(64f, 128f), 0.0f, 1f, 1f, false);
          if (Game1.IsMultiplayer)
            Game1.player.team.festivalScoreStatus.Draw(b, new Vector2(32f, (float) (Game1.viewport.Height - 32)), draw_layer: 0.99f, vertical_origin: PlayerStatusList.VerticalAlignment.Bottom);
        }
      }
      if (this.spriteTextToDraw != null && this.spriteTextToDraw.Length > 0)
      {
        SpriteBatch b1 = b;
        string spriteTextToDraw = this.spriteTextToDraw;
        Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
        int x = viewport.Width / 2;
        viewport = Game1.graphics.GraphicsDevice.Viewport;
        int y = viewport.Height - 192;
        int useMeForAnything = this.int_useMeForAnything;
        int useMeForAnything2 = this.int_useMeForAnything2;
        SpriteText.drawStringHorizontallyCenteredAt(b1, spriteTextToDraw, x, y, useMeForAnything, layerDepth: 1f, color: useMeForAnything2);
      }
      foreach (Character actor in this.actors)
        actor.drawAboveAlwaysFrontLayer(b);
      if (this.skippable && !Game1.options.SnappyMenus && !Game1.game1.takingMapScreenshot)
      {
        Microsoft.Xna.Framework.Rectangle rectangle1 = this.skipBounds();
        Microsoft.Xna.Framework.Color white = Microsoft.Xna.Framework.Color.White;
        if (rectangle1.Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
          white *= 0.5f;
        Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(205, 406, 22, 15);
        b.Draw(Game1.mouseCursors, Utility.PointToVector2(rectangle1.Location), new Microsoft.Xna.Framework.Rectangle?(rectangle2), white, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.92f);
      }
      if (this.currentCustomEventScript == null)
        return;
      this.currentCustomEventScript.drawAboveAlwaysFront(b);
    }

    public void EndPlayerControlSequence()
    {
      this.playerControlSequence = false;
      this.playerControlSequenceID = (string) null;
    }

    public void OnPlayerControlSequenceEnd(string id)
    {
      Game1.player.CanMove = false;
      Game1.player.Halt();
    }

    public void setUpPlayerControlSequence(string id)
    {
      this.playerControlSequenceID = id;
      this.playerControlSequence = true;
      Game1.player.CanMove = true;
      Game1.viewportFreeze = false;
      Game1.forceSnapOnNextViewportUpdate = true;
      Game1.globalFade = false;
      this.doingSecretSanta = false;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(id))
      {
        case 5462067:
          if (!(id == "fair"))
            break;
          this.festivalHost = this.getActorByName("Lewis");
          this.hostMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1535");
          break;
        case 480875097:
          if (!(id == "boatRide"))
            break;
          Game1.viewportFreeze = true;
          Game1.currentViewportTarget = Utility.PointToVector2(Game1.viewportCenter);
          ++this.currentCommand;
          break;
        case 750634491:
          if (!(id == "christmas"))
            break;
          Random r = new Random((int) (Game1.uniqueIDForThisGame / 2UL) ^ Game1.year ^ (int) Game1.player.UniqueMultiplayerID);
          this.secretSantaRecipient = Utility.getRandomTownNPC(r);
          while (this.mySecretSanta == null || this.mySecretSanta.Equals((object) this.secretSantaRecipient) || this.mySecretSanta.isDivorcedFrom(this.farmer))
            this.mySecretSanta = Utility.getRandomTownNPC(r);
          Game1.debugOutput = "Secret Santa Recipient: " + this.secretSantaRecipient.Name + "  My Secret Santa: " + this.mySecretSanta.Name;
          break;
        case 863075767:
          if (!(id == "eggHunt"))
            break;
          for (int index1 = 0; index1 < Game1.currentLocation.map.GetLayer("Paths").LayerWidth; ++index1)
          {
            for (int index2 = 0; index2 < Game1.currentLocation.map.GetLayer("Paths").LayerHeight; ++index2)
            {
              if (Game1.currentLocation.map.GetLayer("Paths").Tiles[index1, index2] != null)
                this.festivalProps.Add(new Prop(this.festivalTexture, Game1.currentLocation.map.GetLayer("Paths").Tiles[index1, index2].TileIndex, 1, 1, 1, index1, index2));
            }
          }
          this.festivalTimer = 52000;
          ++this.currentCommand;
          break;
        case 875582698:
          if (!(id == "eggFestival"))
            break;
          this.festivalHost = this.getActorByName("Lewis");
          this.hostMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1521");
          break;
        case 1073062698:
          if (!(id == "flowerFestival"))
            break;
          this.festivalHost = this.getActorByName("Lewis");
          this.hostMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1524");
          if (!NetWorldState.checkAnywhereForWorldStateID("trashBearDone"))
            break;
          Game1.currentLocation.setMapTileIndex(62, 29, -1, "Buildings");
          Game1.currentLocation.setMapTileIndex(64, 29, -1, "Buildings");
          Game1.currentLocation.setMapTileIndex(72, 49, -1, "Buildings");
          break;
        case 2052688871:
          if (!(id == "haleyBeach"))
            break;
          this.playerControlTargetTile = new Point(53, 8);
          this.props.Add(new Object(new Vector2(53f, 8f), 742, 1)
          {
            Flipped = false
          });
          Game1.player.canOnlyWalk = false;
          break;
        case 2177915280:
          if (!(id == "iceFestival"))
            break;
          this.festivalHost = this.getActorByName("Lewis");
          this.hostMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1548");
          break;
        case 2614493766:
          if (!(id == "iceFishing"))
            break;
          this.festivalTimer = 120000;
          this.farmer.festivalScore = 0;
          this.farmer.CurrentToolIndex = 0;
          this.farmer.TemporaryItem = (Item) new FishingRod();
          (this.farmer.CurrentTool as FishingRod).attachments[1] = new Object(687, 1);
          this.farmer.CurrentToolIndex = 0;
          break;
        case 3356754971:
          if (!(id == "jellies"))
            break;
          this.festivalHost = this.getActorByName("Lewis");
          this.hostMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1531");
          break;
        case 3623672272:
          if (!(id == "halloween"))
            break;
          this.temporaryLocation.objects.Add(new Vector2(33f, 13f), (Object) new Chest(0, new List<Item>()
          {
            (Item) new Object(373, 1)
          }, new Vector2(33f, 13f)));
          break;
        case 3708370127:
          if (!(id == "parrotRide"))
            break;
          Game1.player.canOnlyWalk = false;
          ++this.currentCommand;
          break;
        case 3776204284:
          if (!(id == "luau"))
            break;
          this.festivalHost = this.getActorByName("Lewis");
          this.hostMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1527");
          break;
      }
    }

    public bool canMoveAfterDialogue()
    {
      if (this.playerControlSequenceID != null && this.playerControlSequenceID.Equals("eggHunt"))
      {
        Game1.player.canMove = true;
        ++this.CurrentCommand;
      }
      return this.playerControlSequence;
    }

    public void forceFestivalContinue()
    {
      if (this.festivalData["file"].Equals("fall16"))
      {
        this.initiateGrangeJudging();
      }
      else
      {
        Game1.dialogueUp = false;
        if (Game1.activeClickableMenu != null)
          Game1.activeClickableMenu.emergencyShutDown();
        Game1.exitActiveMenu();
        this.eventCommands = this.GetFestivalDataForYear("mainEvent").Split('/');
        this.CurrentCommand = 0;
        this.eventSwitched = true;
        this.playerControlSequence = false;
        this.setUpFestivalMainEvent();
        Game1.player.Halt();
      }
    }

    public bool isSpecificFestival(string festivalID) => this.isFestival && this.festivalData["file"].Equals(festivalID);

    public void setUpFestivalMainEvent()
    {
      if (!this.isSpecificFestival("spring24"))
        return;
      List<NetDancePartner> netDancePartnerList1 = new List<NetDancePartner>();
      List<NetDancePartner> netDancePartnerList2 = new List<NetDancePartner>();
      List<string> source = new List<string>()
      {
        "Abigail",
        "Penny",
        "Leah",
        "Maru",
        "Haley",
        "Emily"
      };
      List<string> stringList = new List<string>()
      {
        "Sebastian",
        "Sam",
        "Elliott",
        "Harvey",
        "Alex",
        "Shane"
      };
      List<Farmer> list = Game1.getOnlineFarmers().OrderBy<Farmer, long>((Func<Farmer, long>) (f => f.UniqueMultiplayerID)).ToList<Farmer>();
      while (list.Count > 0)
      {
        Farmer farmer = list[0];
        list.RemoveAt(0);
        if (!Game1.multiplayer.isDisconnecting(farmer) && farmer.dancePartner.Value != null)
        {
          if (farmer.dancePartner.GetGender() == 1)
          {
            netDancePartnerList1.Add(farmer.dancePartner);
            if (farmer.dancePartner.IsVillager())
              source.Remove(farmer.dancePartner.TryGetVillager().Name);
            netDancePartnerList2.Add(new NetDancePartner(farmer));
          }
          else
          {
            netDancePartnerList2.Add(farmer.dancePartner);
            if (farmer.dancePartner.IsVillager())
              stringList.Remove(farmer.dancePartner.TryGetVillager().Name);
            netDancePartnerList1.Add(new NetDancePartner(farmer));
          }
          if (farmer.dancePartner.IsFarmer())
            list.Remove(farmer.dancePartner.TryGetFarmer());
        }
      }
      while (netDancePartnerList1.Count < 6)
      {
        string str = source.Last<string>();
        if (stringList.Contains(Utility.getLoveInterest(str)))
        {
          netDancePartnerList1.Add(new NetDancePartner(str));
          netDancePartnerList2.Add(new NetDancePartner(Utility.getLoveInterest(str)));
        }
        source.Remove(str);
      }
      string str1 = this.GetFestivalDataForYear("mainEvent");
      for (int index = 1; index <= 6; ++index)
      {
        string newValue1 = !netDancePartnerList1[index - 1].IsVillager() ? "farmer" + Utility.getFarmerNumberFromFarmer(netDancePartnerList1[index - 1].TryGetFarmer()).ToString() : netDancePartnerList1[index - 1].TryGetVillager().Name;
        string newValue2 = !netDancePartnerList2[index - 1].IsVillager() ? "farmer" + Utility.getFarmerNumberFromFarmer(netDancePartnerList2[index - 1].TryGetFarmer()).ToString() : netDancePartnerList2[index - 1].TryGetVillager().Name;
        str1 = str1.Replace("Girl" + index.ToString(), newValue1).Replace("Guy" + index.ToString(), newValue2);
      }
      Regex regex1 = new Regex("showFrame (?<farmerName>farmer\\d) 44");
      Regex regex2 = new Regex("showFrame (?<farmerName>farmer\\d) 40");
      Regex regex3 = new Regex("animate (?<farmerName>farmer\\d) false true 600 44 45");
      Regex regex4 = new Regex("animate (?<farmerName>farmer\\d) false true 600 43 41 43 42");
      Regex regex5 = new Regex("animate (?<farmerName>farmer\\d) false true 300 46 47");
      Regex regex6 = new Regex("animate (?<farmerName>farmer\\d) false true 600 46 47");
      string input1 = str1;
      string input2 = regex1.Replace(input1, "showFrame $1 12/faceDirection $1 0");
      string input3 = regex2.Replace(input2, "showFrame $1 0/faceDirection $1 2");
      string input4 = regex3.Replace(input3, "animate $1 false true 600 12 13 12 14");
      string input5 = regex4.Replace(input4, "animate $1 false true 596 4 0");
      string input6 = regex5.Replace(input5, "animate $1 false true 150 12 13 12 14");
      this.eventCommands = regex6.Replace(input6, "animate $1 false true 600 0 3").Split('/');
    }

    public string FestivalName => this.festivalData == null ? "" : this.festivalData["name"];

    private void judgeGrange()
    {
      int num1 = 14;
      Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
      int num2 = 0;
      bool flag = false;
      foreach (Item i in Game1.player.team.grangeDisplay)
      {
        if (i != null && i is Object)
        {
          if (Event.IsItemMayorShorts((Item) (i as Object)))
            flag = true;
          num1 += (i as Object).Quality + 1;
          int storePrice = (i as Object).sellToStorePrice();
          if (storePrice >= 20)
            ++num1;
          if (storePrice >= 90)
            ++num1;
          if (storePrice >= 200)
            ++num1;
          if (storePrice >= 300 && (i as Object).Quality < 2)
            ++num1;
          if (storePrice >= 400 && (i as Object).Quality < 1)
            ++num1;
          switch ((i as Object).Category)
          {
            case -81:
            case -80:
            case -27:
              dictionary[-81] = true;
              continue;
            case -79:
              dictionary[-79] = true;
              continue;
            case -75:
              dictionary[-75] = true;
              continue;
            case -26:
              dictionary[-26] = true;
              continue;
            case -18:
            case -14:
            case -6:
            case -5:
              dictionary[-5] = true;
              continue;
            case -12:
            case -2:
              dictionary[-12] = true;
              continue;
            case -7:
              dictionary[-7] = true;
              continue;
            case -4:
              dictionary[-4] = true;
              continue;
            default:
              continue;
          }
        }
        else if (i == null)
          ++num2;
      }
      this.grangeScore = num1 + Math.Min(30, dictionary.Count * 5) + (9 - 2 * num2);
      if (!flag)
        return;
      this.grangeScore = -666;
    }

    private void lewisDoneJudgingGrange()
    {
      if (Game1.activeClickableMenu == null)
      {
        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1584")));
        Game1.player.Halt();
      }
      this.interpretGrangeResults();
    }

    public void interpretGrangeResults()
    {
      List<Character> characterList = new List<Character>();
      characterList.Add((Character) this.getActorByName("Pierre"));
      characterList.Add((Character) this.getActorByName("Marnie"));
      characterList.Add((Character) this.getActorByName("Willy"));
      if (this.grangeScore >= 90)
        characterList.Insert(0, (Character) Game1.player);
      else if (this.grangeScore >= 75)
        characterList.Insert(1, (Character) Game1.player);
      else if (this.grangeScore >= 60)
        characterList.Insert(2, (Character) Game1.player);
      else
        characterList.Add((Character) Game1.player);
      if (characterList[0] is NPC && characterList[0].Name.Equals("Pierre"))
        this.getActorByName("Pierre").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1591"));
      else
        this.getActorByName("Pierre").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1593"));
      this.getActorByName("Marnie").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1595"));
      this.getActorByName("Willy").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1597"));
      if (this.grangeScore == -666)
        this.getActorByName("Marnie").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1600"));
      this.grangeJudged = true;
    }

    private void initiateGrangeJudging()
    {
      this.judgeGrange();
      this.hostMessage = (string) null;
      this.setUpAdvancedMove("advancedMove Lewis False 2 0 0 7 8 0 4 3000 3 0 4 3000 3 0 4 3000 3 0 4 3000 -14 0 2 1000".Split(' '), new NPCController.endBehavior(this.lewisDoneJudgingGrange));
      this.getActorByName("Lewis").CurrentDialogue.Clear();
      this.setUpAdvancedMove("advancedMove Marnie False 0 1 4 1000".Split(' '));
      this.getActorByName("Marnie").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1602"));
      this.getActorByName("Pierre").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1604"));
      this.getActorByName("Willy").setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1606"));
    }

    public void answerDialogueQuestion(NPC who, string answerKey)
    {
      if (!this.isFestival)
        return;
      if (!(answerKey == "yes"))
      {
        if (answerKey == "no" || !(answerKey == "danceAsk"))
          return;
        if (Game1.player.spouse != null && who.Name.Equals(Game1.player.spouse))
        {
          Game1.player.dancePartner.Value = (Character) who;
          switch (Game1.player.spouse)
          {
            case "Abigail":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1613"));
              break;
            case "Alex":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1631"));
              break;
            case "Elliott":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1629"));
              break;
            case "Haley":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1621"));
              break;
            case "Harvey":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1627"));
              break;
            case "Leah":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1619"));
              break;
            case "Maru":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1617"));
              break;
            case "Penny":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1615"));
              break;
            case "Sam":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1625"));
              break;
            case "Sebastian":
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1623"));
              break;
            default:
              who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1632"));
              break;
          }
          foreach (NPC actor in this.actors)
          {
            if (actor.CurrentDialogue != null && actor.CurrentDialogue.Count > 0 && actor.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
              actor.CurrentDialogue.Clear();
          }
        }
        else if (!who.HasPartnerForDance && Game1.player.getFriendshipLevelForNPC(who.Name) >= 1000 && !who.isMarried())
        {
          string s = "";
          switch (who.Gender)
          {
            case 0:
              s = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1633");
              break;
            case 1:
              s = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1634");
              break;
          }
          try
          {
            Game1.player.changeFriendship(250, Game1.getCharacterFromName(who.Name));
          }
          catch (Exception ex)
          {
          }
          Game1.player.dancePartner.Value = (Character) who;
          who.setNewDialogue(s);
          foreach (NPC actor in this.actors)
          {
            if (actor.CurrentDialogue != null && actor.CurrentDialogue.Count > 0 && actor.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
              actor.CurrentDialogue.Clear();
          }
        }
        else if (who.HasPartnerForDance)
        {
          who.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1635"));
        }
        else
        {
          try
          {
            who.setNewDialogue(Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\" + who.Name)["danceRejection"]);
          }
          catch (Exception ex)
          {
            return;
          }
        }
        Game1.drawDialogue(who);
        who.immediateSpeak = true;
        who.facePlayer(Game1.player);
        who.Halt();
      }
      else if (this.festivalData["file"].Equals("fall16"))
      {
        this.initiateGrangeJudging();
        if (!Game1.IsServer)
          return;
        Game1.multiplayer.sendServerToClientsMessage("festivalEvent");
      }
      else
      {
        this.eventCommands = this.GetFestivalDataForYear("mainEvent").Split('/');
        this.CurrentCommand = 0;
        this.eventSwitched = true;
        this.playerControlSequence = false;
        this.setUpFestivalMainEvent();
        if (!Game1.IsServer)
          return;
        Game1.multiplayer.sendServerToClientsMessage("festivalEvent");
      }
    }

    public void addItemToGrangeDisplay(Item i, int position, bool force)
    {
      while (Game1.player.team.grangeDisplay.Count < 9)
        Game1.player.team.grangeDisplay.Add((Item) null);
      if (position < 0 || position >= Game1.player.team.grangeDisplay.Count || Game1.player.team.grangeDisplay[position] != null && !force)
        return;
      Game1.player.team.grangeDisplay[position] = i;
    }

    private bool onGrangeChange(
      Item i,
      int position,
      Item old,
      StorageContainer container,
      bool onRemoval)
    {
      if (!onRemoval)
      {
        if (i.Stack > 1 || i.Stack == 1 && old != null && old.Stack == 1 && i.canStackWith((ISalable) old))
        {
          if (old != null && i != null && old.canStackWith((ISalable) i))
          {
            container.ItemsToGrabMenu.actualInventory[position].Stack = 1;
            container.heldItem = old;
            return false;
          }
          if (old != null)
          {
            Utility.addItemToInventory(old, position, container.ItemsToGrabMenu.actualInventory);
            container.heldItem = i;
            return false;
          }
          int num = i.Stack - 1;
          Item one = i.getOne();
          one.Stack = num;
          container.heldItem = one;
          i.Stack = 1;
        }
      }
      else if (old != null && old.Stack > 1 && !old.Equals((object) i))
        return false;
      this.addItemToGrangeDisplay(!onRemoval || old != null && !old.Equals((object) i) ? i : (Item) null, position, true);
      return true;
    }

    public bool canPlayerUseTool()
    {
      if (this.festivalData == null || !this.festivalData.ContainsKey("file") || !this.festivalData["file"].Equals("winter8") || this.festivalTimer <= 0 || Game1.player.UsingTool)
        return false;
      this.previousFacingDirection = Game1.player.FacingDirection;
      return true;
    }

    public bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
    {
      if (this.isFestival)
      {
        if (this.temporaryLocation != null && this.temporaryLocation.objects.ContainsKey(new Vector2((float) tileLocation.X, (float) tileLocation.Y)))
          this.temporaryLocation.objects[new Vector2((float) tileLocation.X, (float) tileLocation.Y)].checkForAction(who);
        int tileIndexAt = Game1.currentLocation.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings");
        string str1 = Game1.currentLocation.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
        string tileSheetIdAt = Game1.currentLocation.getTileSheetIDAt(tileLocation.X, tileLocation.Y, "Buildings");
        if (Game1.currentSeason == "winter" && Game1.dayOfMonth == 8 && tileSheetIdAt == "fest" && (tileIndexAt == 1009 || tileIndexAt == 1010 || tileIndexAt == 1012 || tileIndexAt == 1013))
        {
          Game1.playSound("pig");
          return true;
        }
        bool flag1 = true;
        switch (tileIndexAt)
        {
          case 87:
          case 88:
            Response[] answerChoices1 = new Response[2]
            {
              new Response("Buy", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1654")),
              new Response("Leave", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1656"))
            };
            if (who.IsLocalPlayer && this.festivalData["file"].Equals("fall16"))
            {
              Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1659"), answerChoices1, "StarTokenShop");
              break;
            }
            break;
          case 175:
          case 176:
            if (tileSheetIdAt == "untitled tile sheet" && who.IsLocalPlayer && this.festivalData["file"].Equals("fall16"))
            {
              Game1.player.eatObject(new Object(241, 1), true);
              break;
            }
            break;
          case 308:
          case 309:
            Response[] answerChoices2 = new Response[3]
            {
              new Response("Orange", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1645")),
              new Response("Green", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1647")),
              new Response("I", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1650"))
            };
            if (who.IsLocalPlayer && this.festivalData["file"].Equals("fall16"))
            {
              Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1652")), answerChoices2, "wheelBet");
              break;
            }
            break;
          case 349:
          case 350:
          case 351:
            if (this.festivalData["file"].Equals("fall16"))
            {
              Game1.player.team.grangeMutex.RequestLock((Action) (() =>
              {
                while (Game1.player.team.grangeDisplay.Count < 9)
                  Game1.player.team.grangeDisplay.Add((Item) null);
                Game1.activeClickableMenu = (IClickableMenu) new StorageContainer((IList<Item>) Game1.player.team.grangeDisplay.ToList<Item>(), 9, itemChangeBehavior: new StorageContainer.behaviorOnItemChange(this.onGrangeChange), highlightMethod: new InventoryMenu.highlightThisItem(Utility.highlightSmallObjects));
                this.waitingForMenuClose = true;
              }));
              break;
            }
            break;
          case 501:
          case 502:
            Response[] answerChoices3 = new Response[2]
            {
              new Response("Play", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1662")),
              new Response("Leave", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1663"))
            };
            if (who.IsLocalPlayer && this.festivalData["file"].Equals("fall16"))
            {
              Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1666"), answerChoices3, "slingshotGame");
              break;
            }
            break;
          case 503:
          case 504:
            Response[] answerChoices4 = new Response[2]
            {
              new Response("Play", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1662")),
              new Response("Leave", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1663"))
            };
            if (who.IsLocalPlayer && this.festivalData["file"].Equals("fall16"))
            {
              Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1681"), answerChoices4, "fishingGame");
              break;
            }
            break;
          case 505:
          case 506:
            if (who.IsLocalPlayer && this.festivalData["file"].Equals("fall16"))
            {
              if (who.Money >= 100 && !who.mailReceived.Contains("fortuneTeller" + Game1.year.ToString()))
              {
                Response[] answerChoices5 = new Response[2]
                {
                  new Response("Read", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1688")),
                  new Response("No", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1690"))
                };
                Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1691")), answerChoices5, "fortuneTeller");
              }
              else if (who.mailReceived.Contains("fortuneTeller" + Game1.year.ToString()))
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1694")));
              else
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1695")));
              who.Halt();
              break;
            }
            break;
          case 510:
          case 511:
            if (who.IsLocalPlayer && this.festivalData["file"].Equals("fall16"))
            {
              if (this.festivalShops == null)
                this.festivalShops = new Dictionary<string, Dictionary<ISalable, int[]>>();
              if (!this.festivalShops.ContainsKey("starTokenShop"))
              {
                Dictionary<ISalable, int[]> dictionary = new Dictionary<ISalable, int[]>();
                dictionary.Add((ISalable) new Furniture(1307, Vector2.Zero), new int[2]
                {
                  100,
                  1
                });
                dictionary.Add((ISalable) new StardewValley.Objects.Hat(19), new int[2]
                {
                  500,
                  1
                });
                dictionary.Add((ISalable) new Object(Vector2.Zero, 110), new int[2]
                {
                  800,
                  1
                });
                if (!Game1.player.mailReceived.Contains("CF_Fair"))
                  dictionary.Add((ISalable) new Object(434, 1), new int[2]
                  {
                    2000,
                    1
                  });
                dictionary.Add((ISalable) new Furniture(2488, Vector2.Zero), new int[2]
                {
                  500,
                  1
                });
                switch (new Random((int) Game1.uniqueIDForThisGame + Game1.year * 17 + 19).Next(5))
                {
                  case 0:
                    dictionary.Add((ISalable) new Object(253, 1), new int[2]
                    {
                      400,
                      1
                    });
                    break;
                  case 1:
                    dictionary.Add((ISalable) new Object(215, 1), new int[2]
                    {
                      250,
                      1
                    });
                    break;
                  case 2:
                    dictionary.Add((ISalable) new Ring(888), new int[2]
                    {
                      1000,
                      1
                    });
                    break;
                  case 3:
                    dictionary.Add((ISalable) new Object(178, 100), new int[2]
                    {
                      500,
                      1
                    });
                    break;
                  case 4:
                    dictionary.Add((ISalable) new Object(770, 24), new int[2]
                    {
                      1000,
                      1
                    });
                    break;
                }
                this.festivalShops.Add("starTokenShop", dictionary);
              }
              Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1672"), Game1.currentLocation.createYesNoResponses(), "starTokenShop");
              break;
            }
            break;
          case 540:
            if (who.IsLocalPlayer && this.festivalData["file"].Equals("fall16"))
            {
              if (who.getTileX() == 29)
              {
                Game1.activeClickableMenu = (IClickableMenu) new StrengthGame();
                break;
              }
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1684")));
              break;
            }
            break;
          default:
            flag1 = false;
            break;
        }
        if (flag1)
          return true;
        if (str1 != null)
        {
          try
          {
            string[] actionparams = str1.Split(' ');
            string str2 = actionparams[0];
            if (!(str2 == "Shop"))
            {
              if (!(str2 == "Message"))
              {
                if (!(str2 == "Dialogue"))
                {
                  if (str2 == "LuauSoup")
                  {
                    if (!this.specialEventVariable2)
                      Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightLuauSoupItems), new ItemGrabMenu.behaviorOnItemSelect(this.clickToAddItemToLuauSoup), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1719"), canBeExitedWithKey: true, context: ((object) this));
                  }
                }
                else
                  Game1.drawObjectDialogue(Game1.currentLocation.actionParamsToString(actionparams).Replace("#", " "));
              }
              else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:" + actionparams[1].Replace("\"", "")));
            }
            else
            {
              if (!who.IsLocalPlayer)
                return false;
              if (this.festivalShops == null)
                this.festivalShops = new Dictionary<string, Dictionary<ISalable, int[]>>();
              Dictionary<ISalable, int[]> itemPriceAndStock;
              if (!this.festivalShops.ContainsKey(actionparams[1]))
              {
                string str3 = actionparams[1];
                string[] strArray = this.festivalData[actionparams[1]].Split(' ');
                itemPriceAndStock = new Dictionary<ISalable, int[]>();
                int maxValue = int.MaxValue;
                for (int index = 0; index < strArray.Length; index += 4)
                {
                  string str4 = strArray[index];
                  string str5 = strArray[index + 1];
                  int int32_1 = Convert.ToInt32(strArray[index + 1]);
                  int num = Convert.ToInt32(strArray[index + 2]);
                  int int32_2 = Convert.ToInt32(strArray[index + 3]);
                  Item key = (Item) null;
                  switch (str4)
                  {
                    case "B":
                    case "Boot":
                      key = (Item) new Boots(int32_1);
                      break;
                    case "BBL":
                    case "BBl":
                    case "BigBlueprint":
                      key = (Item) new Object(Vector2.Zero, int32_1, true);
                      break;
                    case "BL":
                    case "Blueprint":
                      key = (Item) new Object(int32_1, 1, true);
                      break;
                    case "BO":
                    case "BigObject":
                      key = (Item) new Object(Vector2.Zero, int32_1);
                      break;
                    case "F":
                      key = (Item) Furniture.GetFurnitureInstance(int32_1);
                      break;
                    case "H":
                    case "Hat":
                      key = (Item) new StardewValley.Objects.Hat(int32_1);
                      break;
                    case "O":
                    case "Object":
                      int initialStack = int32_2 > 0 ? int32_2 : 1;
                      key = (Item) new Object(int32_1, initialStack);
                      break;
                    case "R":
                    case "Ring":
                      key = (Item) new Ring(int32_1);
                      break;
                    case "W":
                    case "Weapon":
                      key = (Item) new MeleeWeapon(int32_1);
                      break;
                  }
                  if (key is Object && (key as Object).Category == -74)
                    num = (int) Math.Max(1f, (float) num * Game1.MasterPlayer.difficultyModifier);
                  if ((!(key is Object) || !(bool) (NetFieldBase<bool, NetBool>) (key as Object).isRecipe || !who.knowsRecipe(key.Name)) && key != null)
                    itemPriceAndStock.Add((ISalable) key, new int[2]
                    {
                      num,
                      int32_2 <= 0 ? maxValue : int32_2
                    });
                }
                this.festivalShops.Add(actionparams[1], itemPriceAndStock);
              }
              else
                itemPriceAndStock = this.festivalShops[actionparams[1]];
              if (itemPriceAndStock != null && itemPriceAndStock.Count > 0)
                Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(itemPriceAndStock);
              else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1714"));
            }
          }
          catch (Exception ex)
          {
          }
        }
        else if (this.isFestival)
        {
          if (who.IsLocalPlayer)
          {
            foreach (NPC actor in this.actors)
            {
              if (actor.getTileX() == tileLocation.X && actor.getTileY() == tileLocation.Y && actor is Child)
              {
                (actor as Child).checkAction(who, this.temporaryLocation);
                return true;
              }
              if (actor.getTileX() == tileLocation.X && actor.getTileY() == tileLocation.Y && (actor.CurrentDialogue.Count<Dialogue>() >= 1 || actor.CurrentDialogue.Count<Dialogue>() > 0 && !actor.CurrentDialogue.Peek().isOnFinalDialogue() || actor.Equals((object) this.festivalHost) || (bool) (NetFieldBase<bool, NetBool>) actor.datable && this.festivalData["file"].Equals("spring24") || this.secretSantaRecipient != null && actor.Name.Equals(this.secretSantaRecipient.Name)))
              {
                bool flag2 = who.friendshipData.ContainsKey(actor.Name) && who.friendshipData[actor.Name].IsDivorced();
                if ((this.grangeScore > -100 || this.grangeScore == -666) && actor.Equals((object) this.festivalHost) && this.grangeJudged)
                {
                  string s;
                  if (this.grangeScore >= 90)
                  {
                    Game1.playSound("reward");
                    s = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1723", (object) this.grangeScore);
                    Game1.player.festivalScore += 1000;
                  }
                  else if (this.grangeScore >= 75)
                  {
                    Game1.playSound("reward");
                    s = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1726", (object) this.grangeScore);
                    Game1.player.festivalScore += 500;
                  }
                  else if (this.grangeScore >= 60)
                  {
                    Game1.playSound("newArtifact");
                    s = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1729", (object) this.grangeScore);
                    Game1.player.festivalScore += 250;
                  }
                  else if (this.grangeScore == -666)
                  {
                    Game1.playSound("secret1");
                    s = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1730");
                    Game1.player.festivalScore += 750;
                  }
                  else
                  {
                    Game1.playSound("newArtifact");
                    s = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1732", (object) this.grangeScore);
                    Game1.player.festivalScore += 50;
                  }
                  this.grangeScore = -100;
                  actor.setNewDialogue(s);
                }
                else if (((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null || Game1.player.Equals((object) Game1.serverHost.Value)) && actor.Equals((object) this.festivalHost) && (actor.CurrentDialogue.Count<Dialogue>() == 0 || actor.CurrentDialogue.Peek().isOnFinalDialogue()) && this.hostMessage != null)
                  actor.setNewDialogue(this.hostMessage);
                else if (((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost == (NetRef<Farmer>) null || Game1.player.Equals((object) Game1.serverHost.Value)) && actor.Equals((object) this.festivalHost) && (actor.CurrentDialogue.Count == 0 || actor.CurrentDialogue.Peek().isOnFinalDialogue()) && this.hostMessage != null)
                  actor.setNewDialogue(this.hostMessage);
                if (this.isSpecificFestival("spring24") && !flag2 && ((bool) (NetFieldBase<bool, NetBool>) actor.datable || who.spouse != null && actor.Name.Equals(who.spouse)))
                {
                  actor.grantConversationFriendship(who);
                  if (who.dancePartner.Value == null)
                  {
                    if (actor.CurrentDialogue.Count > 0 && actor.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
                      actor.CurrentDialogue.Clear();
                    if (actor.CurrentDialogue.Count == 0)
                    {
                      actor.CurrentDialogue.Push(new Dialogue("...", actor));
                      if (actor.name.Equals((object) who.spouse))
                        actor.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1736", (object) actor.displayName), true);
                      else
                        actor.setNewDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1738", (object) actor.displayName), true);
                    }
                    else if (actor.CurrentDialogue.Peek().isOnFinalDialogue())
                    {
                      Dialogue dialogue = actor.CurrentDialogue.Peek();
                      Game1.drawDialogue(actor);
                      actor.faceTowardFarmerForPeriod(3000, 2, false, who);
                      who.Halt();
                      actor.CurrentDialogue = new Stack<Dialogue>();
                      actor.CurrentDialogue.Push(new Dialogue("...", actor));
                      actor.CurrentDialogue.Push(dialogue);
                      return true;
                    }
                  }
                  else if (actor.CurrentDialogue.Count > 0 && actor.CurrentDialogue.Peek().getCurrentDialogue().Equals("..."))
                    actor.CurrentDialogue.Clear();
                }
                if (!flag2 && this.secretSantaRecipient != null && actor.Name.Equals(this.secretSantaRecipient.Name))
                {
                  actor.grantConversationFriendship(who);
                  Game1.currentLocation.createQuestionDialogue(Game1.parseText((int) (NetFieldBase<int, NetInt>) this.secretSantaRecipient.gender == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1740", (object) this.secretSantaRecipient.displayName) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1741", (object) this.secretSantaRecipient.displayName)), Game1.currentLocation.createYesNoResponses(), "secretSanta");
                  who.Halt();
                  return true;
                }
                if (actor.CurrentDialogue.Count == 0)
                  return true;
                if (who.spouse != null && actor.Name.Equals(who.spouse) && !this.festivalData["file"].Equals("spring24"))
                {
                  if (actor.isRoommate() && this.festivalData.ContainsKey(actor.Name + "_roommate"))
                  {
                    actor.CurrentDialogue.Clear();
                    actor.CurrentDialogue.Push(new Dialogue(this.GetFestivalDataForYear(actor.Name + "_roommate"), actor));
                  }
                  else if (this.festivalData.ContainsKey(actor.Name + "_spouse"))
                  {
                    actor.CurrentDialogue.Clear();
                    actor.CurrentDialogue.Push(new Dialogue(this.GetFestivalDataForYear(actor.Name + "_spouse"), actor));
                  }
                }
                if (flag2)
                {
                  actor.CurrentDialogue.Clear();
                  actor.CurrentDialogue.Push(new Dialogue(Game1.content.Load<Dictionary<string, string>>("Characters\\Dialogue\\" + actor.Name)["divorced"], actor));
                }
                actor.grantConversationFriendship(who);
                Game1.drawDialogue(actor);
                actor.faceTowardFarmerForPeriod(3000, 2, false, who);
                who.Halt();
                return true;
              }
            }
          }
          if (this.festivalData != null && this.festivalData["file"].Equals("spring13"))
          {
            Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
            for (int index = this.festivalProps.Count - 1; index >= 0; --index)
            {
              if (this.festivalProps[index].isColliding(rectangle))
              {
                ++who.festivalScore;
                this.festivalProps.RemoveAt(index);
                who.team.FestivalPropsRemoved(rectangle);
                if (who.IsLocalPlayer)
                  Game1.playSound("coin");
                return true;
              }
            }
          }
        }
      }
      return false;
    }

    public void removeFestivalProps(Microsoft.Xna.Framework.Rectangle rect)
    {
      for (int index = this.festivalProps.Count - 1; index >= 0; --index)
      {
        if (this.festivalProps[index].isColliding(rect))
          this.festivalProps.RemoveAt(index);
      }
    }

    public void checkForSpecialCharacterIconAtThisTile(Vector2 tileLocation)
    {
      if (!this.isFestival || this.festivalHost == null || !this.festivalHost.getTileLocation().Equals(tileLocation))
        return;
      Game1.mouseCursor = 4;
    }

    public void forceEndFestival(Farmer who)
    {
      Game1.currentMinigame = (IMinigame) null;
      Game1.exitActiveMenu();
      Game1.player.Halt();
      this.endBehaviors((string[]) null, Game1.currentLocation);
      if (Game1.IsServer)
        Game1.multiplayer.sendServerToClientsMessage("endFest");
      Game1.changeMusicTrack("none");
    }

    public bool checkForCollision(Microsoft.Xna.Framework.Rectangle position, Farmer who)
    {
      foreach (NPC actor in this.actors)
      {
        Microsoft.Xna.Framework.Rectangle boundingBox = actor.GetBoundingBox();
        if (boundingBox.Intersects(position) && !this.farmer.temporarilyInvincible && this.farmer.TemporaryPassableTiles.IsEmpty() && !actor.IsInvisible)
        {
          boundingBox = who.GetBoundingBox();
          if (!boundingBox.Intersects(actor.GetBoundingBox()) && !actor.farmerPassesThrough)
            return true;
        }
      }
      if (position.X < 0 || position.Y < 0 || position.X >= Game1.currentLocation.map.Layers[0].DisplayWidth || position.Y >= Game1.currentLocation.map.Layers[0].DisplayHeight)
      {
        if (who.IsLocalPlayer && this.isFestival)
        {
          who.Halt();
          who.Position = who.lastPosition;
          if (!Game1.IsMultiplayer && Game1.activeClickableMenu == null)
            Game1.activeClickableMenu = (IClickableMenu) new ConfirmationDialog(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1758", (object) this.FestivalName), new ConfirmationDialog.behavior(this.forceEndFestival));
          else if (Game1.activeClickableMenu == null)
          {
            Game1.player.team.SetLocalReady("festivalEnd", true);
            Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog("festivalEnd", true, new ConfirmationDialog.behavior(this.forceEndFestival));
          }
        }
        return true;
      }
      foreach (Object prop in this.props)
      {
        if (prop.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) prop.tileLocation).Intersects(position))
          return true;
      }
      if (this.temporaryLocation != null)
      {
        foreach (Object @object in this.temporaryLocation.objects.Values)
        {
          if (@object.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) @object.tileLocation).Intersects(position))
            return true;
        }
      }
      foreach (Prop festivalProp in this.festivalProps)
      {
        if (festivalProp.isColliding(position))
          return true;
      }
      return false;
    }

    public void answerDialogue(string questionKey, int answerChoice)
    {
      this.previousAnswerChoice = answerChoice;
      if (questionKey.Contains("fork"))
      {
        int int32 = Convert.ToInt32(questionKey.Replace("fork", ""));
        if (answerChoice != int32)
          return;
        this.specialEventVariable1 = !this.specialEventVariable1;
      }
      else if (questionKey.Contains("quickQuestion"))
      {
        string eventCommand = this.eventCommands[Math.Min(this.eventCommands.Length - 1, this.CurrentCommand)];
        string[] collection = eventCommand.Substring(eventCommand.IndexOf(' ') + 1).Split(new string[1]
        {
          "(break)"
        }, StringSplitOptions.None)[1 + answerChoice].Split('\\');
        List<string> list = ((IEnumerable<string>) this.eventCommands).ToList<string>();
        list.InsertRange(this.CurrentCommand + 1, (IEnumerable<string>) collection);
        this.eventCommands = list.ToArray();
      }
      else
      {
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(questionKey))
        {
          case 119764934:
            if (!(questionKey == "shaneLoan"))
              break;
            if (answerChoice != 0)
              break;
            this.specialEventVariable1 = true;
            this.eventCommands[this.currentCommand + 1] = "fork giveShaneLoan";
            Game1.player.Money -= 3000;
            break;
          case 269688027:
            if (!(questionKey == "wheelBet"))
              break;
            this.specialEventVariable2 = answerChoice == 1;
            if (answerChoice == 2)
              break;
            Game1.activeClickableMenu = (IClickableMenu) new NumberSelectionMenu(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1776"), new NumberSelectionMenu.behaviorOnNumberSelect(this.betStarTokens), minValue: 1, maxValue: Game1.player.festivalScore, defaultNumber: Math.Min(1, Game1.player.festivalScore));
            break;
          case 390240131:
            if (!(questionKey == "shaneCliffs"))
              break;
            switch (answerChoice)
            {
              case 0:
                this.eventCommands[this.currentCommand + 2] = "speak Shane \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1760") + "\"";
                return;
              case 1:
                this.eventCommands[this.currentCommand + 2] = "speak Shane \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1761") + "\"";
                return;
              case 2:
                this.eventCommands[this.currentCommand + 2] = "speak Shane \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1763") + "\"";
                return;
              case 3:
                this.eventCommands[this.currentCommand + 2] = "speak Shane \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1764") + "\"";
                return;
              default:
                return;
            }
          case 472382138:
            if (!(questionKey == "fortuneTeller") || answerChoice != 0)
              break;
            Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.readFortune));
            Game1.player.Money -= 100;
            Game1.player.mailReceived.Add("fortuneTeller" + Game1.year.ToString());
            break;
          case 504494762:
            if (!(questionKey == "starTokenShop") || answerChoice != 0)
              break;
            if (this.festivalShops["starTokenShop"].Count == 0)
            {
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1785")));
              break;
            }
            Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(this.festivalShops["starTokenShop"], 1, context: "StardewFair");
            break;
          case 1766558334:
            if (!(questionKey == "pet"))
              break;
            if (answerChoice == 0)
            {
              Game1.activeClickableMenu = (IClickableMenu) new NamingMenu(new NamingMenu.doneNamingBehavior(this.namePet), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1236"), Game1.player.IsMale ? (Game1.player.catPerson ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1794") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1795")) : (Game1.player.catPerson ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1796") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1797")));
              break;
            }
            Game1.player.mailReceived.Add("rejectedPet");
            this.eventCommands = new string[2];
            this.eventCommands[1] = "end";
            this.eventCommands[0] = "speak Marnie \"" + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1798") + "\"";
            this.currentCommand = 0;
            this.eventSwitched = true;
            this.specialEventVariable1 = true;
            break;
          case 1836559258:
            if (!(questionKey == "secretSanta") || answerChoice != 0)
              break;
            Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightSantaObjects), new ItemGrabMenu.behaviorOnItemSelect(this.chooseSecretSantaGift), Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1788", (object) this.secretSantaRecipient.displayName), context: ((object) this));
            break;
          case 2205664227:
            if (!(questionKey == "haleyDarkRoom"))
              break;
            switch (answerChoice)
            {
              case 0:
                this.specialEventVariable1 = true;
                this.eventCommands[this.currentCommand + 1] = "fork decorate";
                return;
              case 1:
                this.specialEventVariable1 = true;
                this.eventCommands[this.currentCommand + 1] = "fork leave";
                return;
              case 2:
                return;
              default:
                return;
            }
          case 2249818047:
            if (!(questionKey == "chooseCharacter"))
              break;
            switch (answerChoice)
            {
              case 0:
                this.specialEventVariable1 = true;
                this.eventCommands[this.currentCommand + 1] = "fork warrior";
                return;
              case 1:
                this.specialEventVariable1 = true;
                this.eventCommands[this.currentCommand + 1] = "fork healer";
                return;
              case 2:
                return;
              default:
                return;
            }
          case 2337399242:
            if (!(questionKey == "StarTokenShop") || answerChoice != 0)
              break;
            Game1.activeClickableMenu = (IClickableMenu) new NumberSelectionMenu(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1774"), new NumberSelectionMenu.behaviorOnNumberSelect(this.buyStarTokens), 50, maxValue: 999);
            break;
          case 2536635992:
            if (!(questionKey == "cave"))
              break;
            if (answerChoice == 0)
            {
              Game1.MasterPlayer.caveChoice.Value = 2;
              (Game1.getLocationFromName("FarmCave") as FarmCave).setUpMushroomHouse();
              break;
            }
            Game1.MasterPlayer.caveChoice.Value = 1;
            break;
          case 2900380439:
            if (!(questionKey == "fishingGame"))
              break;
            if (answerChoice == 0 && Game1.player.Money >= 50)
            {
              Game1.globalFadeToBlack(new Game1.afterFadeFunction(FishingGame.startMe), 0.01f);
              Game1.player.Money -= 50;
              break;
            }
            if (answerChoice != 0 || Game1.player.Money >= 50)
              break;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1780"));
            break;
          case 3007187074:
            if (!(questionKey == "bandFork"))
              break;
            switch (answerChoice)
            {
              case 76:
                this.specialEventVariable1 = true;
                this.eventCommands[this.currentCommand + 1] = "fork poppy";
                return;
              case 77:
                this.specialEventVariable1 = true;
                this.eventCommands[this.currentCommand + 1] = "fork heavy";
                return;
              case 78:
                this.specialEventVariable1 = true;
                this.eventCommands[this.currentCommand + 1] = "fork techno";
                return;
              case 79:
                this.specialEventVariable1 = true;
                this.eventCommands[this.currentCommand + 1] = "fork honkytonk";
                return;
              default:
                return;
            }
          case 3548149252:
            if (!(questionKey == "slingshotGame"))
              break;
            if (answerChoice == 0 && Game1.player.Money >= 50)
            {
              Game1.globalFadeToBlack(new Game1.afterFadeFunction(TargetGame.startMe), 0.01f);
              Game1.player.Money -= 50;
              break;
            }
            if (answerChoice != 0 || Game1.player.Money >= 50)
              break;
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1780"));
            break;
        }
      }
    }

    private void namePet(string name)
    {
      Pet pet = !Game1.player.catPerson ? (Pet) new Dog(68, 13, Game1.player.whichPetBreed) : (Pet) new Cat(68, 13, Game1.player.whichPetBreed);
      pet.warpToFarmHouse(Game1.player);
      pet.Name = name;
      pet.displayName = (string) (NetFieldBase<string, NetString>) pet.name;
      Game1.exitActiveMenu();
      ++this.CurrentCommand;
    }

    public void chooseSecretSantaGift(Item i, Farmer who)
    {
      if (i == null)
        return;
      if (i is Object)
      {
        if (i.Stack > 1)
        {
          --i.Stack;
          who.addItemToInventory(i);
        }
        Game1.exitActiveMenu();
        NPC actorByName = this.getActorByName(this.secretSantaRecipient.Name);
        actorByName.faceTowardFarmerForPeriod(15000, 5, false, who);
        actorByName.receiveGift(i as Object, who, false, 5f, false);
        actorByName.CurrentDialogue.Clear();
        if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
          actorByName.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1801", (object) i.DisplayName), actorByName));
        else
          actorByName.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1801", (object) i.DisplayName, (object) Lexicon.getProperArticleForWord(i.DisplayName)), actorByName));
        Game1.drawDialogue(actorByName);
        this.secretSantaRecipient = (NPC) null;
        this.startSecretSantaAfterDialogue = true;
        who.Halt();
        who.completelyStopAnimatingOrDoingAction();
        who.faceGeneralDirection(actorByName.Position, 0, false, false);
      }
      else
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1803"));
    }

    public void perfectFishing()
    {
      if (!this.isFestival || Game1.currentMinigame == null || !this.festivalData["file"].Equals("fall16"))
        return;
      ++(Game1.currentMinigame as FishingGame).perfections;
    }

    public void caughtFish(int whichFish, int size, Farmer who)
    {
      if (!this.isFestival)
        return;
      if (whichFish != -1 && Game1.currentMinigame != null && this.festivalData["file"].Equals("fall16"))
      {
        (Game1.currentMinigame as FishingGame).score += size > 0 ? size + 5 : 1;
        if (size > 0)
          ++(Game1.currentMinigame as FishingGame).fishCaught;
        Game1.player.FarmerSprite.PauseForSingleAnimation = false;
        Game1.player.FarmerSprite.StopAnimation();
      }
      else
      {
        if (whichFish == -1 || !this.festivalData["file"].Equals("winter8"))
          return;
        if (size > 0 && who.getTileX() < 79 && who.getTileY() < 43)
        {
          ++who.festivalScore;
          Game1.playSound("newArtifact");
        }
        who.forceCanMove();
        if (this.previousFacingDirection == -1)
          return;
        who.faceDirection(this.previousFacingDirection);
      }
    }

    public void readFortune()
    {
      Game1.globalFade = true;
      Game1.fadeToBlackAlpha = 1f;
      NPC romanticInterest1 = Utility.getTopRomanticInterest(Game1.player);
      NPC romanticInterest2 = Utility.getTopNonRomanticInterest(Game1.player);
      int highestSkill = Utility.getHighestSkill(Game1.player);
      string[] messages = new string[5];
      if (romanticInterest2 != null && Game1.player.getFriendshipLevelForNPC(romanticInterest2.Name) > 100)
      {
        if (Utility.getNumberOfFriendsWithinThisRange(Game1.player, Game1.player.getFriendshipLevelForNPC(romanticInterest2.Name) - 100, Game1.player.getFriendshipLevelForNPC(romanticInterest2.Name)) > 3 && Game1.random.NextDouble() < 0.5)
        {
          messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1810");
        }
        else
        {
          switch (Game1.random.Next(4))
          {
            case 0:
              messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1811", (object) romanticInterest2.displayName);
              break;
            case 1:
              messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1813", (object) romanticInterest2.displayName) + (romanticInterest2.Gender == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1815") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1816"));
              break;
            case 2:
              messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1818", (object) romanticInterest2.displayName);
              break;
            case 3:
              messages[0] = (romanticInterest2.Gender == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1820") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1821")) + Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1823", (object) romanticInterest2.displayName);
              break;
          }
        }
      }
      else
        messages[0] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1825");
      if (romanticInterest1 != null && Game1.player.getFriendshipLevelForNPC(romanticInterest1.Name) > 250)
      {
        if (Utility.getNumberOfFriendsWithinThisRange(Game1.player, Game1.player.getFriendshipLevelForNPC(romanticInterest1.Name) - 100, Game1.player.getFriendshipLevelForNPC(romanticInterest1.Name), true) > 2)
        {
          messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1826");
        }
        else
        {
          switch (Game1.random.Next(4))
          {
            case 0:
              messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1827", (object) romanticInterest1.displayName);
              break;
            case 1:
              messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1827", (object) romanticInterest1.displayName);
              break;
            case 2:
              messages[1] = (romanticInterest1.Gender == 0 ? (romanticInterest1.SocialAnxiety == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1831") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1832")) : (romanticInterest1.SocialAnxiety == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1833") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1834"))) + " " + (romanticInterest1.Gender == 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1837", (object) romanticInterest1.displayName[0]) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1838", (object) romanticInterest1.displayName[0]));
              break;
            case 3:
              messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1843", (object) romanticInterest1.displayName);
              break;
          }
        }
      }
      else
        messages[1] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1845");
      switch (highestSkill)
      {
        case 0:
          messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1846");
          break;
        case 1:
          messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1849");
          break;
        case 2:
          messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1850");
          break;
        case 3:
          messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1847");
          break;
        case 4:
          messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1848");
          break;
        case 5:
          messages[2] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1851");
          break;
      }
      messages[3] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1852");
      messages[4] = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1853");
      Game1.multipleDialogues(messages);
      Game1.afterDialogues = new Game1.afterFadeFunction(this.fadeClearAndviewportUnfreeze);
      Game1.viewportFreeze = true;
      Game1.viewport.X = -9999;
    }

    public void fadeClearAndviewportUnfreeze()
    {
      Game1.fadeClear();
      Game1.viewportFreeze = false;
    }

    public void betStarTokens(int value, int price, Farmer who)
    {
      if (value > who.festivalScore)
        return;
      Game1.playSound("smallSelect");
      Game1.activeClickableMenu = (IClickableMenu) new WheelSpinGame(value);
    }

    public void buyStarTokens(int value, int price, Farmer who)
    {
      if (value <= 0 || value * price > who.Money)
        return;
      who.Money -= price * value;
      who.festivalScore += value;
      Game1.playSound("purchase");
      Game1.exitActiveMenu();
    }

    public void clickToAddItemToLuauSoup(Item i, Farmer who) => this.addItemToLuauSoup(i, who);

    public void setUpAdvancedMove(string[] split, NPCController.endBehavior endBehavior = null)
    {
      if (this.npcControllers == null)
        this.npcControllers = new List<NPCController>();
      List<Vector2> path = new List<Vector2>();
      for (int index = 3; index < split.Length; index += 2)
        path.Add(new Vector2((float) Convert.ToInt32(split[index]), (float) Convert.ToInt32(split[index + 1])));
      if (split[1].Contains("farmer"))
      {
        this.npcControllers.Add(new NPCController((Character) this.getFarmerFromFarmerNumberString(split[1], this.farmer), path, Convert.ToBoolean(split[2]), endBehavior));
      }
      else
      {
        NPC actorByName = this.getActorByName(split[1].Replace('_', ' '));
        if (actorByName == null)
          return;
        this.npcControllers.Add(new NPCController((Character) actorByName, path, Convert.ToBoolean(split[2]), endBehavior));
      }
    }

    public static bool IsItemMayorShorts(Item i) => Utility.IsNormalObjectAtParentSheetIndex(i, 789) || Utility.IsNormalObjectAtParentSheetIndex(i, 71);

    public void addItemToLuauSoup(Item i, Farmer who)
    {
      if (i == null)
        return;
      who.team.luauIngredients.Add(i.getOne());
      if (!who.IsLocalPlayer)
        return;
      this.specialEventVariable2 = true;
      bool flag = Event.IsItemMayorShorts(i);
      if (i != null && i.Stack > 1 && !flag)
      {
        --i.Stack;
        who.addItemToInventory(i);
      }
      else if (flag)
        who.addItemToInventory(i);
      Game1.exitActiveMenu();
      Game1.playSound("dropItemInWater");
      if (i != null)
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1857", (object) i.DisplayName));
      string str = "";
      if (i is Object)
        str = (i as Object).Quality == 1 ? " ([51])" : ((i as Object).Quality == 2 ? " ([52])" : ((i as Object).Quality == 4 ? " ([53])" : ""));
      Game1.multiplayer.globalChatInfoMessage("LuauSoup", Game1.player.Name, i.DisplayName + str);
    }

    private void governorTaste()
    {
      int num1 = 5;
      foreach (Item luauIngredient in Game1.player.team.luauIngredients)
      {
        Object i = luauIngredient as Object;
        int num2 = 5;
        if (Event.IsItemMayorShorts((Item) i))
        {
          num1 = 6;
          break;
        }
        if (i.Quality >= 2 && (int) (NetFieldBase<int, NetInt>) i.price >= 160 || i.Quality == 1 && (int) (NetFieldBase<int, NetInt>) i.price >= 300 && (int) (NetFieldBase<int, NetInt>) i.edibility > 10)
        {
          num2 = 4;
          Utility.improveFriendshipWithEveryoneInRegion(Game1.player, 120, 2);
        }
        else if ((int) (NetFieldBase<int, NetInt>) i.edibility >= 20 || (int) (NetFieldBase<int, NetInt>) i.price >= 100 || (int) (NetFieldBase<int, NetInt>) i.price >= 70 && i.Quality >= 1)
        {
          num2 = 3;
          Utility.improveFriendshipWithEveryoneInRegion(Game1.player, 60, 2);
        }
        else if ((int) (NetFieldBase<int, NetInt>) i.price > 20 && (int) (NetFieldBase<int, NetInt>) i.edibility >= 10 || (int) (NetFieldBase<int, NetInt>) i.price >= 40 && (int) (NetFieldBase<int, NetInt>) i.edibility >= 5)
          num2 = 2;
        else if ((int) (NetFieldBase<int, NetInt>) i.edibility >= 0)
        {
          num2 = 1;
          Utility.improveFriendshipWithEveryoneInRegion(Game1.player, -50, 2);
        }
        if ((int) (NetFieldBase<int, NetInt>) i.edibility > -300 && (int) (NetFieldBase<int, NetInt>) i.edibility < 0)
        {
          num2 = 0;
          Utility.improveFriendshipWithEveryoneInRegion(Game1.player, -100, 2);
        }
        if (num2 < num1)
          num1 = num2;
      }
      if (num1 != 6 && Game1.player.team.luauIngredients.Count < Game1.numberOfPlayers())
        num1 = 5;
      this.eventCommands[this.CurrentCommand + 1] = "switchEvent governorReaction" + num1.ToString();
    }

    private void eggHuntWinner()
    {
      int num = 12;
      switch (Game1.numberOfPlayers())
      {
        case 1:
          num = 9;
          break;
        case 2:
          num = 6;
          break;
        case 3:
          num = 5;
          break;
        case 4:
          num = 4;
          break;
      }
      List<Farmer> source = new List<Farmer>();
      Farmer player = Game1.player;
      int festivalScore = Game1.player.festivalScore;
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer.festivalScore > festivalScore)
          festivalScore = onlineFarmer.festivalScore;
      }
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer.festivalScore == festivalScore)
        {
          source.Add(onlineFarmer);
          this.festivalWinners.Add(onlineFarmer.UniqueMultiplayerID);
        }
      }
      string masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1862");
      if (festivalScore >= num)
      {
        if (source.Count == 1)
        {
          masterDialogue = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es ? "¡" + source[0].displayName + "!" : source[0].displayName + "!";
        }
        else
        {
          string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1864");
          for (int index = 0; index < source.Count; ++index)
          {
            if (index == source.Count<Farmer>() - 1)
              str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1865");
            str = str + " " + source[index].displayName;
            if (index < source.Count - 1)
              str += ",";
          }
          masterDialogue = str + "!";
        }
        this.specialEventVariable1 = false;
      }
      else
        this.specialEventVariable1 = true;
      this.getActorByName("Lewis").CurrentDialogue.Push(new Dialogue(masterDialogue, this.getActorByName("Lewis")));
      Game1.drawDialogue(this.getActorByName("Lewis"));
    }

    private void iceFishingWinner()
    {
      int num = 5;
      this.winners = new List<Farmer>();
      Farmer player = Game1.player;
      int festivalScore = Game1.player.festivalScore;
      for (int number = 1; number <= Game1.numberOfPlayers(); ++number)
      {
        Farmer fromFarmerNumber = Utility.getFarmerFromFarmerNumber(number);
        if (fromFarmerNumber != null && fromFarmerNumber.festivalScore > festivalScore)
          festivalScore = fromFarmerNumber.festivalScore;
      }
      for (int number = 1; number <= Game1.numberOfPlayers(); ++number)
      {
        Farmer fromFarmerNumber = Utility.getFarmerFromFarmerNumber(number);
        if (fromFarmerNumber != null && fromFarmerNumber.festivalScore == festivalScore)
        {
          this.winners.Add(fromFarmerNumber);
          this.festivalWinners.Add(fromFarmerNumber.UniqueMultiplayerID);
        }
      }
      string masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1871");
      if (festivalScore >= num)
      {
        if (this.winners.Count == 1)
        {
          masterDialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1872", (object) this.winners[0].displayName, (object) this.winners[0].festivalScore);
        }
        else
        {
          string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1864");
          for (int index = 0; index < this.winners.Count; ++index)
          {
            if (index == this.winners.Count<Farmer>() - 1)
              str += Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1865");
            str = str + " " + this.winners[index].displayName;
            if (index < this.winners.Count - 1)
              str += ",";
          }
          masterDialogue = str + "!";
        }
        this.specialEventVariable1 = false;
      }
      else
        this.specialEventVariable1 = true;
      this.getActorByName("Lewis").CurrentDialogue.Push(new Dialogue(masterDialogue, this.getActorByName("Lewis")));
      Game1.drawDialogue(this.getActorByName("Lewis"));
    }

    private void iceFishingWinnerMP() => this.specialEventVariable1 = !this.winners.Contains(Game1.player);
  }
}
