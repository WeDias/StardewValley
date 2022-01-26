// Decompiled with JetBrains decompiler
// Type: StardewValley.Character
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley
{
  [InstanceStatics]
  public class Character : INetObject<NetFields>
  {
    public const float emoteBeginInterval = 20f;
    public const float emoteNormalInterval = 250f;
    public const int emptyCanEmote = 4;
    public const int questionMarkEmote = 8;
    public const int angryEmote = 12;
    public const int exclamationEmote = 16;
    public const int heartEmote = 20;
    public const int sleepEmote = 24;
    public const int sadEmote = 28;
    public const int happyEmote = 32;
    public const int xEmote = 36;
    public const int pauseEmote = 40;
    public const int videoGameEmote = 52;
    public const int musicNoteEmote = 56;
    public const int blushEmote = 60;
    public const int blockedIntervalBeforeEmote = 3000;
    public const int blockedIntervalBeforeSprint = 5000;
    public const double chanceForSound = 0.001;
    [XmlIgnore]
    public readonly NetRef<AnimatedSprite> sprite = new NetRef<AnimatedSprite>();
    [XmlIgnore]
    public readonly NetPosition position = new NetPosition();
    [XmlIgnore]
    private readonly NetInt netSpeed = new NetInt();
    [XmlIgnore]
    private readonly NetInt netAddedSpeed = new NetInt();
    [XmlIgnore]
    public readonly NetDirection facingDirection = new NetDirection(2);
    [XmlIgnore]
    public int blockedInterval;
    [XmlIgnore]
    public int faceTowardFarmerTimer;
    [XmlIgnore]
    public int forceUpdateTimer;
    [XmlIgnore]
    public int movementPause;
    [XmlIgnore]
    public NetEvent1Field<int, NetInt> faceTowardFarmerEvent = new NetEvent1Field<int, NetInt>();
    [XmlIgnore]
    public readonly NetInt faceTowardFarmerRadius = new NetInt();
    [XmlElement("name")]
    public readonly NetString name = new NetString();
    [XmlElement("forceOneTileWide")]
    public readonly NetBool forceOneTileWide = new NetBool(false);
    protected bool moveUp;
    protected bool moveRight;
    protected bool moveDown;
    protected bool moveLeft;
    protected bool freezeMotion;
    [XmlIgnore]
    private string _displayName;
    public bool isEmoting;
    public bool isCharging;
    public bool isGlowing;
    public bool coloredBorder;
    public bool flip;
    public bool drawOnTop;
    public bool faceTowardFarmer;
    public bool ignoreMovementAnimation;
    [XmlIgnore]
    public bool hasJustStartedFacingPlayer;
    [XmlElement("faceAwayFromFarmer")]
    public readonly NetBool faceAwayFromFarmer = new NetBool();
    protected int currentEmote;
    protected int currentEmoteFrame;
    protected readonly NetInt facingDirectionBeforeSpeakingToPlayer = new NetInt(-1);
    [XmlIgnore]
    public float emoteInterval;
    [XmlIgnore]
    public float xVelocity;
    [XmlIgnore]
    public float yVelocity;
    [XmlIgnore]
    public Vector2 lastClick = Vector2.Zero;
    public readonly NetFloat scale = new NetFloat(1f);
    public float timeBeforeAIMovementAgain;
    public float glowingTransparency;
    public float glowRate;
    private bool glowUp;
    [XmlIgnore]
    public readonly NetBool swimming = new NetBool();
    [XmlIgnore]
    public bool nextEventcommandAfterEmote;
    [XmlIgnore]
    public bool eventActor;
    [XmlIgnore]
    public bool farmerPassesThrough;
    [XmlIgnore]
    public readonly NetBool collidesWithOtherCharacters = new NetBool();
    protected bool ignoreMovementAnimations;
    [XmlIgnore]
    public int yJumpOffset;
    [XmlIgnore]
    public int ySourceRectOffset;
    [XmlIgnore]
    public float yJumpVelocity;
    [XmlIgnore]
    public float yJumpGravity = -0.5f;
    [XmlIgnore]
    public bool wasJumpWithSound;
    [XmlIgnore]
    private readonly NetFarmerRef whoToFace = new NetFarmerRef();
    [XmlIgnore]
    public Color glowingColor;
    [XmlIgnore]
    public PathFindController controller;
    private bool emoteFading;
    [XmlIgnore]
    private readonly NetBool _willDestroyObjectsUnderfoot = new NetBool(true);
    [XmlIgnore]
    protected readonly NetLocationRef currentLocationRef = new NetLocationRef();
    /// <summary>
    /// Used for modders to store metadata to this object. This data is synchronized in multiplayer and saved to the save data.
    /// </summary>
    [XmlIgnore]
    public ModDataDictionary modData = new ModDataDictionary();
    private Microsoft.Xna.Framework.Rectangle originalSourceRect;
    public static readonly Vector2[] AdjacentTilesOffsets = new Vector2[4]
    {
      new Vector2(1f, 0.0f),
      new Vector2(-1f, 0.0f),
      new Vector2(0.0f, -1f),
      new Vector2(0.0f, 1f)
    };
    [XmlIgnore]
    public readonly NetVector2 drawOffset = new NetVector2(Vector2.Zero);
    [XmlIgnore]
    public bool shouldShadowBeOffset;

    [XmlIgnore]
    public int speed
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netSpeed;
      set => this.netSpeed.Value = value;
    }

    [XmlIgnore]
    public int addedSpeed
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netAddedSpeed;
      set => this.netAddedSpeed.Value = value;
    }

    [XmlIgnore]
    public virtual string displayName
    {
      get => this._displayName ?? (this._displayName = this.translateName((string) (NetFieldBase<string, NetString>) this.name));
      set => this._displayName = value;
    }

    public bool willDestroyObjectsUnderfoot
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this._willDestroyObjectsUnderfoot;
      set => this._willDestroyObjectsUnderfoot.Value = value;
    }

    public Vector2 Position
    {
      get => (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position;
      set
      {
        if (!(this.position.Value != value))
          return;
        this.position.Set(value);
      }
    }

    public int Speed
    {
      get => this.speed;
      set => this.speed = value;
    }

    public virtual int FacingDirection
    {
      get => (int) this.facingDirection;
      set => this.facingDirection.Set(value);
    }

    [XmlIgnore]
    public string Name
    {
      get => (string) (NetFieldBase<string, NetString>) this.name;
      set => this.name.Set(value);
    }

    [XmlIgnore]
    public virtual AnimatedSprite Sprite
    {
      get => this.sprite.Value;
      set => this.sprite.Value = value;
    }

    public bool IsEmoting
    {
      get => this.isEmoting;
      set => this.isEmoting = value;
    }

    public int CurrentEmote
    {
      get => this.currentEmote;
      set => this.currentEmote = value;
    }

    public int CurrentEmoteIndex => this.currentEmoteFrame;

    public virtual bool IsMonster => false;

    public float Scale
    {
      get => (float) (NetFieldBase<float, NetFloat>) this.scale;
      set => this.scale.Value = value;
    }

    [XmlIgnore]
    public GameLocation currentLocation
    {
      get => this.currentLocationRef.Value;
      set => this.currentLocationRef.Value = value;
    }

    /// <summary>Get the mod populated metadata as it will be serialized for game saving. Identical to <see cref="F:StardewValley.Character.modData" /> except returns null during save if it is empty. It is strongly recommended to use <see cref="F:StardewValley.Character.modData" /> instead.</summary>
    [XmlElement("modData")]
    public ModDataDictionary modDataForSerialization
    {
      get => this.modData.GetForSerialization();
      set => this.modData.SetFromSerialization(value);
    }

    public NetFields NetFields { get; } = new NetFields();

    public Character() => this.initNetFields();

    protected virtual void initNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.sprite, (INetSerializable) this.position.NetFields, (INetSerializable) this.facingDirection, (INetSerializable) this.netSpeed, (INetSerializable) this.netAddedSpeed, (INetSerializable) this.name, (INetSerializable) this.scale, (INetSerializable) this.currentLocationRef.NetFields, (INetSerializable) this.swimming, (INetSerializable) this.collidesWithOtherCharacters, (INetSerializable) this.facingDirectionBeforeSpeakingToPlayer, (INetSerializable) this.faceTowardFarmerRadius, (INetSerializable) this.faceAwayFromFarmer, (INetSerializable) this.whoToFace.NetFields, (INetSerializable) this.faceTowardFarmerEvent, (INetSerializable) this._willDestroyObjectsUnderfoot, (INetSerializable) this.forceOneTileWide);
      this.facingDirection.Position = this.position;
      this.faceTowardFarmerEvent.onEvent += new AbstractNetEvent1<int>.Event(this.performFaceTowardFarmerEvent);
      this.NetFields.AddField((INetSerializable) this.modData);
    }

    public Character(AnimatedSprite sprite, Vector2 position, int speed, string name)
      : this()
    {
      this.Sprite = sprite;
      this.Position = position;
      this.speed = speed;
      this.Name = name;
      if (sprite == null)
        return;
      this.originalSourceRect = sprite.SourceRect;
    }

    protected virtual string translateName(string name) => name;

    public virtual void SetMovingUp(bool b)
    {
      this.moveUp = b;
      if (b)
        return;
      this.Halt();
    }

    public virtual void SetMovingRight(bool b)
    {
      this.moveRight = b;
      if (b)
        return;
      this.Halt();
    }

    public virtual void SetMovingDown(bool b)
    {
      this.moveDown = b;
      if (b)
        return;
      this.Halt();
    }

    public virtual void SetMovingLeft(bool b)
    {
      this.moveLeft = b;
      if (b)
        return;
      this.Halt();
    }

    public void setMovingInFacingDirection()
    {
      switch (this.FacingDirection)
      {
        case 0:
          this.SetMovingUp(true);
          break;
        case 1:
          this.SetMovingRight(true);
          break;
        case 2:
          this.SetMovingDown(true);
          break;
        case 3:
          this.SetMovingLeft(true);
          break;
      }
    }

    public int getFacingDirection()
    {
      if (this.Sprite.currentFrame < 4)
        return 2;
      if (this.Sprite.currentFrame < 8)
        return 1;
      return this.Sprite.currentFrame < 12 ? 0 : 3;
    }

    public void setTrajectory(int xVelocity, int yVelocity) => this.setTrajectory(new Vector2((float) xVelocity, (float) yVelocity));

    public virtual void setTrajectory(Vector2 trajectory)
    {
      this.xVelocity = trajectory.X;
      this.yVelocity = trajectory.Y;
    }

    public virtual void Halt()
    {
      this.moveUp = false;
      this.moveDown = false;
      this.moveRight = false;
      this.moveLeft = false;
      this.Sprite.StopAnimation();
    }

    public void extendSourceRect(int horizontal, int vertical, bool ignoreSourceRectUpdates = true)
    {
      this.Sprite.sourceRect.Inflate(Math.Abs(horizontal) / 2, Math.Abs(vertical) / 2);
      this.Sprite.sourceRect.Offset(horizontal / 2, vertical / 2);
      Microsoft.Xna.Framework.Rectangle originalSourceRect = this.originalSourceRect;
      if (this.Sprite.SourceRect.Equals(this.originalSourceRect))
        this.Sprite.ignoreSourceRectUpdates = false;
      else
        this.Sprite.ignoreSourceRectUpdates = ignoreSourceRectUpdates;
    }

    public virtual bool collideWith(Object o) => true;

    public virtual void faceDirection(int direction)
    {
      if (direction != -3)
      {
        this.FacingDirection = direction;
        if (this.Sprite != null)
          this.Sprite.faceDirection(direction);
        this.faceTowardFarmer = false;
      }
      else
        this.faceTowardFarmer = true;
    }

    public int getDirection()
    {
      if (this.moveUp)
        return 0;
      if (this.moveRight)
        return 1;
      if (this.moveDown)
        return 2;
      if (this.moveLeft)
        return 3;
      return this.IsRemoteMoving() ? (int) this.facingDirection : -1;
    }

    public bool IsRemoteMoving()
    {
      if (!LocalMultiplayer.IsLocalMultiplayer(true))
        return this.position.Field.IsInterpolating();
      return this.position.moving.Value || this.position.Field.IsInterpolating();
    }

    public void tryToMoveInDirection(int direction, bool isFarmer, int damagesFarmer, bool glider)
    {
      if (this.currentLocation.isCollidingPosition(this.nextPosition(direction), Game1.viewport, isFarmer, damagesFarmer, glider, this))
        return;
      switch (direction)
      {
        case 0:
          this.position.Y -= (float) (this.speed + this.addedSpeed);
          break;
        case 1:
          this.position.X += (float) (this.speed + this.addedSpeed);
          break;
        case 2:
          this.position.Y += (float) (this.speed + this.addedSpeed);
          break;
        case 3:
          this.position.X -= (float) (this.speed + this.addedSpeed);
          break;
      }
    }

    public virtual Vector2 GetShadowOffset() => this.shouldShadowBeOffset ? (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawOffset : Vector2.Zero;

    public virtual bool shouldCollideWithBuildingLayer(GameLocation location) => this.controller == null && !this.IsMonster;

    protected void applyVelocity(GameLocation currentLocation)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      boundingBox.X += (int) this.xVelocity;
      boundingBox.Y -= (int) this.yVelocity;
      if (currentLocation == null || !currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, this))
      {
        this.position.X += this.xVelocity;
        this.position.Y -= this.yVelocity;
      }
      this.xVelocity = (float) (int) ((double) this.xVelocity - (double) this.xVelocity / 2.0);
      this.yVelocity = (float) (int) ((double) this.yVelocity - (double) this.yVelocity / 2.0);
    }

    public virtual void MovePosition(
      GameTime time,
      xTile.Dimensions.Rectangle viewport,
      GameLocation currentLocation)
    {
      if (this is FarmAnimal)
        this.willDestroyObjectsUnderfoot = false;
      bool flag = this.willDestroyObjectsUnderfoot;
      if (this.controller != null && this.controller.nonDestructivePathing)
        flag = false;
      if ((double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
        this.applyVelocity(currentLocation);
      else if (this.moveUp)
      {
        if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, 0, false, this) || this.isCharging)
        {
          this.position.Y -= (float) (this.speed + this.addedSpeed);
          if (!this.ignoreMovementAnimation)
          {
            this.Sprite.AnimateUp(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
            this.faceDirection(0);
          }
        }
        else if (!currentLocation.isTilePassable(this.nextPosition(0), viewport) || !flag)
          this.Halt();
        else if (flag)
        {
          Vector2 vector2 = new Vector2((float) (this.getStandingX() / 64), (float) (this.getStandingY() / 64 - 1));
          if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(0), true))
          {
            this.doEmote(12);
            this.position.Y -= (float) (this.speed + this.addedSpeed);
          }
          else
            this.blockedInterval += time.ElapsedGameTime.Milliseconds;
        }
      }
      else if (this.moveRight)
      {
        if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, 0, false, this) || this.isCharging)
        {
          this.position.X += (float) (this.speed + this.addedSpeed);
          if (!this.ignoreMovementAnimation)
          {
            this.Sprite.AnimateRight(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
            this.faceDirection(1);
          }
        }
        else if (!currentLocation.isTilePassable(this.nextPosition(1), viewport) || !flag)
          this.Halt();
        else if (flag)
        {
          Vector2 vector2 = new Vector2((float) (this.getStandingX() / 64 + 1), (float) (this.getStandingY() / 64));
          if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(1), true))
          {
            this.doEmote(12);
            this.position.X += (float) (this.speed + this.addedSpeed);
          }
          else
            this.blockedInterval += time.ElapsedGameTime.Milliseconds;
        }
      }
      else if (this.moveDown)
      {
        if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, 0, false, this) || this.isCharging)
        {
          this.position.Y += (float) (this.speed + this.addedSpeed);
          if (!this.ignoreMovementAnimation)
          {
            this.Sprite.AnimateDown(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
            this.faceDirection(2);
          }
        }
        else if (!currentLocation.isTilePassable(this.nextPosition(2), viewport) || !flag)
          this.Halt();
        else if (flag)
        {
          Vector2 vector2 = new Vector2((float) (this.getStandingX() / 64), (float) (this.getStandingY() / 64 + 1));
          if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(2), true))
          {
            this.doEmote(12);
            this.position.Y += (float) (this.speed + this.addedSpeed);
          }
          else
            this.blockedInterval += time.ElapsedGameTime.Milliseconds;
        }
      }
      else if (this.moveLeft)
      {
        if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, 0, false, this) || this.isCharging)
        {
          this.position.X -= (float) (this.speed + this.addedSpeed);
          if (!this.ignoreMovementAnimation)
          {
            this.Sprite.AnimateLeft(time, (this.speed - 2 + this.addedSpeed) * -25, Utility.isOnScreen(this.getTileLocationPoint(), 1, currentLocation) ? "Cowboy_Footstep" : "");
            this.faceDirection(3);
          }
        }
        else if (!currentLocation.isTilePassable(this.nextPosition(3), viewport) || !flag)
          this.Halt();
        else if (flag)
        {
          Vector2 vector2 = new Vector2((float) (this.getStandingX() / 64 - 1), (float) (this.getStandingY() / 64));
          if (currentLocation.characterDestroyObjectWithinRectangle(this.nextPosition(3), true))
          {
            this.doEmote(12);
            this.position.X -= (float) (this.speed + this.addedSpeed);
          }
          else
            this.blockedInterval += time.ElapsedGameTime.Milliseconds;
        }
      }
      else
        this.Sprite.animateOnce(time);
      if (flag && currentLocation != null && this.isMoving())
      {
        Point standingXy = this.getStandingXY();
        Vector2 tile = new Vector2((float) (standingXy.X / 64), (float) (standingXy.Y / 64));
        currentLocation.characterTrampleTile(tile);
      }
      if (this.blockedInterval >= 3000 && (double) this.blockedInterval <= 3750.0 && !Game1.eventUp)
      {
        this.doEmote(Game1.random.NextDouble() < 0.5 ? 8 : 40);
        this.blockedInterval = 3750;
      }
      else
      {
        if (this.blockedInterval < 5000)
          return;
        this.speed = 4;
        this.isCharging = true;
        this.blockedInterval = 0;
      }
    }

    public virtual bool canPassThroughActionTiles() => false;

    public virtual Microsoft.Xna.Framework.Rectangle nextPosition(int direction)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      switch (direction)
      {
        case 0:
          boundingBox.Y -= this.speed + this.addedSpeed;
          break;
        case 1:
          boundingBox.X += this.speed + this.addedSpeed;
          break;
        case 2:
          boundingBox.Y += this.speed + this.addedSpeed;
          break;
        case 3:
          boundingBox.X -= this.speed + this.addedSpeed;
          break;
      }
      return boundingBox;
    }

    public Location nextPositionPoint()
    {
      Location location = new Location();
      switch (this.getDirection())
      {
        case 0:
          location = new Location(this.getStandingX(), this.getStandingY() - 64);
          break;
        case 1:
          location = new Location(this.getStandingX() + 64, this.getStandingY());
          break;
        case 2:
          location = new Location(this.getStandingX(), this.getStandingY() + 64);
          break;
        case 3:
          location = new Location(this.getStandingX() - 64, this.getStandingY());
          break;
      }
      return location;
    }

    public int getHorizontalMovement()
    {
      if (this.moveRight)
        return this.speed + this.addedSpeed;
      return !this.moveLeft ? 0 : -this.speed - this.addedSpeed;
    }

    public int getVerticalMovement()
    {
      if (this.moveDown)
        return this.speed + this.addedSpeed;
      return !this.moveUp ? 0 : -this.speed - this.addedSpeed;
    }

    public Vector2 nextPositionVector2() => new Vector2((float) (this.getStandingX() + this.getHorizontalMovement()), (float) (this.getStandingY() + this.getVerticalMovement()));

    public Location nextPositionTile()
    {
      Location location = this.nextPositionPoint();
      location.X /= 64;
      location.Y /= 64;
      return location;
    }

    public virtual void doEmote(int whichEmote, bool playSound, bool nextEventCommand = true)
    {
      if (this.isEmoting || Game1.eventUp && !(this is Farmer) && (Game1.currentLocation.currentEvent == null || !((IEnumerable<Character>) Game1.currentLocation.currentEvent.actors).Contains<Character>(this)))
        return;
      this.isEmoting = true;
      this.currentEmote = whichEmote;
      this.currentEmoteFrame = 0;
      this.emoteInterval = 0.0f;
      this.nextEventcommandAfterEmote = nextEventCommand;
    }

    public void doEmote(int whichEmote, bool nextEventCommand = true) => this.doEmote(whichEmote, true, nextEventCommand);

    public void updateEmote(GameTime time)
    {
      if (!this.isEmoting)
        return;
      this.emoteInterval += (float) time.ElapsedGameTime.Milliseconds;
      if (this.emoteFading && (double) this.emoteInterval > 20.0)
      {
        this.emoteInterval = 0.0f;
        --this.currentEmoteFrame;
        if (this.currentEmoteFrame >= 0)
          return;
        this.emoteFading = false;
        this.isEmoting = false;
        if (!this.nextEventcommandAfterEmote || Game1.currentLocation.currentEvent == null || !((IEnumerable<Character>) Game1.currentLocation.currentEvent.actors).Contains<Character>(this) && !((IEnumerable<Character>) Game1.currentLocation.currentEvent.farmerActors).Contains<Character>(this) && !this.Name.Equals(Game1.player.Name))
          return;
        ++Game1.currentLocation.currentEvent.CurrentCommand;
      }
      else if (!this.emoteFading && (double) this.emoteInterval > 20.0 && this.currentEmoteFrame <= 3)
      {
        this.emoteInterval = 0.0f;
        ++this.currentEmoteFrame;
        if (this.currentEmoteFrame != 4)
          return;
        this.currentEmoteFrame = this.currentEmote;
      }
      else
      {
        if (this.emoteFading || (double) this.emoteInterval <= 250.0)
          return;
        this.emoteInterval = 0.0f;
        ++this.currentEmoteFrame;
        if (this.currentEmoteFrame < this.currentEmote + 4)
          return;
        this.emoteFading = true;
        this.currentEmoteFrame = 3;
      }
    }

    public Vector2 GetGrabTile()
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      switch (this.FacingDirection)
      {
        case 0:
          return new Vector2((float) ((boundingBox.X + boundingBox.Width / 2) / 64), (float) ((boundingBox.Y - 5) / 64));
        case 1:
          return new Vector2((float) ((boundingBox.X + boundingBox.Width + 5) / 64), (float) ((boundingBox.Y + boundingBox.Height / 2) / 64));
        case 2:
          return new Vector2((float) ((boundingBox.X + boundingBox.Width / 2) / 64), (float) ((boundingBox.Y + boundingBox.Height + 5) / 64));
        case 3:
          return new Vector2((float) ((boundingBox.X - 5) / 64), (float) ((boundingBox.Y + boundingBox.Height / 2) / 64));
        default:
          return this.getStandingPosition();
      }
    }

    public Vector2 GetDropLocation()
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      switch (this.FacingDirection)
      {
        case 0:
          return new Vector2((float) (boundingBox.X + 16), (float) (boundingBox.Y - 64));
        case 1:
          return new Vector2((float) (boundingBox.X + boundingBox.Width + 64), (float) (boundingBox.Y + 16));
        case 2:
          return new Vector2((float) (boundingBox.X + 16), (float) (boundingBox.Y + boundingBox.Height + 64));
        case 3:
          return new Vector2((float) (boundingBox.X - 64), (float) (boundingBox.Y + 16));
        default:
          return this.getStandingPosition();
      }
    }

    public virtual Vector2 GetToolLocation(Vector2 target_position, bool ignoreClick = false)
    {
      int num = this.FacingDirection;
      if ((Game1.player.CurrentTool == null || !Game1.player.CurrentTool.CanUseOnStandingTile()) && (int) ((double) target_position.X / 64.0) == Game1.player.getTileX() && (int) ((double) target_position.Y / 64.0) == Game1.player.getTileY())
      {
        Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
        switch (this.FacingDirection)
        {
          case 0:
            return new Vector2((float) (boundingBox.X + boundingBox.Width / 2), (float) (boundingBox.Y - 64));
          case 1:
            return new Vector2((float) (boundingBox.X + boundingBox.Width + 64), (float) (boundingBox.Y + boundingBox.Height / 2));
          case 2:
            return new Vector2((float) (boundingBox.X + boundingBox.Width / 2), (float) (boundingBox.Y + boundingBox.Height + 64));
          case 3:
            return new Vector2((float) (boundingBox.X - 64), (float) (boundingBox.Y + boundingBox.Height / 2));
        }
      }
      if (!ignoreClick && !target_position.Equals(Vector2.Zero) && this.Name.Equals(Game1.player.Name))
      {
        bool flag = false;
        if (Game1.player.CurrentTool != null && Game1.player.CurrentTool.CanUseOnStandingTile())
          flag = true;
        if (Utility.withinRadiusOfPlayer((int) target_position.X, (int) target_position.Y, 1, Game1.player))
        {
          num = Game1.player.getGeneralDirectionTowards(new Vector2((float) (int) target_position.X, (float) (int) target_position.Y));
          if (flag || (double) Math.Abs(target_position.X - (float) Game1.player.getStandingX()) >= 32.0 || (double) Math.Abs(target_position.Y - (float) Game1.player.getStandingY()) >= 32.0)
            return target_position;
        }
      }
      Microsoft.Xna.Framework.Rectangle boundingBox1 = this.GetBoundingBox();
      if (Game1.player.CurrentTool != null && Game1.player.CurrentTool.Name.Equals("Fishing Rod"))
      {
        switch (num)
        {
          case 0:
            return new Vector2((float) (boundingBox1.X - 16), (float) (boundingBox1.Y - 102));
          case 1:
            return new Vector2((float) (boundingBox1.X + boundingBox1.Width + 64), (float) boundingBox1.Y);
          case 2:
            return new Vector2((float) (boundingBox1.X - 16), (float) (boundingBox1.Y + boundingBox1.Height + 64));
          case 3:
            return new Vector2((float) (boundingBox1.X - 112), (float) boundingBox1.Y);
        }
      }
      else
      {
        switch (num)
        {
          case 0:
            return new Vector2((float) (boundingBox1.X + boundingBox1.Width / 2), (float) (boundingBox1.Y - 48));
          case 1:
            return new Vector2((float) (boundingBox1.X + boundingBox1.Width + 48), (float) (boundingBox1.Y + boundingBox1.Height / 2));
          case 2:
            return new Vector2((float) (boundingBox1.X + boundingBox1.Width / 2), (float) (boundingBox1.Y + boundingBox1.Height + 48));
          case 3:
            return new Vector2((float) (boundingBox1.X - 48), (float) (boundingBox1.Y + boundingBox1.Height / 2));
        }
      }
      return new Vector2((float) this.getStandingX(), (float) this.getStandingY());
    }

    public virtual Vector2 GetToolLocation(bool ignoreClick = false)
    {
      if (!Game1.wasMouseVisibleThisFrame || Game1.isAnyGamePadButtonBeingHeld())
        ignoreClick = true;
      return this.GetToolLocation(this.lastClick, ignoreClick);
    }

    public int getGeneralDirectionTowards(
      Vector2 target,
      int yBias = 0,
      bool opposite = false,
      bool useTileCalculations = true)
    {
      int num1 = opposite ? -1 : 1;
      int num2;
      int num3;
      if (useTileCalculations)
      {
        int tileX = this.getTileX();
        int tileY = this.getTileY();
        num2 = ((int) ((double) target.X / 64.0) - tileX) * num1;
        num3 = ((int) ((double) target.Y / 64.0) - tileY) * num1;
        if (num2 == 0 && num3 == 0)
        {
          Vector2 vector2 = new Vector2((float) (((double) (int) ((double) target.X / 64.0) + 0.5) * 64.0), (float) (((double) (int) ((double) target.Y / 64.0) + 0.5) * 64.0));
          num2 = (int) ((double) vector2.X - (double) this.getStandingX()) * num1;
          num3 = (int) ((double) vector2.Y - (double) this.getStandingY()) * num1;
          yBias *= 64;
        }
      }
      else
      {
        int standingX = this.getStandingX();
        int standingY = this.getStandingY();
        num2 = (int) ((double) target.X - (double) standingX) * num1;
        num3 = (int) ((double) target.Y - (double) standingY) * num1;
      }
      if (num2 > Math.Abs(num3) + yBias)
        return 1;
      if (Math.Abs(num2) > Math.Abs(num3) + yBias)
        return 3;
      return num3 > 0 || ((double) this.getStandingY() - (double) target.Y) * (double) num1 < 0.0 ? 2 : 0;
    }

    public void faceGeneralDirection(
      Vector2 target,
      int yBias,
      bool opposite,
      bool useTileCalculations)
    {
      this.faceDirection(this.getGeneralDirectionTowards(target, yBias, opposite, useTileCalculations));
    }

    public void faceGeneralDirection(Vector2 target, int yBias = 0, bool opposite = false) => this.faceGeneralDirection(target, yBias, opposite, true);

    public virtual void draw(SpriteBatch b) => this.draw(b, 1f);

    public virtual void drawAboveAlwaysFrontLayer(SpriteBatch b)
    {
    }

    public virtual void draw(SpriteBatch b, float alpha = 1f)
    {
      Vector2 position = this.Position;
      this.Sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, position), (float) this.GetBoundingBox().Center.Y / 10000f);
      if (!this.IsEmoting)
        return;
      Vector2 localPosition = this.getLocalPosition(Game1.viewport);
      localPosition.Y -= 96f;
      b.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.getStandingY() / 10000f);
    }

    public virtual void draw(SpriteBatch b, int ySourceRectOffset, float alpha = 1f)
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      this.Sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position) + new Vector2((float) (this.GetSpriteWidthForPositioning() * 4 / 2), (float) (boundingBox.Height / 2)), (float) boundingBox.Center.Y / 10000f, 0, ySourceRectOffset, Color.White, scale: 4f, characterSourceRectOffset: true);
      if (!this.IsEmoting)
        return;
      Vector2 localPosition = this.getLocalPosition(Game1.viewport);
      localPosition.Y -= 96f;
      b.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.getStandingY() / 10000f);
    }

    public virtual int GetSpriteWidthForPositioning() => this.forceOneTileWide.Value ? 16 : this.Sprite.SpriteWidth;

    public virtual Microsoft.Xna.Framework.Rectangle GetBoundingBox()
    {
      if (this.Sprite == null)
        return Microsoft.Xna.Framework.Rectangle.Empty;
      Vector2 position = this.Position;
      int width = this.GetSpriteWidthForPositioning() * 4 * 3 / 4;
      return new Microsoft.Xna.Framework.Rectangle((int) position.X + 8, (int) position.Y + 16, width, 32);
    }

    public void stopWithoutChangingFrame()
    {
      this.moveDown = false;
      this.moveLeft = false;
      this.moveRight = false;
      this.moveUp = false;
    }

    public virtual void collisionWithFarmerBehavior()
    {
    }

    public int getStandingX()
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      return boundingBox.X + boundingBox.Width / 2;
    }

    public int getStandingY()
    {
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      return boundingBox.Y + boundingBox.Height / 2;
    }

    public Vector2 getStandingPosition()
    {
      Point center = this.GetBoundingBox().Center;
      return new Vector2((float) center.X, (float) center.Y);
    }

    public Point getStandingXY() => this.GetBoundingBox().Center;

    public Vector2 getLocalPosition(xTile.Dimensions.Rectangle viewport)
    {
      Vector2 position = this.Position;
      return new Vector2(position.X - (float) viewport.X, position.Y - (float) viewport.Y + (float) this.yJumpOffset) + (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawOffset;
    }

    public virtual bool isMoving() => this.moveUp || this.moveDown || this.moveRight || this.moveLeft || this.position.Field.IsInterpolating();

    public Point getTileLocationPoint()
    {
      Point standingXy = this.getStandingXY();
      return new Point(standingXy.X / 64, standingXy.Y / 64);
    }

    public int getTileX() => this.getStandingX() / 64;

    public int getTileY() => this.getStandingY() / 64;

    public Vector2 getTileLocation()
    {
      Point standingXy = this.getStandingXY();
      return new Vector2((float) (standingXy.X / 64), (float) (standingXy.Y / 64));
    }

    public void setTileLocation(Vector2 tileLocation)
    {
      float num1 = (float) (((double) tileLocation.X + 0.5) * 64.0);
      float num2 = (float) (((double) tileLocation.Y + 0.5) * 64.0);
      Vector2 position = this.Position;
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      position.X += num1 - (float) boundingBox.Center.X;
      position.Y += num2 - (float) boundingBox.Center.Y;
      this.Position = position;
    }

    public void startGlowing(Color glowingColor, bool border, float glowRate)
    {
      if (this.glowingColor.Equals(glowingColor))
        return;
      this.isGlowing = true;
      this.coloredBorder = border;
      this.glowingColor = glowingColor;
      this.glowUp = true;
      this.glowRate = glowRate;
      this.glowingTransparency = 0.0f;
    }

    public void stopGlowing()
    {
      this.isGlowing = false;
      this.glowingColor = Color.White;
    }

    public virtual void jumpWithoutSound(float velocity = 8f)
    {
      this.yJumpVelocity = velocity;
      this.yJumpOffset = -1;
      this.yJumpGravity = -0.5f;
    }

    public virtual void jump()
    {
      this.yJumpVelocity = 8f;
      this.yJumpOffset = -1;
      this.yJumpGravity = -0.5f;
      this.wasJumpWithSound = true;
      this.currentLocation.localSound("dwop");
    }

    public virtual void jump(float jumpVelocity)
    {
      this.yJumpVelocity = jumpVelocity;
      this.yJumpOffset = -1;
      this.yJumpGravity = -0.5f;
      this.wasJumpWithSound = true;
      this.currentLocation.localSound("dwop");
    }

    public void faceTowardFarmerForPeriod(int milliseconds, int radius, bool faceAway, Farmer who)
    {
      if ((this.Sprite == null || this.Sprite.CurrentAnimation != null) && !this.isMoving())
        return;
      if (this.isMoving())
        milliseconds /= 2;
      this.faceTowardFarmerEvent.Fire(milliseconds);
      this.faceTowardFarmerEvent.Poll();
      if ((int) (NetFieldBase<int, NetInt>) this.facingDirectionBeforeSpeakingToPlayer == -1)
        this.facingDirectionBeforeSpeakingToPlayer.Value = this.FacingDirection;
      this.faceTowardFarmerRadius.Value = radius;
      this.faceAwayFromFarmer.Value = faceAway;
      this.whoToFace.Value = who;
      this.hasJustStartedFacingPlayer = true;
    }

    private void performFaceTowardFarmerEvent(int milliseconds)
    {
      if ((this.Sprite == null || this.Sprite.CurrentAnimation != null) && !this.isMoving())
        return;
      this.Halt();
      this.faceTowardFarmerTimer = milliseconds;
      this.movementPause = milliseconds;
    }

    public virtual void update(GameTime time, GameLocation location)
    {
      this.position.UpdateExtrapolation((float) (this.speed + this.addedSpeed));
      this.update(time, location, 0L, true);
    }

    public virtual void performBehavior(byte which)
    {
    }

    public virtual void checkForFootstep() => Game1.currentLocation.playTerrainSound(this.getTileLocation(), this);

    public virtual void update(GameTime time, GameLocation location, long id, bool move)
    {
      this.position.UpdateExtrapolation((float) (this.speed + this.addedSpeed));
      this.currentLocation = location;
      this.faceTowardFarmerEvent.Poll();
      if (this.yJumpOffset != 0)
      {
        this.yJumpVelocity += this.yJumpGravity;
        this.yJumpOffset -= (int) this.yJumpVelocity;
        if (this.yJumpOffset >= 0)
        {
          this.yJumpOffset = 0;
          this.yJumpVelocity = 0.0f;
          if (!this.IsMonster && (location == null || location.Equals(Game1.currentLocation)) && this.wasJumpWithSound)
            this.checkForFootstep();
        }
      }
      if (this.forceUpdateTimer > 0)
        this.forceUpdateTimer -= time.ElapsedGameTime.Milliseconds;
      this.updateGlow();
      this.updateEmote(time);
      this.updateFaceTowardsFarmer(time, location);
      bool flag = false;
      if (location.currentEvent != null)
      {
        if (location.isTemp())
          flag = true;
        else if (((IEnumerable<Character>) location.currentEvent.actors).Contains<Character>(this))
          flag = true;
      }
      if (Game1.IsMasterGame | flag)
      {
        if (this.controller == null & move && !this.freezeMotion)
          this.updateMovement(location, time);
        if (this.controller != null && !this.freezeMotion && this.controller.update(time))
          this.controller = (PathFindController) null;
      }
      else
        this.updateSlaveAnimation(time);
      this.hasJustStartedFacingPlayer = false;
    }

    public virtual void updateFaceTowardsFarmer(GameTime time, GameLocation location)
    {
      if (this.faceTowardFarmerTimer > 0)
      {
        this.faceTowardFarmerTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.whoToFace.Value != null)
        {
          if (!this.faceTowardFarmer && this.faceTowardFarmerTimer > 0 && Utility.tileWithinRadiusOfPlayer((int) this.getTileLocation().X, (int) this.getTileLocation().Y, (int) (NetFieldBase<int, NetInt>) this.faceTowardFarmerRadius, (Farmer) this.whoToFace))
            this.faceTowardFarmer = true;
          else if (!Utility.tileWithinRadiusOfPlayer((int) this.getTileLocation().X, (int) this.getTileLocation().Y, (int) (NetFieldBase<int, NetInt>) this.faceTowardFarmerRadius, (Farmer) this.whoToFace) || this.faceTowardFarmerTimer <= 0)
          {
            this.faceDirection(this.facingDirectionBeforeSpeakingToPlayer.Value);
            if (this.faceTowardFarmerTimer <= 0)
            {
              this.facingDirectionBeforeSpeakingToPlayer.Value = -1;
              this.faceTowardFarmer = false;
              this.faceAwayFromFarmer.Value = false;
              this.faceTowardFarmerTimer = 0;
            }
          }
        }
      }
      if ((Game1.IsMasterGame || location.currentEvent != null) && this.faceTowardFarmer && this.whoToFace.Value != null)
      {
        this.faceGeneralDirection(this.whoToFace.Value.getStandingPosition(), 0, false, true);
        if ((bool) (NetFieldBase<bool, NetBool>) this.faceAwayFromFarmer)
          this.faceDirection((this.FacingDirection + 2) % 4);
      }
      this.hasJustStartedFacingPlayer = false;
    }

    public virtual bool hasSpecialCollisionRules() => false;

    /// <summary>
    /// 
    /// make sure that you also override hasSpecialCollisionRules() in any class that overrides isColliding().
    /// Otherwise isColliding() will never be called.
    /// dumb I kno
    /// </summary>
    /// <param name="l"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public virtual bool isColliding(GameLocation l, Vector2 tile) => false;

    public virtual void animateInFacingDirection(GameTime time)
    {
      switch (this.FacingDirection)
      {
        case 0:
          this.Sprite.AnimateUp(time);
          break;
        case 1:
          this.Sprite.AnimateRight(time);
          break;
        case 2:
          this.Sprite.AnimateDown(time);
          break;
        case 3:
          this.Sprite.AnimateLeft(time);
          break;
      }
    }

    public virtual void updateMovement(GameLocation location, GameTime time)
    {
    }

    protected virtual void updateSlaveAnimation(GameTime time)
    {
      if (this.Sprite.CurrentAnimation != null)
      {
        this.Sprite.animateOnce(time);
      }
      else
      {
        this.faceDirection(this.FacingDirection);
        if (this.isMoving())
          this.animateInFacingDirection(time);
        else
          this.Sprite.StopAnimation();
      }
    }

    public void updateGlow()
    {
      if (!this.isGlowing)
        return;
      if (this.glowUp)
      {
        this.glowingTransparency += this.glowRate;
        if ((double) this.glowingTransparency < 1.0)
          return;
        this.glowingTransparency = 1f;
        this.glowUp = false;
      }
      else
      {
        this.glowingTransparency -= this.glowRate;
        if ((double) this.glowingTransparency > 0.0)
          return;
        this.glowingTransparency = 0.0f;
        this.glowUp = true;
      }
    }

    public void convertEventMotionCommandToMovement(Vector2 command)
    {
      if ((double) command.X < 0.0)
        this.SetMovingLeft(true);
      else if ((double) command.X > 0.0)
        this.SetMovingRight(true);
      else if ((double) command.Y < 0.0)
      {
        this.SetMovingUp(true);
      }
      else
      {
        if ((double) command.Y <= 0.0)
          return;
        this.SetMovingDown(true);
      }
    }
  }
}
