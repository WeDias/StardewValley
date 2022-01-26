// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.CraneGame
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.GameData.Movies;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Minigames
{
  public class CraneGame : IMinigame
  {
    public int gameWidth = 304;
    public int gameHeight = 150;
    protected LocalizedContentManager _content;
    public Texture2D spriteSheet;
    public Vector2 upperLeft;
    protected List<CraneGame.CraneGameObject> _gameObjects;
    protected Dictionary<CraneGame.GameButtons, int> _buttonStates;
    protected bool _shouldQuit;
    public Action onQuit;
    public ICue music;
    public ICue fastMusic;
    public Effect _effect;
    public int freezeFrames;
    public ICue craneSound;
    public List<Type> _gameObjectTypes;
    public Dictionary<Type, List<CraneGame.CraneGameObject>> _gameObjectsByType;

    public CraneGame()
    {
      Utility.farmerHeardSong("crane_game");
      Utility.farmerHeardSong("crane_game_fast");
      this._effect = Game1.content.Load<Effect>("Effects\\ShadowRemoveMG3.8.0");
      this._content = Game1.content.CreateTemporary();
      this.spriteSheet = this._content.Load<Texture2D>("LooseSprites\\CraneGame");
      this._buttonStates = new Dictionary<CraneGame.GameButtons, int>();
      this._gameObjects = new List<CraneGame.CraneGameObject>();
      this._gameObjectTypes = new List<Type>();
      this._gameObjectsByType = new Dictionary<Type, List<CraneGame.CraneGameObject>>();
      this.changeScreenSize();
      CraneGame.GameLogic gameLogic = new CraneGame.GameLogic(this);
      for (int key = 0; key < 9; ++key)
        this._buttonStates[(CraneGame.GameButtons) key] = 0;
    }

    public void Quit()
    {
      if (this._shouldQuit)
        return;
      if (this.onQuit != null)
        this.onQuit();
      this._shouldQuit = true;
    }

    protected void _UpdateInput()
    {
      HashSet<InputButton> emulated_keys = new HashSet<InputButton>();
      if (Game1.options.gamepadControls)
      {
        GamePadState gamePadState = Game1.input.GetGamePadState();
        foreach (Buttons b in new ButtonCollection(ref gamePadState))
        {
          Keys key = Utility.mapGamePadButtonToKey(b);
          emulated_keys.Add(new InputButton(key));
        }
      }
      if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed)
        emulated_keys.Add(new InputButton(true));
      else if (Game1.input.GetMouseState().RightButton == ButtonState.Pressed)
        emulated_keys.Add(new InputButton(false));
      this._UpdateButtonState(CraneGame.GameButtons.Action, Game1.options.actionButton, emulated_keys);
      this._UpdateButtonState(CraneGame.GameButtons.Tool, Game1.options.useToolButton, emulated_keys);
      this._UpdateButtonState(CraneGame.GameButtons.Confirm, Game1.options.menuButton, emulated_keys);
      this._UpdateButtonState(CraneGame.GameButtons.Cancel, Game1.options.cancelButton, emulated_keys);
      this._UpdateButtonState(CraneGame.GameButtons.Run, Game1.options.runButton, emulated_keys);
      this._UpdateButtonState(CraneGame.GameButtons.Up, Game1.options.moveUpButton, emulated_keys);
      this._UpdateButtonState(CraneGame.GameButtons.Down, Game1.options.moveDownButton, emulated_keys);
      this._UpdateButtonState(CraneGame.GameButtons.Left, Game1.options.moveLeftButton, emulated_keys);
      this._UpdateButtonState(CraneGame.GameButtons.Right, Game1.options.moveRightButton, emulated_keys);
    }

    public bool IsButtonPressed(CraneGame.GameButtons button) => this._buttonStates[button] == 1;

    public bool IsButtonDown(CraneGame.GameButtons button) => this._buttonStates[button] > 0;

    public bool IsButtonReleased(CraneGame.GameButtons button) => this._buttonStates[button] == -1;

    public bool IsButtonDownRepeating(
      CraneGame.GameButtons button,
      int repeat_rate,
      int repeat_delay = 0)
    {
      int num = this._buttonStates[button];
      if (num < 0)
        num = -1;
      return num >= repeat_delay && (num - repeat_delay) % repeat_rate == 0;
    }

    public int GetButtonHoldTime(CraneGame.GameButtons button) => Math.Max(this._buttonStates[button], 0);

    protected void _UpdateButtonState(
      CraneGame.GameButtons button,
      InputButton[] keys,
      HashSet<InputButton> emulated_keys)
    {
      bool flag = Game1.isOneOfTheseKeysDown(Game1.GetKeyboardState(), keys);
      for (int index = 0; index < keys.Length; ++index)
      {
        if (emulated_keys.Contains(keys[index]))
        {
          flag = true;
          break;
        }
      }
      if (this._buttonStates[button] == -1)
        this._buttonStates[button] = 0;
      if (flag)
      {
        this._buttonStates[button]++;
      }
      else
      {
        if (this._buttonStates[button] <= 0)
          return;
        this._buttonStates[button] = -1;
      }
    }

    public T GetObjectAtPoint<T>(Vector2 point, int max_count = -1) where T : CraneGame.CraneGameObject
    {
      foreach (CraneGame.CraneGameObject gameObject in this._gameObjects)
      {
        if (gameObject is T && gameObject.GetBounds().Contains((int) point.X, (int) point.Y))
          return gameObject as T;
      }
      return default (T);
    }

    public List<T> GetObjectsAtPoint<T>(Vector2 point, int max_count = -1) where T : CraneGame.CraneGameObject
    {
      List<T> objectsAtPoint = new List<T>();
      foreach (CraneGame.CraneGameObject gameObject in this._gameObjects)
      {
        if (gameObject is T && gameObject.GetBounds().Contains((int) point.X, (int) point.Y))
        {
          objectsAtPoint.Add(gameObject as T);
          if (max_count >= 0 && objectsAtPoint.Count >= max_count)
            return objectsAtPoint;
        }
      }
      return objectsAtPoint;
    }

    public T GetObjectOfType<T>() where T : CraneGame.CraneGameObject
    {
      if (this._gameObjectsByType.ContainsKey(typeof (T)))
      {
        List<CraneGame.CraneGameObject> craneGameObjectList = this._gameObjectsByType[typeof (T)];
        if (craneGameObjectList.Count > 0)
          return craneGameObjectList[0] as T;
      }
      return default (T);
    }

    public List<T> GetObjectsOfType<T>() where T : CraneGame.CraneGameObject
    {
      List<T> objectsOfType = new List<T>();
      foreach (CraneGame.CraneGameObject gameObject in this._gameObjects)
      {
        if (gameObject is T)
          objectsOfType.Add(gameObject as T);
      }
      return objectsOfType;
    }

    public T GetOverlap<T>(CraneGame.CraneGameObject target, int max_count = -1) where T : CraneGame.CraneGameObject
    {
      foreach (CraneGame.CraneGameObject gameObject in this._gameObjects)
      {
        if (gameObject is T && target.GetBounds().Intersects(gameObject.GetBounds()) && target != gameObject)
          return gameObject as T;
      }
      return default (T);
    }

    public List<T> GetOverlaps<T>(CraneGame.CraneGameObject target, int max_count = -1) where T : CraneGame.CraneGameObject
    {
      List<T> overlaps = new List<T>();
      foreach (CraneGame.CraneGameObject gameObject in this._gameObjects)
      {
        if (gameObject is T && target.GetBounds().Intersects(gameObject.GetBounds()) && target != gameObject)
        {
          overlaps.Add(gameObject as T);
          if (max_count >= 0 && overlaps.Count >= max_count)
            return overlaps;
        }
      }
      return overlaps;
    }

    public bool tick(GameTime time)
    {
      if (this._shouldQuit)
      {
        if (this.music != null && this.music.IsPlaying)
          this.music.Stop(AudioStopOptions.Immediate);
        if (this.fastMusic != null && this.fastMusic.IsPlaying)
          this.fastMusic.Stop(AudioStopOptions.Immediate);
        if (this.craneSound != null && !this.craneSound.IsStopped)
          this.craneSound.Stop(AudioStopOptions.Immediate);
        return true;
      }
      if (this.freezeFrames > 0)
      {
        --this.freezeFrames;
      }
      else
      {
        this._UpdateInput();
        for (int index = 0; index < this._gameObjects.Count; ++index)
        {
          if (this._gameObjects[index] != null)
            this._gameObjects[index].Update(time);
        }
      }
      if (this.IsButtonPressed(CraneGame.GameButtons.Confirm))
      {
        this.Quit();
        Game1.playSound("bigDeSelect");
        CraneGame.GameLogic objectOfType = this.GetObjectOfType<CraneGame.GameLogic>();
        if (objectOfType != null && objectOfType.collectedItems.Count > 0)
        {
          List<Item> inventory = new List<Item>();
          foreach (Item collectedItem in objectOfType.collectedItems)
            inventory.Add(collectedItem.getOne());
          Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) inventory, false, true, (InventoryMenu.highlightThisItem) null, (ItemGrabMenu.behaviorOnItemSelect) null, "Rewards", playRightClickSound: false, allowRightClick: false, context: ((object) this));
        }
      }
      return false;
    }

    public bool forceQuit()
    {
      this.Quit();
      CraneGame.GameLogic objectOfType = this.GetObjectOfType<CraneGame.GameLogic>();
      if (objectOfType != null)
      {
        List<Item> objList = new List<Item>();
        foreach (Item collectedItem in objectOfType.collectedItems)
          Utility.CollectOrDrop(collectedItem.getOne());
      }
      return true;
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public bool doMainGameUpdates() => false;

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveKeyPress(Keys k)
    {
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void RegisterGameObject(CraneGame.CraneGameObject game_object)
    {
      if (!this._gameObjectTypes.Contains(game_object.GetType()))
      {
        this._gameObjectTypes.Add(game_object.GetType());
        this._gameObjectsByType[game_object.GetType()] = new List<CraneGame.CraneGameObject>();
      }
      this._gameObjectsByType[game_object.GetType()].Add(game_object);
      this._gameObjects.Add(game_object);
    }

    public void UnregisterGameObject(CraneGame.CraneGameObject game_object)
    {
      this._gameObjectsByType[game_object.GetType()].Remove(game_object);
      this._gameObjects.Remove(game_object);
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, effect: this._effect);
      b.Draw(this.spriteSheet, this.upperLeft, new Rectangle?(new Rectangle(0, 0, this.gameWidth, this.gameHeight)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      Dictionary<CraneGame.CraneGameObject, float> dictionary = new Dictionary<CraneGame.CraneGameObject, float>();
      float num1 = 0.0f;
      float num2 = 0.0f;
      for (int index = 0; index < this._gameObjects.Count; ++index)
      {
        if (this._gameObjects[index] != null)
        {
          float rendererLayerDepth = this._gameObjects[index].GetRendererLayerDepth();
          dictionary[this._gameObjects[index]] = rendererLayerDepth;
          if ((double) rendererLayerDepth < (double) num1)
            num1 = rendererLayerDepth;
          if ((double) rendererLayerDepth > (double) num2)
            num2 = rendererLayerDepth;
        }
      }
      for (int index1 = 0; index1 < this._gameObjectTypes.Count; ++index1)
      {
        Type gameObjectType = this._gameObjectTypes[index1];
        for (int index2 = 0; index2 < this._gameObjectsByType[gameObjectType].Count; ++index2)
        {
          float layer_depth = Utility.Lerp(0.1f, 0.9f, (float) (((double) dictionary[this._gameObjectsByType[gameObjectType][index2]] - (double) num1) / ((double) num2 - (double) num1)));
          this._gameObjectsByType[gameObjectType][index2].Draw(b, layer_depth);
        }
      }
      b.End();
    }

    public void changeScreenSize()
    {
      float num = 1f / Game1.options.zoomLevel;
      Rectangle multiplayerWindow = Game1.game1.localMultiplayerWindow;
      Vector2 vector2 = new Vector2((float) ((double) multiplayerWindow.Width / 2.0), (float) multiplayerWindow.Height / 2f) * num;
      vector2.X -= (float) (this.gameWidth / 2 * 4);
      vector2.Y -= (float) (this.gameHeight / 2 * 4);
      if (!(this.upperLeft != vector2))
        return;
      this.upperLeft = vector2;
      Console.WriteLine("CraneGame.changeScreenSize(); vp={0}, zl={1}, upperLeft={2}", (object) multiplayerWindow, (object) Game1.options.zoomLevel, (object) this.upperLeft);
    }

    public void unload()
    {
      Game1.stopMusicTrack(Game1.MusicContext.MiniGame);
      this._content.Unload();
    }

    public void receiveEventPoke(int data)
    {
    }

    public string minigameId() => nameof (CraneGame);

    public enum GameButtons
    {
      Action,
      Tool,
      Confirm,
      Cancel,
      Run,
      Up,
      Left,
      Down,
      Right,
      MAX,
    }

    public class GameLogic : CraneGame.CraneGameObject
    {
      public List<Item> collectedItems;
      public const int CLAW_HEIGHT = 50;
      protected CraneGame.Claw _claw;
      public int maxLives = 3;
      public int lives = 3;
      public Vector2 _startPosition = new Vector2(24f, 56f);
      public Vector2 _dropPosition = new Vector2(32f, 56f);
      public Rectangle playArea = new Rectangle(16, 48, 272, 64);
      public Rectangle prizeChute = new Rectangle(16, 48, 32, 32);
      protected CraneGame.GameLogic.GameStates _currentState;
      protected int _stateTimer;
      public CraneGame.CraneGameObject moveRightIndicator;
      public CraneGame.CraneGameObject moveDownIndicator;
      public CraneGame.CraneGameObject creditsDisplay;
      public CraneGame.CraneGameObject timeDisplay1;
      public CraneGame.CraneGameObject timeDisplay2;
      public CraneGame.CraneGameObject sunShockedFace;
      public int currentTimer;
      public CraneGame.CraneGameObject joystick;
      public int[] conveyerBeltTiles = new int[68]
      {
        0,
        0,
        0,
        0,
        7,
        6,
        6,
        9,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        8,
        0,
        0,
        2,
        0,
        0,
        0,
        7,
        6,
        6,
        6,
        6,
        9,
        0,
        0,
        0,
        0,
        8,
        0,
        0,
        2,
        0,
        0,
        0,
        8,
        0,
        0,
        0,
        0,
        2,
        0,
        0,
        0,
        0,
        1,
        4,
        4,
        3,
        0,
        0,
        0,
        1,
        4,
        4,
        4,
        4,
        3
      };
      public int[] prizeMap = new int[68]
      {
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        1,
        0,
        1,
        0,
        2,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        1,
        0,
        0,
        1,
        0,
        0,
        0,
        0,
        1,
        0,
        2,
        0,
        3
      };

      public GameLogic(CraneGame game)
        : base(game)
      {
        this._game.music = Game1.soundBank.GetCue("crane_game");
        this._game.fastMusic = Game1.soundBank.GetCue("crane_game_fast");
        this._game.music.Play();
        this._claw = new CraneGame.Claw(this._game);
        this._claw.position = this._startPosition;
        this._claw.zPosition = 50f;
        this.collectedItems = new List<Item>();
        this.SetState(CraneGame.GameLogic.GameStates.Setup);
        CraneGame.Bush bush1 = new CraneGame.Bush(this._game, 55, 2, 3, 31, 111);
        CraneGame.Bush bush2 = new CraneGame.Bush(this._game, 45, 2, 2, 112, 84);
        CraneGame.Bush bush3 = new CraneGame.Bush(this._game, 45, 2, 2, 63, 63);
        CraneGame.Bush bush4 = new CraneGame.Bush(this._game, 48, 1, 2, 56, 80);
        CraneGame.Bush bush5 = new CraneGame.Bush(this._game, 48, 1, 2, 72, 80);
        CraneGame.Bush bush6 = new CraneGame.Bush(this._game, 48, 1, 2, 56, 96);
        CraneGame.Bush bush7 = new CraneGame.Bush(this._game, 48, 1, 2, 72, 96);
        CraneGame.Bush bush8 = new CraneGame.Bush(this._game, 48, 1, 2, 56, 112);
        CraneGame.Bush bush9 = new CraneGame.Bush(this._game, 48, 1, 2, 72, 112);
        CraneGame.Bush bush10 = new CraneGame.Bush(this._game, 45, 2, 2, 159, 63);
        CraneGame.Bush bush11 = new CraneGame.Bush(this._game, 48, 1, 2, 152, 80);
        CraneGame.Bush bush12 = new CraneGame.Bush(this._game, 48, 1, 2, 168, 80);
        CraneGame.Bush bush13 = new CraneGame.Bush(this._game, 48, 1, 2, 152, 96);
        CraneGame.Bush bush14 = new CraneGame.Bush(this._game, 48, 1, 2, 168, 96);
        CraneGame.Bush bush15 = new CraneGame.Bush(this._game, 48, 1, 2, 152, 112);
        CraneGame.Bush bush16 = new CraneGame.Bush(this._game, 48, 1, 2, 168, 112);
        this.sunShockedFace = new CraneGame.CraneGameObject(this._game);
        this.sunShockedFace.SetSpriteFromIndex(9);
        this.sunShockedFace.position = new Vector2(96f, 0.0f);
        this.sunShockedFace.spriteAnchor = Vector2.Zero;
        CraneGame.CraneGameObject craneGameObject = new CraneGame.CraneGameObject(this._game);
        craneGameObject.position.X = 16f;
        craneGameObject.position.Y = 87f;
        craneGameObject.SetSpriteFromIndex(3);
        craneGameObject.spriteRect.Width = 32;
        craneGameObject.spriteAnchor = new Vector2(0.0f, 15f);
        this.joystick = new CraneGame.CraneGameObject(this._game);
        this.joystick.position.X = 151f;
        this.joystick.position.Y = 134f;
        this.joystick.SetSpriteFromIndex(28);
        this.joystick.spriteRect.Width = 32;
        this.joystick.spriteRect.Height = 48;
        this.joystick.spriteAnchor = new Vector2(15f, 47f);
        this.lives = this.maxLives;
        this.moveRightIndicator = new CraneGame.CraneGameObject(this._game);
        this.moveRightIndicator.position.X = 21f;
        this.moveRightIndicator.position.Y = 126f;
        this.moveRightIndicator.SetSpriteFromIndex(26);
        this.moveRightIndicator.spriteAnchor = Vector2.Zero;
        this.moveRightIndicator.visible = false;
        this.moveDownIndicator = new CraneGame.CraneGameObject(this._game);
        this.moveDownIndicator.position.X = 49f;
        this.moveDownIndicator.position.Y = 126f;
        this.moveDownIndicator.SetSpriteFromIndex(27);
        this.moveDownIndicator.spriteAnchor = Vector2.Zero;
        this.moveDownIndicator.visible = false;
        this.creditsDisplay = new CraneGame.CraneGameObject(this._game);
        this.creditsDisplay.SetSpriteFromIndex(70);
        this.creditsDisplay.position = new Vector2(234f, 125f);
        this.creditsDisplay.spriteAnchor = Vector2.Zero;
        this.timeDisplay1 = new CraneGame.CraneGameObject(this._game);
        this.timeDisplay1.SetSpriteFromIndex(70);
        this.timeDisplay1.position = new Vector2(274f, 125f);
        this.timeDisplay1.spriteAnchor = Vector2.Zero;
        this.timeDisplay2 = new CraneGame.CraneGameObject(this._game);
        this.timeDisplay2.SetSpriteFromIndex(70);
        this.timeDisplay2.position = new Vector2(285f, 125f);
        this.timeDisplay2.spriteAnchor = Vector2.Zero;
        int num1 = 17;
        for (int index = 0; index < this.conveyerBeltTiles.Length; ++index)
        {
          if (this.conveyerBeltTiles[index] != 0)
          {
            int x = index % num1 + 1;
            int y = index / num1 + 3;
            if (this.conveyerBeltTiles[index] == 8)
            {
              CraneGame.ConveyerBelt conveyerBelt1 = new CraneGame.ConveyerBelt(this._game, x, y, 0);
            }
            else if (this.conveyerBeltTiles[index] == 4)
            {
              CraneGame.ConveyerBelt conveyerBelt2 = new CraneGame.ConveyerBelt(this._game, x, y, 3);
            }
            else if (this.conveyerBeltTiles[index] == 6)
            {
              CraneGame.ConveyerBelt conveyerBelt3 = new CraneGame.ConveyerBelt(this._game, x, y, 1);
            }
            else if (this.conveyerBeltTiles[index] == 2)
            {
              CraneGame.ConveyerBelt conveyerBelt4 = new CraneGame.ConveyerBelt(this._game, x, y, 2);
            }
            else if (this.conveyerBeltTiles[index] == 7)
              new CraneGame.ConveyerBelt(this._game, x, y, 1).SetSpriteFromCorner(240, 272);
            else if (this.conveyerBeltTiles[index] == 9)
              new CraneGame.ConveyerBelt(this._game, x, y, 2).SetSpriteFromCorner(240, 240);
            else if (this.conveyerBeltTiles[index] == 1)
              new CraneGame.ConveyerBelt(this._game, x, y, 0).SetSpriteFromCorner(240, 224);
            else if (this.conveyerBeltTiles[index] == 3)
              new CraneGame.ConveyerBelt(this._game, x, y, 3).SetSpriteFromCorner(240, 256);
          }
        }
        Dictionary<int, List<Item>> dictionary = new Dictionary<int, List<Item>>();
        dictionary[1] = new List<Item>()
        {
          (Item) new Furniture(1760, Vector2.Zero),
          (Item) new Furniture(1761, Vector2.Zero),
          (Item) new Furniture(1762, Vector2.Zero),
          (Item) new Furniture(1763, Vector2.Zero),
          (Item) new Furniture(1764, Vector2.Zero),
          (Item) new Furniture(1365, Vector2.Zero)
        };
        List<Item> objList1 = new List<Item>();
        MovieData movieForDate = MovieTheater.GetMovieForDate(Game1.Date);
        if (movieForDate != null)
        {
          switch (movieForDate.ID)
          {
            case "fall_movie_0":
              objList1.Add((Item) new Furniture(1953, Vector2.Zero));
              break;
            case "fall_movie_1":
              objList1.Add((Item) new Furniture(1959, Vector2.Zero));
              break;
            case "spring_movie_0":
              objList1.Add((Item) new Furniture(1952, Vector2.Zero));
              break;
            case "spring_movie_1":
              objList1.Add((Item) new Furniture(1958, Vector2.Zero));
              break;
            case "summer_movie_0":
              objList1.Add((Item) new Furniture(1954, Vector2.Zero));
              break;
            case "summer_movie_1":
              objList1.Add((Item) new Furniture(1955, Vector2.Zero));
              break;
            case "winter_movie_0":
              objList1.Add((Item) new Furniture(1957, Vector2.Zero));
              break;
            case "winter_movie_1":
              objList1.Add((Item) new Furniture(1956, Vector2.Zero));
              break;
          }
        }
        objList1.Add((Item) new Furniture(1669, Vector2.Zero));
        if (Game1.currentSeason == "spring")
          objList1.Add((Item) new Furniture(1960, Vector2.Zero));
        else if (Game1.currentSeason == "winter")
          objList1.Add((Item) new Furniture(1961, Vector2.Zero));
        else if (Game1.currentSeason == "summer")
          objList1.Add((Item) new Furniture(1294, Vector2.Zero));
        else if (Game1.currentSeason == "fall")
          objList1.Add((Item) new Furniture(1918, Vector2.Zero));
        objList1.Add((Item) new StardewValley.Object(Vector2.Zero, 2));
        dictionary[2] = objList1;
        List<Item> objList2 = new List<Item>();
        if (Game1.currentSeason == "spring")
        {
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 107));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 36));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 48));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 184));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 188));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 192));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 204));
        }
        else if (Game1.currentSeason == "winter")
        {
          objList2.Add((Item) new Furniture(1440, Vector2.Zero));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 44));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 40));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 41));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 43));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 42));
        }
        else if (Game1.currentSeason == "summer")
        {
          if (movieForDate != null && movieForDate.ID == "summer_movie_1")
          {
            objList2.Add((Item) new Furniture(1371, Vector2.Zero));
            objList2.Add((Item) new Furniture(1373, Vector2.Zero));
          }
          else
          {
            objList2.Add((Item) new Furniture(985, Vector2.Zero));
            objList2.Add((Item) new Furniture(984, Vector2.Zero));
          }
        }
        else if (Game1.currentSeason == "fall")
        {
          objList2.Add((Item) new Furniture(1917, Vector2.Zero));
          objList2.Add((Item) new Furniture(1307, Vector2.Zero));
          objList2.Add((Item) new StardewValley.Object(Vector2.Zero, 47));
          objList2.Add((Item) new Furniture(1471, Vector2.Zero));
          objList2.Add((Item) new Furniture(1375, Vector2.Zero));
        }
        dictionary[3] = objList2;
        for (int index1 = 0; index1 < this.prizeMap.Length; ++index1)
        {
          if (this.prizeMap[index1] != 0)
          {
            int num2 = index1 % num1 + 1;
            int num3 = index1 / num1 + 3;
            Item obj = (Item) null;
            for (int index2 = index1; index2 > 0 && obj == null; --index2)
            {
              if (this.prizeMap[index1] == 1)
                obj = Utility.GetRandom<Item>(dictionary[1]);
              else if (this.prizeMap[index1] == 2)
                obj = Utility.GetRandom<Item>(dictionary[2]);
              else if (this.prizeMap[index1] == 3)
                obj = Utility.GetRandom<Item>(dictionary[3]);
            }
            CraneGame.Prize prize = new CraneGame.Prize(this._game, obj);
            prize.position.X = (float) (num2 * 16 + 8);
            prize.position.Y = (float) (num3 * 16 + 8);
          }
        }
        if (Game1.random.NextDouble() < 0.1)
        {
          Item obj = (Item) null;
          Vector2 vector2 = new Vector2(0.0f, 4f);
          switch (Game1.random.Next(4))
          {
            case 0:
              obj = (Item) new StardewValley.Object(107, 1);
              break;
            case 1:
              obj = (Item) new StardewValley.Object(749, 5);
              break;
            case 2:
              obj = (Item) new StardewValley.Object(688, 5);
              break;
            case 3:
              obj = (Item) new StardewValley.Object(288, 5);
              break;
          }
          CraneGame.Prize prize = new CraneGame.Prize(this._game, obj);
          prize.position.X = (float) ((double) vector2.X * 16.0 + 30.0);
          prize.position.Y = (float) ((double) vector2.Y * 16.0 + 32.0);
        }
        else if (Game1.random.NextDouble() < 0.2)
        {
          CraneGame.Prize prize = new CraneGame.Prize(this._game, (Item) new StardewValley.Object(809, 1));
          prize.position.X = 160f;
          prize.position.Y = 58f;
        }
        if (Game1.random.NextDouble() < 0.25)
        {
          CraneGame.Prize prize1 = new CraneGame.Prize(this._game, (Item) new Furniture(986, Vector2.Zero));
          prize1.position = new Vector2(263f, 56f);
          prize1.zPosition = 0.0f;
          CraneGame.Prize prize2 = new CraneGame.Prize(this._game, (Item) new Furniture(986, Vector2.Zero));
          prize2.position = new Vector2(215f, 56f);
          prize2.zPosition = 0.0f;
        }
        else
        {
          CraneGame.Prize prize3 = new CraneGame.Prize(this._game, (Item) new Furniture(989, Vector2.Zero));
          prize3.position = new Vector2(263f, 56f);
          prize3.zPosition = 0.0f;
          CraneGame.Prize prize4 = new CraneGame.Prize(this._game, (Item) new Furniture(989, Vector2.Zero));
          prize4.position = new Vector2(215f, 56f);
          prize4.zPosition = 0.0f;
        }
      }

      public CraneGame.GameLogic.GameStates GetCurrentState() => this._currentState;

      public override void Update(GameTime time)
      {
        float to = 0.0f;
        foreach (CraneGame.Shadow shadow in this._game.GetObjectsOfType<CraneGame.Shadow>())
        {
          if (this.prizeChute.Contains(new Point((int) shadow.position.X, (int) shadow.position.Y)))
            shadow.visible = false;
          else
            shadow.visible = true;
        }
        int num = this.currentTimer / 60;
        if (this._currentState == CraneGame.GameLogic.GameStates.Setup)
          this.creditsDisplay.SetSpriteFromIndex(70);
        else
          this.creditsDisplay.SetSpriteFromIndex(70 + this.lives);
        this.timeDisplay1.SetSpriteFromIndex(70 + num / 10);
        this.timeDisplay2.SetSpriteFromIndex(70 + num % 10);
        if (this.currentTimer < 0)
        {
          this.timeDisplay1.SetSpriteFromIndex(80);
          this.timeDisplay2.SetSpriteFromIndex(81);
        }
        if (this._currentState == CraneGame.GameLogic.GameStates.Setup)
        {
          if (!this._game.music.IsPlaying)
            this._game.music.Play();
          this._claw.openAngle = 40f;
          bool flag = false;
          foreach (CraneGame.Prize prize in this._game.GetObjectsOfType<CraneGame.Prize>())
          {
            if (!prize.CanBeGrabbed())
            {
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            if (this._stateTimer >= 10)
              this.SetState(CraneGame.GameLogic.GameStates.Idle);
          }
          else
            this._stateTimer = 0;
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.Idle)
        {
          if (!this._game.music.IsPlaying)
            this._game.music.Play();
          if (this._game.fastMusic.IsPlaying)
          {
            this._game.fastMusic.Stop(AudioStopOptions.Immediate);
            this._game.fastMusic = Game1.soundBank.GetCue("crane_game_fast");
          }
          this.currentTimer = 900;
          this.moveRightIndicator.visible = Game1.ticks / 20 % 2 == 0;
          if (this._game.IsButtonPressed(CraneGame.GameButtons.Tool) || this._game.IsButtonPressed(CraneGame.GameButtons.Action) || this._game.IsButtonPressed(CraneGame.GameButtons.Right))
          {
            Game1.playSound("bigSelect");
            this.SetState(CraneGame.GameLogic.GameStates.MoveClawRight);
          }
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.MoveClawRight)
        {
          to = 15f;
          if (this._stateTimer < 15)
          {
            if (!this._game.IsButtonDown(CraneGame.GameButtons.Tool) && !this._game.IsButtonDown(CraneGame.GameButtons.Action) && !this._game.IsButtonDown(CraneGame.GameButtons.Right))
            {
              Game1.playSound("bigDeSelect");
              this.SetState(CraneGame.GameLogic.GameStates.Idle);
              return;
            }
          }
          else
          {
            if (this._game.craneSound == null || !this._game.craneSound.IsPlaying)
            {
              this._game.craneSound = Game1.soundBank.GetCue("crane");
              this._game.craneSound.Play();
              this._game.craneSound.SetVariable("Pitch", 1200);
            }
            --this.currentTimer;
            if (this.currentTimer <= 0)
            {
              this.SetState(CraneGame.GameLogic.GameStates.ClawDescend);
              this.currentTimer = -1;
              if (this._game.craneSound != null && !this._game.craneSound.IsStopped)
                this._game.craneSound.Stop(AudioStopOptions.Immediate);
            }
            this.moveRightIndicator.visible = true;
            if (this._stateTimer > 10)
            {
              if (this._stateTimer == 11)
              {
                this._claw.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(1f, 1f));
                this._claw.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.SwayEffect(2f, 10f, 20));
                this._claw.ApplyDrawEffectToArms((CraneGame.DrawEffect) new CraneGame.SwayEffect(15f, 4f, 50));
              }
              if (!this._game.IsButtonDown(CraneGame.GameButtons.Tool) && !this._game.IsButtonDown(CraneGame.GameButtons.Right) && !this._game.IsButtonDown(CraneGame.GameButtons.Action))
              {
                Game1.playSound("bigDeSelect");
                this._claw.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.SwayEffect(2f, 10f, 20));
                this._claw.ApplyDrawEffectToArms((CraneGame.DrawEffect) new CraneGame.SwayEffect(15f, 4f, 100));
                this.SetState(CraneGame.GameLogic.GameStates.WaitForMoveDown);
                this.moveRightIndicator.visible = false;
                if (this._game.craneSound != null && !this._game.craneSound.IsStopped)
                  this._game.craneSound.Stop(AudioStopOptions.Immediate);
              }
              else
              {
                this._claw.Move(0.5f, 0.0f);
                if (this._claw.GetBounds().Right >= this.playArea.Right)
                  this._claw.Move(-0.5f, 0.0f);
              }
            }
          }
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.WaitForMoveDown)
        {
          --this.currentTimer;
          if (this.currentTimer <= 0)
          {
            this.SetState(CraneGame.GameLogic.GameStates.ClawDescend);
            this.currentTimer = -1;
          }
          this.moveDownIndicator.visible = Game1.ticks / 20 % 2 == 0;
          if (this._game.IsButtonPressed(CraneGame.GameButtons.Tool) || this._game.IsButtonPressed(CraneGame.GameButtons.Down) || this._game.IsButtonPressed(CraneGame.GameButtons.Action))
          {
            Game1.playSound("bigSelect");
            this.SetState(CraneGame.GameLogic.GameStates.MoveClawDown);
          }
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.MoveClawDown)
        {
          if (this._game.craneSound == null || !this._game.craneSound.IsPlaying)
          {
            this._game.craneSound = Game1.soundBank.GetCue("crane");
            this._game.craneSound.SetVariable("Pitch", 1200);
            this._game.craneSound.Play();
          }
          --this.currentTimer;
          if (this.currentTimer <= 0)
          {
            this.SetState(CraneGame.GameLogic.GameStates.ClawDescend);
            this.currentTimer = -1;
            if (this._game.craneSound != null && !this._game.craneSound.IsStopped)
              this._game.craneSound.Stop(AudioStopOptions.Immediate);
          }
          to = -5f;
          this.moveDownIndicator.visible = true;
          if (this._stateTimer > 10)
          {
            if (this._stateTimer == 11)
            {
              this._claw.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(1f, 1f));
              this._claw.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.SwayEffect(2f, 10f, 20));
              this._claw.ApplyDrawEffectToArms((CraneGame.DrawEffect) new CraneGame.SwayEffect(15f, 4f, 50));
            }
            if (!this._game.IsButtonDown(CraneGame.GameButtons.Tool) && !this._game.IsButtonDown(CraneGame.GameButtons.Down) && !this._game.IsButtonDown(CraneGame.GameButtons.Action))
            {
              Game1.playSound("bigDeSelect");
              this._claw.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.SwayEffect(2f, 10f, 20));
              this._claw.ApplyDrawEffectToArms((CraneGame.DrawEffect) new CraneGame.SwayEffect(15f, 4f, 100));
              this.moveDownIndicator.visible = false;
              this.SetState(CraneGame.GameLogic.GameStates.ClawDescend);
              if (this._game.craneSound != null && !this._game.craneSound.IsStopped)
                this._game.craneSound.Stop(AudioStopOptions.Immediate);
            }
            else
            {
              this._claw.Move(0.0f, 0.5f);
              if (this._claw.GetBounds().Bottom >= this.playArea.Bottom)
                this._claw.Move(0.0f, -0.5f);
            }
          }
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.ClawDescend)
        {
          if ((double) this._claw.openAngle < 40.0)
          {
            this._claw.openAngle += 1.5f;
            this._stateTimer = 0;
          }
          else if (this._stateTimer > 30)
          {
            if (this._game.craneSound != null && this._game.craneSound.IsPlaying)
            {
              this._game.craneSound.SetVariable("Pitch", 2000);
            }
            else
            {
              this._game.craneSound = Game1.soundBank.GetCue("crane");
              this._game.craneSound.SetVariable("Pitch", 2000);
              this._game.craneSound.Play();
            }
            if ((double) this._claw.zPosition > 0.0)
            {
              this._claw.zPosition -= 0.5f;
              if ((double) this._claw.zPosition <= 0.0)
              {
                this._claw.zPosition = 0.0f;
                this.SetState(CraneGame.GameLogic.GameStates.ClawAscend);
                if (this._game.craneSound != null && !this._game.craneSound.IsStopped)
                  this._game.craneSound.Stop(AudioStopOptions.Immediate);
              }
            }
          }
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.ClawAscend)
        {
          if ((double) this._claw.openAngle > 0.0 && this._claw.GetGrabbedPrize() == null)
          {
            --this._claw.openAngle;
            if ((double) this._claw.openAngle == 15.0)
            {
              this._claw.GrabObject();
              if (this._claw.GetGrabbedPrize() != null)
              {
                Game1.playSound("FishHit");
                this.sunShockedFace.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(1f, 1f, 5));
                this._game.freezeFrames = 60;
                if (this._game.music.IsPlaying)
                {
                  this._game.music.Stop(AudioStopOptions.Immediate);
                  this._game.music = Game1.soundBank.GetCue("crane_game");
                }
              }
            }
            else if ((double) this._claw.openAngle == 0.0 && this._claw.GetGrabbedPrize() == null)
            {
              if (this.lives == 1)
              {
                this._game.music.Stop(AudioStopOptions.Immediate);
                Game1.playSound("fishEscape");
              }
              else
                Game1.playSound("stoneStep");
            }
            this._stateTimer = 0;
          }
          else
          {
            if (this._claw.GetGrabbedPrize() != null)
            {
              if (!this._game.fastMusic.IsPlaying)
                this._game.fastMusic.Play();
            }
            else if (this._game.fastMusic.IsPlaying)
            {
              this._game.fastMusic.Stop(AudioStopOptions.AsAuthored);
              this._game.fastMusic = Game1.soundBank.GetCue("crane_game_fast");
            }
            if ((double) this._claw.zPosition < 50.0)
            {
              this._claw.zPosition += 0.5f;
              if ((double) this._claw.zPosition >= 50.0)
              {
                this._claw.zPosition = 50f;
                this.SetState(CraneGame.GameLogic.GameStates.ClawReturn);
                if (this._claw.GetGrabbedPrize() == null && this.lives == 1)
                  this.SetState(CraneGame.GameLogic.GameStates.EndGame);
              }
            }
            this._claw.CheckDropPrize();
          }
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.ClawReturn)
        {
          if (this._claw.GetGrabbedPrize() != null)
          {
            if (!this._game.fastMusic.IsPlaying)
              this._game.fastMusic.Play();
          }
          else if (this._game.fastMusic.IsPlaying)
          {
            this._game.fastMusic.Stop(AudioStopOptions.AsAuthored);
            this._game.fastMusic = Game1.soundBank.GetCue("crane_game_fast");
          }
          if (this._stateTimer > 10)
          {
            if (this._claw.position.Equals(this._dropPosition))
            {
              this.SetState(CraneGame.GameLogic.GameStates.ClawRelease);
            }
            else
            {
              float delta = 0.5f;
              if (this._claw.GetGrabbedPrize() == null)
                delta = 0.75f;
              if ((double) this._claw.position.X != (double) this._dropPosition.X)
                this._claw.position.X = Utility.MoveTowards(this._claw.position.X, this._dropPosition.X, delta);
              if ((double) this._claw.position.X != (double) this._dropPosition.Y)
                this._claw.position.Y = Utility.MoveTowards(this._claw.position.Y, this._dropPosition.Y, delta);
            }
          }
          this._claw.CheckDropPrize();
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.ClawRelease)
        {
          bool flag = this._claw.GetGrabbedPrize() != null;
          if (this._stateTimer > 10)
          {
            this._claw.ReleaseGrabbedObject();
            if ((double) this._claw.openAngle < 40.0)
            {
              ++this._claw.openAngle;
            }
            else
            {
              this.SetState(CraneGame.GameLogic.GameStates.ClawReset);
              if (!flag)
              {
                Game1.playSound("button1");
                this._claw.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(1f, 1f));
              }
            }
          }
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.ClawReset)
        {
          if (this._stateTimer > 50)
          {
            if (this._claw.position.Equals(this._startPosition))
            {
              --this.lives;
              if (this.lives <= 0)
                this.SetState(CraneGame.GameLogic.GameStates.EndGame);
              else
                this.SetState(CraneGame.GameLogic.GameStates.Idle);
            }
            else
            {
              float delta = 0.5f;
              if ((double) this._claw.position.X != (double) this._startPosition.X)
                this._claw.position.X = Utility.MoveTowards(this._claw.position.X, this._startPosition.X, delta);
              if ((double) this._claw.position.X != (double) this._startPosition.Y)
                this._claw.position.Y = Utility.MoveTowards(this._claw.position.Y, this._startPosition.Y, delta);
            }
          }
        }
        else if (this._currentState == CraneGame.GameLogic.GameStates.EndGame)
        {
          if (this._game.music.IsPlaying)
            this._game.music.Stop(AudioStopOptions.Immediate);
          if (this._game.fastMusic.IsPlaying)
            this._game.fastMusic.Stop(AudioStopOptions.Immediate);
          bool flag = false;
          foreach (CraneGame.Prize prize in this._game.GetObjectsOfType<CraneGame.Prize>())
          {
            if (!prize.CanBeGrabbed())
            {
              flag = true;
              break;
            }
          }
          if (!flag && this._stateTimer >= 20)
          {
            if (this.collectedItems.Count > 0)
            {
              List<Item> inventory = new List<Item>();
              foreach (Item collectedItem in this.collectedItems)
                inventory.Add(collectedItem.getOne());
              Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) inventory, false, true, (InventoryMenu.highlightThisItem) null, (ItemGrabMenu.behaviorOnItemSelect) null, "Rewards", playRightClickSound: false, allowRightClick: false, context: ((object) this._game));
            }
            this._game.Quit();
          }
        }
        this.sunShockedFace.visible = this._claw.GetGrabbedPrize() != null;
        this.joystick.rotation = Utility.MoveTowards(this.joystick.rotation, to, 2f);
        ++this._stateTimer;
      }

      public override void Draw(SpriteBatch b, float layer_depth)
      {
      }

      public void SetState(CraneGame.GameLogic.GameStates new_state)
      {
        this._currentState = new_state;
        this._stateTimer = 0;
      }

      [XmlType("CraneGame.GameStates")]
      public enum GameStates
      {
        Setup,
        Idle,
        MoveClawRight,
        WaitForMoveDown,
        MoveClawDown,
        ClawDescend,
        ClawAscend,
        ClawReturn,
        ClawRelease,
        ClawReset,
        EndGame,
      }
    }

    public class Trampoline : CraneGame.CraneGameObject
    {
      public Trampoline(CraneGame game, int x, int y)
        : base(game)
      {
        this.SetSpriteFromIndex(30);
        this.spriteRect.Width = 32;
        this.spriteRect.Height = 32;
        this.spriteAnchor.X = 15f;
        this.spriteAnchor.Y = 15f;
        this.position.X = (float) x;
        this.position.Y = (float) y;
      }
    }

    public class Shadow : CraneGame.CraneGameObject
    {
      public CraneGame.CraneGameObject _target;

      public Shadow(CraneGame game, CraneGame.CraneGameObject target)
        : base(game)
      {
        this.SetSpriteFromIndex(2);
        this.layerDepth = 900f;
        this._target = target;
      }

      public override void Update(GameTime time)
      {
        if (this._target != null)
          this.position = this._target.position;
        if (this._target is CraneGame.Prize && (this._target as CraneGame.Prize).grabbed)
          this.visible = false;
        if (this._target.IsDestroyed())
        {
          this.Destroy();
        }
        else
        {
          this.color.A = (byte) ((double) Math.Min(1f, this._target.zPosition / 50f) * (double) byte.MaxValue);
          this.scale = Utility.Lerp(1f, 0.5f, Math.Min(this._target.zPosition / 100f, 1f)) * new Vector2(1f, 1f);
        }
      }
    }

    public class Claw : CraneGame.CraneGameObject
    {
      protected CraneGame.CraneGameObject _leftArm;
      protected CraneGame.CraneGameObject _rightArm;
      protected CraneGame.Prize _grabbedPrize;
      protected Vector2 _prizePositionOffset;
      protected int _nextDropCheckTimer;
      protected int _dropChances;
      protected int _grabTime;

      public float openAngle
      {
        get => this._leftArm.rotation;
        set => this._leftArm.rotation = value;
      }

      public Claw(CraneGame game)
        : base(game)
      {
        this.SetSpriteFromIndex();
        this.spriteAnchor = new Vector2(8f, 24f);
        this._leftArm = new CraneGame.CraneGameObject(game);
        this._leftArm.SetSpriteFromIndex(1);
        this._leftArm.spriteAnchor = new Vector2(16f, 0.0f);
        this._rightArm = new CraneGame.CraneGameObject(game);
        this._rightArm.SetSpriteFromIndex(1);
        this._rightArm.flipX = true;
        this._rightArm.spriteAnchor = new Vector2(0.0f, 0.0f);
        CraneGame.Shadow shadow = new CraneGame.Shadow(this._game, (CraneGame.CraneGameObject) this);
      }

      public void CheckDropPrize()
      {
        if (this._grabbedPrize == null)
          return;
        --this._nextDropCheckTimer;
        if (this._nextDropCheckTimer > 0)
          return;
        float num1 = this._prizePositionOffset.Length() * 0.1f + this.zPosition * (1f / 1000f);
        if (this._grabbedPrize.isLargeItem)
          num1 += 0.1f;
        double num2 = Game1.random.NextDouble();
        if (num2 < (double) num1)
        {
          --this._dropChances;
          if (this._dropChances <= 0)
          {
            Game1.playSound("fishEscape");
            this.ReleaseGrabbedObject();
          }
          else
          {
            Game1.playSound("bob");
            this._grabbedPrize.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(2f, 2f, 50));
            this._grabbedPrize.rotation += (float) Game1.random.NextDouble() * 10f;
          }
        }
        else if (num2 < (double) num1)
        {
          Game1.playSound("dwop");
          this._grabbedPrize.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(1f, 1f, 50));
        }
        this._nextDropCheckTimer = Game1.random.Next(50, 100);
      }

      public void ApplyDrawEffectToArms(CraneGame.DrawEffect new_effect)
      {
        this._leftArm.ApplyDrawEffect(new_effect);
        this._rightArm.ApplyDrawEffect(new_effect);
      }

      public void ReleaseGrabbedObject()
      {
        if (this._grabbedPrize == null)
          return;
        this._grabbedPrize.grabbed = false;
        this._grabbedPrize.OnDrop();
        this._grabbedPrize = (CraneGame.Prize) null;
      }

      public void GrabObject()
      {
        CraneGame.Prize prize1 = (CraneGame.Prize) null;
        float num1 = 0.0f;
        foreach (CraneGame.Prize prize2 in this._game.GetObjectsAtPoint<CraneGame.Prize>(this.position))
        {
          if (!prize2.IsDestroyed() && prize2.CanBeGrabbed())
          {
            float num2 = (this.position - prize2.position).LengthSquared();
            if (prize1 == null || (double) num2 < (double) num1)
            {
              num1 = num2;
              prize1 = prize2;
            }
          }
        }
        if (prize1 == null)
          return;
        this._grabbedPrize = prize1;
        this._grabbedPrize.grabbed = true;
        this._prizePositionOffset = this._grabbedPrize.position - this.position;
        this._nextDropCheckTimer = Game1.random.Next(50, 100);
        this._dropChances = 3;
        Game1.playSound("pickUpItem");
        this._grabTime = 0;
        this._grabbedPrize.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.StretchEffect(0.95f, 1.1f));
        this._grabbedPrize.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(1f, 1f, 20));
      }

      public override void Move(float x, float y) => base.Move(x, y);

      public CraneGame.Prize GetGrabbedPrize() => this._grabbedPrize;

      public override void Update(GameTime time)
      {
        this._leftArm.position = this.position + new Vector2(0.0f, -16f);
        this._rightArm.position = this.position + new Vector2(0.0f, -16f);
        this._rightArm.rotation = -this._leftArm.rotation;
        this._leftArm.layerDepth = this._rightArm.layerDepth = this.GetRendererLayerDepth() + 0.01f;
        this._leftArm.zPosition = this._rightArm.zPosition = this.zPosition;
        if (this._grabbedPrize != null)
        {
          this._grabbedPrize.position = this.position + this._prizePositionOffset * Utility.Lerp(1f, 0.25f, Math.Min(1f, (float) this._grabTime / 200f));
          this._grabbedPrize.zPosition = this.zPosition + this._grabbedPrize.GetRestingZPosition();
        }
        ++this._grabTime;
      }

      public override void Destroy()
      {
        this._leftArm.Destroy();
        this._rightArm.Destroy();
        base.Destroy();
      }
    }

    public class ConveyerBelt : CraneGame.CraneGameObject
    {
      protected int _direction;
      protected Vector2 _spriteStartPosition;
      protected int _spriteOffset;

      public int GetDirection() => this._direction;

      public ConveyerBelt(CraneGame game, int x, int y, int direction)
        : base(game)
      {
        this.position.X = (float) (x * 16);
        this.position.Y = (float) (y * 16);
        this._direction = direction;
        this.spriteAnchor = Vector2.Zero;
        this.layerDepth = 1000f;
        if (this._direction == 0)
          this.SetSpriteFromIndex(5);
        else if (this._direction == 2)
          this.SetSpriteFromIndex(10);
        else if (this._direction == 3)
          this.SetSpriteFromIndex(15);
        else if (this._direction == 1)
          this.SetSpriteFromIndex(20);
        this._spriteStartPosition = new Vector2((float) this.spriteRect.X, (float) this.spriteRect.Y);
      }

      public void SetSpriteFromCorner(int x, int y)
      {
        this.spriteRect.X = x;
        this.spriteRect.Y = y;
        this._spriteStartPosition = new Vector2((float) this.spriteRect.X, (float) this.spriteRect.Y);
      }

      public override void Update(GameTime time)
      {
        int num1 = 4;
        int num2 = 4;
        this.spriteRect.X = (int) this._spriteStartPosition.X + this._spriteOffset / num1 * 16;
        ++this._spriteOffset;
        if (this._spriteOffset < (num2 - 1) * num1)
          return;
        this._spriteOffset = 0;
      }
    }

    public class Bush : CraneGame.CraneGameObject
    {
      public Bush(CraneGame game, int tile_index, int tile_width, int tile_height, int x, int y)
        : base(game)
      {
        this.SetSpriteFromIndex(tile_index);
        this.spriteRect.Width = tile_width * 16;
        this.spriteRect.Height = tile_height * 16;
        this.spriteAnchor.X = (float) this.spriteRect.Width / 2f;
        this.spriteAnchor.Y = (float) this.spriteRect.Height;
        if (tile_height > 16)
          this.spriteAnchor.Y -= 8f;
        else
          this.spriteAnchor.Y -= 4f;
        this.position.X = (float) x;
        this.position.Y = (float) y;
      }

      public override void Update(GameTime time) => this.rotation = (float) Math.Sin(time.TotalGameTime.TotalMilliseconds * (1.0 / 400.0) + (double) this.position.Y + (double) this.position.X * 2.0) * 2f;
    }

    public class Prize : CraneGame.CraneGameObject
    {
      protected Vector2 _conveyerBeltMove;
      public bool grabbed;
      public float gravity;
      protected Vector2 _velocity = Vector2.Zero;
      protected Item _item;
      protected float _restingZPosition;
      protected float _angularSpeed;
      protected bool _isBeingCollected;
      public bool isLargeItem;

      public float GetRestingZPosition() => this._restingZPosition;

      public Prize(CraneGame game, Item item)
        : base(game)
      {
        this.SetSpriteFromIndex(3);
        this.spriteAnchor = new Vector2(8f, 12f);
        this._item = item;
        this._UpdateItemSprite();
        CraneGame.Shadow shadow = new CraneGame.Shadow(this._game, (CraneGame.CraneGameObject) this);
      }

      public void OnDrop()
      {
        if (!this.isLargeItem)
          this._angularSpeed = Utility.Lerp(-5f, 5f, (float) Game1.random.NextDouble());
        else
          this.rotation = 0.0f;
      }

      public void _UpdateItemSprite()
      {
        if (this._item is Furniture)
        {
          this.texture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\furniture");
          this.spriteRect = (this._item as Furniture).defaultSourceRect.Value;
        }
        else if (this._item is StardewValley.Object)
        {
          StardewValley.Object @object = this._item as StardewValley.Object;
          if ((bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable)
          {
            this.texture = Game1.bigCraftableSpriteSheet;
            this.spriteRect = StardewValley.Object.getSourceRectForBigCraftable(@object.ParentSheetIndex);
          }
          else
          {
            this.texture = Game1.objectSpriteSheet;
            this.spriteRect = GameLocation.getSourceRectForObject(@object.ParentSheetIndex);
          }
        }
        this.width = this.spriteRect.Width;
        this.height = this.spriteRect.Height;
        this.isLargeItem = this.width > 16 || this.height > 16;
        if (this.height <= 16)
          this.spriteAnchor = new Vector2((float) (this.width / 2), (float) this.height - 4f);
        else
          this.spriteAnchor = new Vector2((float) (this.width / 2), (float) this.height - 8f);
        this._restingZPosition = 0.0f;
      }

      public bool CanBeGrabbed() => !this.IsDestroyed() && !this._isBeingCollected && (double) this.zPosition == (double) this._restingZPosition;

      public override void Update(GameTime time)
      {
        if (this._isBeingCollected)
        {
          Vector4 vector4 = this.color.ToVector4();
          vector4.X = Utility.MoveTowards(vector4.X, 0.0f, 0.05f);
          vector4.Y = Utility.MoveTowards(vector4.Y, 0.0f, 0.05f);
          vector4.Z = Utility.MoveTowards(vector4.Z, 0.0f, 0.05f);
          vector4.W = Utility.MoveTowards(vector4.W, 0.0f, 0.05f);
          this.color = new Color(vector4);
          this.scale.X = Utility.MoveTowards(this.scale.X, 0.5f, 0.05f);
          this.scale.Y = Utility.MoveTowards(this.scale.Y, 0.5f, 0.05f);
          if ((double) vector4.W == 0.0)
          {
            Game1.playSound("Ship");
            this.Destroy();
          }
          this.position.Y += 0.5f;
        }
        else
        {
          if (this.grabbed)
            return;
          if ((double) this._velocity.X != 0.0 || (double) this._velocity.Y != 0.0)
          {
            this.position.X += this._velocity.X;
            if (!this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].playArea.Contains(new Point((int) this.position.X, (int) this.position.Y)))
            {
              this.position.X -= this._velocity.X;
              this._velocity.X *= -1f;
            }
            this.position.Y += this._velocity.Y;
            if (!this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].playArea.Contains(new Point((int) this.position.X, (int) this.position.Y)))
            {
              this.position.Y -= this._velocity.Y;
              this._velocity.Y *= -1f;
            }
          }
          if ((double) this.zPosition < (double) this._restingZPosition)
            this.zPosition = this._restingZPosition;
          if ((double) this.zPosition > (double) this._restingZPosition || this._velocity != Vector2.Zero || (double) this.gravity != 0.0)
          {
            if (!this.isLargeItem)
              this.rotation += this._angularSpeed;
            this._conveyerBeltMove = Vector2.Zero;
            if ((double) this.zPosition > (double) this._restingZPosition)
              this.gravity += 0.1f;
            this.zPosition -= this.gravity;
            if ((double) this.zPosition >= (double) this._restingZPosition)
              return;
            this.zPosition = this._restingZPosition;
            if ((double) this.gravity < 0.0)
              return;
            if (!this.isLargeItem)
              this._angularSpeed = Utility.Lerp(-10f, 10f, (float) Game1.random.NextDouble());
            this.gravity = (float) (-(double) this.gravity * 0.600000023841858);
            if (this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].prizeChute.Contains(new Point((int) this.position.X, (int) this.position.Y)))
            {
              if (this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].GetCurrentState() != CraneGame.GameLogic.GameStates.Setup)
              {
                Game1.playSound("reward");
                this._isBeingCollected = true;
                this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].collectedItems.Add(this._item);
              }
              else
              {
                this.gravity = -2.5f;
                Vector2 vector2 = new Vector2((float) this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].playArea.Center.X, (float) this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].playArea.Center.Y) - new Vector2(this.position.X, this.position.Y);
                vector2.Normalize();
                this._velocity = vector2 * Utility.Lerp(1f, 2f, (float) Game1.random.NextDouble());
              }
            }
            else if (this._game.GetOverlaps<CraneGame.Trampoline>((CraneGame.CraneGameObject) this, 1).Count > 0)
            {
              CraneGame.Trampoline overlap = this._game.GetOverlaps<CraneGame.Trampoline>((CraneGame.CraneGameObject) this, 1)[0];
              Game1.playSound("axchop");
              overlap.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.StretchEffect(0.75f, 0.75f, 5));
              overlap.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(2f, 2f));
              this.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(2f, 2f));
              this.gravity = -2.5f;
              Vector2 vector2 = new Vector2((float) this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].playArea.Center.X, (float) this._game.GetObjectsOfType<CraneGame.GameLogic>()[0].playArea.Center.Y) - new Vector2(this.position.X, this.position.Y);
              vector2.Normalize();
              this._velocity = vector2 * Utility.Lerp(0.5f, 1f, (float) Game1.random.NextDouble());
            }
            else if ((double) Math.Abs(this.gravity) < 1.5)
            {
              this.rotation = 0.0f;
              this._velocity = Vector2.Zero;
              this.gravity = 0.0f;
            }
            else
            {
              bool flag = false;
              foreach (CraneGame.Prize overlap in this._game.GetOverlaps<CraneGame.Prize>((CraneGame.CraneGameObject) this))
              {
                if ((double) overlap.gravity == 0.0 && overlap.CanBeGrabbed())
                {
                  Vector2 vector2 = this.position - overlap.position;
                  vector2.Normalize();
                  this._velocity = vector2 * Utility.Lerp(0.25f, 1f, (float) Game1.random.NextDouble());
                  if (!overlap.isLargeItem || this.isLargeItem)
                  {
                    overlap._velocity = -vector2 * Utility.Lerp(0.75f, 1.5f, (float) Game1.random.NextDouble());
                    overlap.gravity = this.gravity * 0.75f;
                    overlap.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(2f, 2f, 20));
                  }
                  flag = true;
                }
              }
              this.ApplyDrawEffect((CraneGame.DrawEffect) new CraneGame.ShakeEffect(2f, 2f, 20));
              if (flag)
                return;
              float num = Utility.Lerp(0.0f, 6.283185f, (float) Game1.random.NextDouble());
              this._velocity = new Vector2((float) Math.Sin((double) num), (float) Math.Cos((double) num)) * Utility.Lerp(0.5f, 1f, (float) Game1.random.NextDouble());
            }
          }
          else if ((double) this._conveyerBeltMove.X == 0.0 && (double) this._conveyerBeltMove.Y == 0.0)
          {
            List<CraneGame.ConveyerBelt> objectsAtPoint = this._game.GetObjectsAtPoint<CraneGame.ConveyerBelt>(this.position, 1);
            if (objectsAtPoint.Count <= 0)
              return;
            switch (objectsAtPoint[0].GetDirection())
            {
              case 0:
                this._conveyerBeltMove = new Vector2(0.0f, -16f);
                break;
              case 1:
                this._conveyerBeltMove = new Vector2(16f, 0.0f);
                break;
              case 2:
                this._conveyerBeltMove = new Vector2(0.0f, 16f);
                break;
              case 3:
                this._conveyerBeltMove = new Vector2(-16f, 0.0f);
                break;
            }
          }
          else
          {
            float delta = 0.3f;
            if ((double) this._conveyerBeltMove.X != 0.0)
            {
              this.Move(delta * (float) Math.Sign(this._conveyerBeltMove.X), 0.0f);
              this._conveyerBeltMove.X = Utility.MoveTowards(this._conveyerBeltMove.X, 0.0f, delta);
            }
            if ((double) this._conveyerBeltMove.Y == 0.0)
              return;
            this.Move(0.0f, delta * (float) Math.Sign(this._conveyerBeltMove.Y));
            this._conveyerBeltMove.Y = Utility.MoveTowards(this._conveyerBeltMove.Y, 0.0f, delta);
          }
        }
      }
    }

    public class CraneGameObject
    {
      protected CraneGame _game;
      public Vector2 position = Vector2.Zero;
      public float rotation;
      public Vector2 scale = new Vector2(1f, 1f);
      public bool flipX;
      public bool flipY;
      public Rectangle spriteRect;
      public Texture2D texture;
      public Vector2 spriteAnchor;
      public Color color = Color.White;
      public float layerDepth = -1f;
      public int width = 16;
      public int height = 16;
      public float zPosition;
      public bool visible = true;
      public List<CraneGame.DrawEffect> drawEffects;
      protected bool _destroyed;

      public CraneGameObject(CraneGame game)
      {
        this._game = game;
        this.texture = this._game.spriteSheet;
        this.spriteRect = new Rectangle(0, 0, 16, 16);
        this.spriteAnchor = new Vector2(8f, 8f);
        this.drawEffects = new List<CraneGame.DrawEffect>();
        this._game.RegisterGameObject(this);
      }

      public void SetSpriteFromIndex(int index = 0)
      {
        this.spriteRect.X = 304 + index % 5 * 16;
        this.spriteRect.Y = index / 5 * 16;
      }

      public bool IsDestroyed() => this._destroyed;

      public virtual void Destroy()
      {
        this._destroyed = true;
        this._game.UnregisterGameObject(this);
      }

      public virtual void Move(float x, float y)
      {
        this.position.X += x;
        this.position.Y += y;
      }

      public Rectangle GetBounds() => new Rectangle((int) ((double) this.position.X - (double) this.spriteAnchor.X), (int) ((double) this.position.Y - (double) this.spriteAnchor.Y), this.width, this.height);

      public virtual void Update(GameTime time)
      {
      }

      public float GetRendererLayerDepth()
      {
        float rendererLayerDepth = this.layerDepth;
        if ((double) rendererLayerDepth < 0.0)
          rendererLayerDepth = (float) this._game.gameHeight - this.position.Y;
        return rendererLayerDepth;
      }

      public void ApplyDrawEffect(CraneGame.DrawEffect new_effect) => this.drawEffects.Add(new_effect);

      public virtual void Draw(SpriteBatch b, float layer_depth)
      {
        if (!this.visible)
          return;
        SpriteEffects effects = SpriteEffects.None;
        if (this.flipX)
          effects |= SpriteEffects.FlipHorizontally;
        if (this.flipY)
          effects |= SpriteEffects.FlipVertically;
        float rotation = this.rotation;
        Vector2 scale = this.scale;
        Vector2 position = this.position - new Vector2(0.0f, this.zPosition);
        for (int index = 0; index < this.drawEffects.Count; ++index)
        {
          if (this.drawEffects[index].Apply(ref position, ref rotation, ref scale))
          {
            this.drawEffects.RemoveAt(index);
            --index;
          }
        }
        b.Draw(this.texture, this._game.upperLeft + position * 4f, new Rectangle?(this.spriteRect), this.color, rotation * ((float) Math.PI / 180f), this.spriteAnchor, 4f * scale, effects, layer_depth);
      }
    }

    public class SwayEffect : CraneGame.DrawEffect
    {
      public float swayMagnitude;
      public float swaySpeed;
      public int swayDuration = 1;
      public int age;

      public SwayEffect(float magnitude, float speed = 1f, int sway_duration = 10)
      {
        this.swayMagnitude = magnitude;
        this.swaySpeed = speed;
        this.swayDuration = sway_duration;
        this.age = 0;
      }

      public override bool Apply(ref Vector2 position, ref float rotation, ref Vector2 scale)
      {
        if (this.age > this.swayDuration)
          return true;
        float num = (float) this.age / (float) this.swayDuration;
        rotation += (float) (Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 1000.0 * 360.0 * (double) this.swaySpeed * (Math.PI / 180.0)) * (1.0 - (double) num)) * this.swayMagnitude;
        ++this.age;
        return false;
      }
    }

    public class ShakeEffect : CraneGame.DrawEffect
    {
      public Vector2 shakeAmount;
      public int shakeDuration = 1;
      public int age;

      public ShakeEffect(float shake_x, float shake_y, int shake_duration = 10)
      {
        this.shakeAmount = new Vector2(shake_x, shake_y);
        this.shakeDuration = shake_duration;
        this.age = 0;
      }

      public override bool Apply(ref Vector2 position, ref float rotation, ref Vector2 scale)
      {
        if (this.age > this.shakeDuration)
          return true;
        float t = (float) this.age / (float) this.shakeDuration;
        Vector2 vector2 = new Vector2(Utility.Lerp(this.shakeAmount.X, 1f, t), Utility.Lerp(this.shakeAmount.Y, 1f, t));
        position += new Vector2((float) (Game1.random.NextDouble() - 0.5) * 2f * vector2.X, (float) (Game1.random.NextDouble() - 0.5) * 2f * vector2.Y);
        ++this.age;
        return false;
      }
    }

    public class StretchEffect : CraneGame.DrawEffect
    {
      public Vector2 stretchScale;
      public int stretchDuration = 1;
      public int age;

      public StretchEffect(float x_scale, float y_scale, int stretch_duration = 10)
      {
        this.stretchScale = new Vector2(x_scale, y_scale);
        this.stretchDuration = stretch_duration;
        this.age = 0;
      }

      public override bool Apply(ref Vector2 position, ref float rotation, ref Vector2 scale)
      {
        if (this.age > this.stretchDuration)
          return true;
        float t = (float) this.age / (float) this.stretchDuration;
        Vector2 vector2 = new Vector2(Utility.Lerp(this.stretchScale.X, 1f, t), Utility.Lerp(this.stretchScale.Y, 1f, t));
        scale *= vector2;
        ++this.age;
        return false;
      }
    }

    public class DrawEffect
    {
      public virtual bool Apply(ref Vector2 position, ref float rotation, ref Vector2 scale) => true;
    }
  }
}
