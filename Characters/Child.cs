// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Child
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Characters
{
  public class Child : NPC
  {
    public const int newborn = 0;
    public const int baby = 1;
    public const int crawler = 2;
    public const int toddler = 3;
    [XmlElement("daysOld")]
    public readonly NetInt daysOld = new NetInt(0);
    [XmlElement("idOfParent")]
    public NetLong idOfParent = new NetLong(0L);
    [XmlElement("darkSkinned")]
    public readonly NetBool darkSkinned = new NetBool(false);
    private readonly NetEvent1Field<int, NetInt> setStateEvent = new NetEvent1Field<int, NetInt>();
    [XmlElement("hat")]
    public readonly NetRef<Hat> hat = new NetRef<Hat>();
    [XmlIgnore]
    public readonly NetMutex mutex = new NetMutex();
    private int previousState;

    public Child()
    {
    }

    public Child(string name, bool isMale, bool isDarkSkinned, Farmer parent)
    {
      this.Age = 2;
      this.Gender = isMale ? 0 : 1;
      this.darkSkinned.Value = isDarkSkinned;
      this.reloadSprite();
      this.Name = name;
      this.displayName = name;
      this.DefaultMap = "FarmHouse";
      this.HideShadow = true;
      this.speed = 1;
      this.idOfParent.Value = parent.UniqueMultiplayerID;
      this.Breather = false;
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.setStateEvent, (INetSerializable) this.darkSkinned, (INetSerializable) this.daysOld, (INetSerializable) this.idOfParent, (INetSerializable) this.mutex.NetFields, (INetSerializable) this.hat);
      this.age.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((a, b, c) => this.reloadSprite());
      this.setStateEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doSetState);
      this.name.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
    }

    public override void reloadSprite()
    {
      if (Game1.IsMasterGame)
      {
        Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(this.idOfParent.Value);
        if (this.idOfParent.Value == 0L || farmerMaybeOffline == null)
        {
          long uniqueMultiplayerId = Game1.MasterPlayer.UniqueMultiplayerID;
          if (this.currentLocation is FarmHouse)
          {
            GameLocation currentLocation = this.currentLocation;
            foreach (Farmer allFarmer in Game1.getAllFarmers())
            {
              if (Utility.getHomeOfFarmer(allFarmer) == this.currentLocation)
              {
                uniqueMultiplayerId = allFarmer.UniqueMultiplayerID;
                break;
              }
            }
          }
          this.idOfParent.Value = uniqueMultiplayerId;
        }
      }
      if (this.Sprite == null)
        this.Sprite = new AnimatedSprite("Characters\\Baby" + (this.darkSkinned.Value ? "_dark" : ""), 0, 22, 16);
      if (this.Age >= 3)
      {
        this.Sprite.textureName.Value = "Characters\\Toddler" + (this.Gender == 0 ? "" : "_girl") + (this.darkSkinned.Value ? "_dark" : "");
        this.Sprite.SpriteWidth = 16;
        this.Sprite.SpriteHeight = 32;
        this.Sprite.currentFrame = 0;
        this.HideShadow = false;
      }
      else
      {
        this.Sprite.textureName.Value = "Characters\\Baby" + (this.darkSkinned.Value ? "_dark" : "");
        this.Sprite.SpriteWidth = 22;
        this.Sprite.SpriteHeight = this.Age == 1 ? 32 : 16;
        this.Sprite.currentFrame = 0;
        if (this.Age == 1)
          this.Sprite.currentFrame = 4;
        else if (this.Age == 2)
          this.Sprite.currentFrame = 32;
        this.HideShadow = true;
      }
      this.Sprite.UpdateSourceRect();
      this.Breather = false;
    }

    protected override void updateSlaveAnimation(GameTime time)
    {
      if (this.Age < 2 || this.Sprite.currentFrame <= 7 && this.Sprite.SpriteHeight == 16)
        return;
      base.updateSlaveAnimation(time);
    }

    public override void MovePosition(
      GameTime time,
      xTile.Dimensions.Rectangle viewport,
      GameLocation currentLocation)
    {
      if (Game1.eventUp && Game1.CurrentEvent != null && Game1.CurrentEvent.isFestival)
      {
        base.MovePosition(time, viewport, currentLocation);
      }
      else
      {
        if (!Game1.IsMasterGame)
        {
          this.moveLeft = this.IsRemoteMoving() && this.FacingDirection == 3;
          this.moveRight = this.IsRemoteMoving() && this.FacingDirection == 1;
          this.moveUp = this.IsRemoteMoving() && this.FacingDirection == 0;
          this.moveDown = this.IsRemoteMoving() && this.FacingDirection == 2;
        }
        if (this.moveUp)
        {
          if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(0), viewport, false, 0, false, (Character) this) || this.isCharging)
          {
            if (Game1.IsMasterGame)
              this.position.Y -= (float) (this.speed + this.addedSpeed);
            if (this.Age == 3)
            {
              this.Sprite.AnimateUp(time);
              this.FacingDirection = 0;
            }
          }
          else if (!currentLocation.isTilePassable(this.nextPosition(0), viewport) || !this.willDestroyObjectsUnderfoot)
          {
            this.moveUp = false;
            this.Sprite.currentFrame = this.Sprite.CurrentAnimation != null ? this.Sprite.CurrentAnimation[0].frame : this.Sprite.currentFrame;
            this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
            if (Game1.IsMasterGame && this.Age == 2 && Game1.timeOfDay < 1800)
              this.setCrawlerInNewDirection();
          }
        }
        else if (this.moveRight)
        {
          if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(1), viewport, false, 0, false, (Character) this) || this.isCharging)
          {
            if (Game1.IsMasterGame)
              this.position.X += (float) (this.speed + this.addedSpeed);
            if (this.Age == 3)
            {
              this.Sprite.AnimateRight(time);
              this.FacingDirection = 1;
            }
          }
          else if (!currentLocation.isTilePassable(this.nextPosition(1), viewport) || !this.willDestroyObjectsUnderfoot)
          {
            this.moveRight = false;
            this.Sprite.currentFrame = this.Sprite.CurrentAnimation != null ? this.Sprite.CurrentAnimation[0].frame : this.Sprite.currentFrame;
            this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
            if (Game1.IsMasterGame && this.Age == 2 && Game1.timeOfDay < 1800)
              this.setCrawlerInNewDirection();
          }
        }
        else if (this.moveDown)
        {
          if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(2), viewport, false, 0, false, (Character) this) || this.isCharging)
          {
            if (Game1.IsMasterGame)
              this.position.Y += (float) (this.speed + this.addedSpeed);
            if (this.Age == 3)
            {
              this.Sprite.AnimateDown(time);
              this.FacingDirection = 2;
            }
          }
          else if (!currentLocation.isTilePassable(this.nextPosition(2), viewport) || !this.willDestroyObjectsUnderfoot)
          {
            this.moveDown = false;
            this.Sprite.currentFrame = this.Sprite.CurrentAnimation != null ? this.Sprite.CurrentAnimation[0].frame : this.Sprite.currentFrame;
            this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
            if (Game1.IsMasterGame && this.Age == 2 && Game1.timeOfDay < 1800)
              this.setCrawlerInNewDirection();
          }
        }
        else if (this.moveLeft)
        {
          if (currentLocation == null || !currentLocation.isCollidingPosition(this.nextPosition(3), viewport, false, 0, false, (Character) this) || this.isCharging)
          {
            if (Game1.IsMasterGame)
              this.position.X -= (float) (this.speed + this.addedSpeed);
            if (this.Age == 3)
            {
              this.Sprite.AnimateLeft(time);
              this.FacingDirection = 3;
            }
          }
          else if (!currentLocation.isTilePassable(this.nextPosition(3), viewport) || !this.willDestroyObjectsUnderfoot)
          {
            this.moveLeft = false;
            this.Sprite.currentFrame = this.Sprite.CurrentAnimation != null ? this.Sprite.CurrentAnimation[0].frame : this.Sprite.currentFrame;
            this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
            if (Game1.IsMasterGame && this.Age == 2 && Game1.timeOfDay < 1800)
              this.setCrawlerInNewDirection();
          }
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
          this.speed = 1;
          this.isCharging = true;
          this.blockedInterval = 0;
        }
      }
    }

    public override bool canPassThroughActionTiles() => false;

    public override void resetForNewDay(int dayOfMonth)
    {
      base.resetForNewDay(dayOfMonth);
      if (!(this.currentLocation is FarmHouse) || (this.currentLocation as FarmHouse).GetChildBed(this.GetChildIndex()) != null)
        return;
      this.sleptInBed.Value = false;
    }

    protected override string translateName(string name) => name.TrimEnd();

    public override void dayUpdate(int dayOfMonth)
    {
      this.resetForNewDay(dayOfMonth);
      this.mutex.ReleaseLock();
      this.moveUp = false;
      this.moveDown = false;
      this.moveLeft = false;
      this.moveRight = false;
      int uniqueMultiplayerId = (int) Game1.MasterPlayer.UniqueMultiplayerID;
      if (Game1.currentLocation is FarmHouse)
      {
        FarmHouse currentLocation = Game1.currentLocation as FarmHouse;
        if (currentLocation.owner != null)
          uniqueMultiplayerId = (int) currentLocation.owner.UniqueMultiplayerID;
      }
      Random r = new Random(Game1.Date.TotalDays + (int) Game1.uniqueIDForThisGame / 2 + uniqueMultiplayerId * 2);
      ++this.daysOld.Value;
      if (this.daysOld.Value >= 55)
      {
        this.Age = 3;
        this.speed = 4;
      }
      else if (this.daysOld.Value >= 27)
        this.Age = 2;
      else if (this.daysOld.Value >= 13)
        this.Age = 1;
      if ((int) (NetFieldBase<int, NetInt>) this.age == 0 || (int) (NetFieldBase<int, NetInt>) this.age == 1)
        this.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
      if (this.Age == 2)
      {
        this.speed = 1;
        Point openPointInHouse = (this.currentLocation as FarmHouse).getRandomOpenPointInHouse(r, 1, 200);
        if (!openPointInHouse.Equals(Point.Zero))
          this.setTilePosition(openPointInHouse);
        else
          this.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
        this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      }
      if (this.Age == 3)
      {
        Point p = (this.currentLocation as FarmHouse).getRandomOpenPointInHouse(r, 1, 200);
        if (!p.Equals(Point.Zero))
        {
          this.setTilePosition(p);
        }
        else
        {
          FarmHouse currentLocation = this.currentLocation as FarmHouse;
          currentLocation.GetChildBed(this.GetChildIndex());
          p = currentLocation.GetChildBedSpot(this.GetChildIndex());
          if (!p.Equals(Point.Zero))
            this.setTilePosition(p);
        }
        this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      }
      this.reloadSprite();
      if (this.Age != 2)
        return;
      this.setCrawlerInNewDirection();
    }

    public bool isInCrib() => this.getTileX() >= 15 && this.getTileX() <= 17 && this.getTileY() >= 3 && this.getTileY() <= 4;

    public void toss(Farmer who)
    {
      if (Game1.timeOfDay >= 1800 || this.Sprite.SpriteHeight <= 16)
        return;
      if (who == Game1.player)
        this.mutex.RequestLock((Action) (() => this.performToss(who)));
      else
        this.performToss(who);
    }

    public void performToss(Farmer who)
    {
      who.forceTimePass = true;
      who.faceDirection(2);
      who.FarmerSprite.PauseForSingleAnimation = false;
      who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
      {
        new FarmerSprite.AnimationFrame(57, 1500, false, false, new AnimatedSprite.endOfAnimationBehavior(this.doneTossing), true)
      });
      this.Position = who.Position + new Vector2(-16f, -96f);
      this.yJumpVelocity = (float) Game1.random.Next(12, 19);
      this.yJumpOffset = -1;
      Game1.playSound("dwop");
      who.CanMove = false;
      who.freezePause = 1500;
      this.drawOnTop = true;
      this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
      {
        new FarmerSprite.AnimationFrame(4, 100),
        new FarmerSprite.AnimationFrame(5, 100),
        new FarmerSprite.AnimationFrame(6, 100),
        new FarmerSprite.AnimationFrame(7, 100)
      });
    }

    public void doneTossing(Farmer who)
    {
      who.forceTimePass = false;
      this.resetForPlayerEntry(who.currentLocation);
      who.CanMove = true;
      who.forceCanMove();
      who.faceDirection(0);
      this.drawOnTop = false;
      this.doEmote(20);
      if (!who.friendshipData.ContainsKey(this.Name))
        who.friendshipData.Add(this.Name, new Friendship(250));
      who.talkToFriend((NPC) this);
      Game1.playSound("tinyWhip");
      if (!this.mutex.IsLockHeld())
        return;
      this.mutex.ReleaseLock();
    }

    public override Microsoft.Xna.Framework.Rectangle getMugShotSourceRect()
    {
      switch (this.Age)
      {
        case 0:
          return new Microsoft.Xna.Framework.Rectangle(0, 0, 22, 16);
        case 1:
          return new Microsoft.Xna.Framework.Rectangle(0, 42, 22, 24);
        case 2:
          return new Microsoft.Xna.Framework.Rectangle(0, 112, 22, 16);
        case 3:
          return new Microsoft.Xna.Framework.Rectangle(0, 4, 16, 24);
        default:
          return Microsoft.Xna.Framework.Rectangle.Empty;
      }
    }

    private void setState(int state) => this.setStateEvent.Fire(state);

    private void doSetState(int state)
    {
      switch (state)
      {
        case 0:
          this.SetMovingOnlyUp();
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(32, 160),
            new FarmerSprite.AnimationFrame(33, 160),
            new FarmerSprite.AnimationFrame(34, 160),
            new FarmerSprite.AnimationFrame(35, 160)
          });
          break;
        case 1:
          this.SetMovingOnlyRight();
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(28, 160),
            new FarmerSprite.AnimationFrame(29, 160),
            new FarmerSprite.AnimationFrame(30, 160),
            new FarmerSprite.AnimationFrame(31, 160)
          });
          break;
        case 2:
          this.SetMovingOnlyDown();
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(24, 160),
            new FarmerSprite.AnimationFrame(25, 160),
            new FarmerSprite.AnimationFrame(26, 160),
            new FarmerSprite.AnimationFrame(27, 160)
          });
          break;
        case 3:
          this.SetMovingOnlyLeft();
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(36, 160),
            new FarmerSprite.AnimationFrame(37, 160),
            new FarmerSprite.AnimationFrame(38, 160),
            new FarmerSprite.AnimationFrame(39, 160)
          });
          break;
        case 4:
          this.Halt();
          this.Sprite.SpriteHeight = 16;
          this.Sprite.setCurrentAnimation(this.getRandomCrawlerAnimation(0));
          break;
        case 5:
          this.Halt();
          this.Sprite.SpriteHeight = 16;
          this.Sprite.setCurrentAnimation(this.getRandomCrawlerAnimation(1));
          break;
      }
    }

    private void setCrawlerInNewDirection()
    {
      if (!Game1.IsMasterGame)
        return;
      this.speed = 1;
      int state = Game1.random.Next(6);
      if (Game1.timeOfDay >= 1800 && this.isInCrib())
      {
        this.Sprite.currentFrame = 7;
        this.Sprite.UpdateSourceRect();
      }
      else
      {
        if (this.previousState >= 4 && Game1.random.NextDouble() < 0.6)
          state = this.previousState;
        if (state < 4)
        {
          while (state == this.previousState)
            state = Game1.random.Next(6);
        }
        else if (this.previousState >= 4)
          state = this.previousState;
        if (this.isInCrib())
          state = Game1.random.Next(4, 6);
        this.setState(state);
        this.previousState = state;
      }
    }

    public override bool hasSpecialCollisionRules() => true;

    public override bool isColliding(GameLocation l, Vector2 tile) => !l.isTilePlaceable(tile);

    public void tenMinuteUpdate()
    {
      if (Game1.IsMasterGame && this.Age == 2)
        this.setCrawlerInNewDirection();
      else if (Game1.IsMasterGame && Game1.timeOfDay % 100 == 0 && this.Age == 3 && Game1.timeOfDay < 1900)
      {
        this.IsWalkingInSquare = false;
        this.Halt();
        FarmHouse currentLocation = this.currentLocation as FarmHouse;
        if (!currentLocation.characters.Contains((NPC) this))
          return;
        this.controller = new PathFindController((Character) this, (GameLocation) currentLocation, currentLocation.getRandomOpenPointInHouse(Game1.random, 1), -1, new PathFindController.endBehavior(this.toddlerReachedDestination));
        if (this.controller.pathToEndPoint != null && currentLocation.isTileOnMap(this.controller.pathToEndPoint.Last<Point>().X, this.controller.pathToEndPoint.Last<Point>().Y))
          return;
        this.controller = (PathFindController) null;
      }
      else
      {
        if (!Game1.IsMasterGame || this.Age != 3 || Game1.timeOfDay != 1900)
          return;
        this.IsWalkingInSquare = false;
        this.Halt();
        FarmHouse currentLocation = this.currentLocation as FarmHouse;
        if (!currentLocation.characters.Contains((NPC) this))
          return;
        int childIndex = this.GetChildIndex();
        BedFurniture childBed = currentLocation.GetChildBed(childIndex);
        Point childBedSpot = currentLocation.GetChildBedSpot(childIndex);
        if (childBedSpot.Equals(Point.Zero))
          return;
        this.controller = new PathFindController((Character) this, (GameLocation) currentLocation, childBedSpot, -1, new PathFindController.endBehavior(this.toddlerReachedDestination));
        if (this.controller.pathToEndPoint == null || !currentLocation.isTileOnMap(this.controller.pathToEndPoint.Last<Point>().X, this.controller.pathToEndPoint.Last<Point>().Y))
          this.controller = (PathFindController) null;
        else
          childBed?.ReserveForNPC();
      }
    }

    public virtual int GetChildIndex()
    {
      Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(this.idOfParent.Value);
      if (farmerMaybeOffline == null)
        return this.Gender;
      List<Child> children = farmerMaybeOffline.getChildren();
      children.Sort((Comparison<Child>) ((a, b) => a.daysOld.Value.CompareTo(b.daysOld.Value)));
      children.Reverse();
      return children.IndexOf(this);
    }

    public void toddlerReachedDestination(Character c, GameLocation l)
    {
      if (Game1.random.NextDouble() < 0.8 && c.FacingDirection == 2)
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(16, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(17, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(18, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(19, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(18, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(17, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(16, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(0, 1000, 0, false, false),
          new FarmerSprite.AnimationFrame(16, 100, 0, false, false),
          new FarmerSprite.AnimationFrame(17, 100, 0, false, false),
          new FarmerSprite.AnimationFrame(18, 100, 0, false, false),
          new FarmerSprite.AnimationFrame(19, 100, 0, false, false),
          new FarmerSprite.AnimationFrame(18, 300, 0, false, false),
          new FarmerSprite.AnimationFrame(17, 100, 0, false, false),
          new FarmerSprite.AnimationFrame(16, 100, 0, false, false),
          new FarmerSprite.AnimationFrame(0, 2000, 0, false, false),
          new FarmerSprite.AnimationFrame(16, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(17, 180, 0, false, false),
          new FarmerSprite.AnimationFrame(16, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(0, 800, 0, false, false)
        });
      else if (Game1.random.NextDouble() < 0.8 && c.FacingDirection == 1)
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(20, 120, 0, false, false),
          new FarmerSprite.AnimationFrame(21, 70, 0, false, false),
          new FarmerSprite.AnimationFrame(22, 70, 0, false, false),
          new FarmerSprite.AnimationFrame(23, 70, 0, false, false),
          new FarmerSprite.AnimationFrame(22, 999999, 0, false, false)
        });
      else if (Game1.random.NextDouble() < 0.8 && c.FacingDirection == 3)
      {
        this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
        {
          new FarmerSprite.AnimationFrame(20, 120, 0, false, true),
          new FarmerSprite.AnimationFrame(21, 70, 0, false, true),
          new FarmerSprite.AnimationFrame(22, 70, 0, false, true),
          new FarmerSprite.AnimationFrame(23, 70, 0, false, true),
          new FarmerSprite.AnimationFrame(22, 999999, 0, false, true)
        });
      }
      else
      {
        if (c.FacingDirection != 0)
          return;
        this.lastCrossroad = new Microsoft.Xna.Framework.Rectangle(this.getTileX() * 64, this.getTileY() * 64, 64, 64);
        this.squareMovementFacingPreference = -1;
        this.walkInSquare(4, 4, 2000);
      }
    }

    public override bool canTalk() => Game1.player.friendshipData.ContainsKey(this.Name) && !Game1.player.friendshipData[this.Name].TalkedToToday;

    public override bool checkAction(Farmer who, GameLocation l)
    {
      if (!who.friendshipData.ContainsKey(this.Name))
        who.friendshipData.Add(this.Name, new Friendship(250));
      if (this.Age >= 2 && !who.hasTalkedToFriendToday(this.Name))
      {
        who.talkToFriend((NPC) this);
        this.doEmote(20);
        if (this.Age == 3)
          this.faceTowardFarmerForPeriod(4000, 3, false, who);
        return true;
      }
      if (Game1.CurrentEvent != null || this.Age < 3 || who.items.Count <= who.CurrentToolIndex || who.items[who.CurrentToolIndex] == null || !(who.Items[who.CurrentToolIndex] is Hat))
        return false;
      if (this.hat.Value != null)
      {
        Game1.createItemDebris((Item) (Hat) (NetFieldBase<Hat, NetRef<Hat>>) this.hat, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position, (int) this.facingDirection);
        this.hat.Value = (Hat) null;
      }
      else
      {
        Hat hat = who.Items[who.CurrentToolIndex] as Hat;
        who.Items[who.CurrentToolIndex] = (Item) null;
        this.hat.Value = hat;
        Game1.playSound("dirtyHit");
      }
      return false;
    }

    private List<FarmerSprite.AnimationFrame> getRandomCrawlerAnimation(int which = -1)
    {
      List<FarmerSprite.AnimationFrame> crawlerAnimation = new List<FarmerSprite.AnimationFrame>();
      double num = Game1.random.NextDouble();
      if (which == 0 || num < 0.5)
      {
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(40, 500, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(41, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(42, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(43, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(42, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(41, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(40, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(41, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(42, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(43, 1900, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(42, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(41, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(40, 500, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(41, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(40, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(41, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(40, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(40, 1500, 0, false, false));
      }
      else if (which == 1 || num >= 0.5)
      {
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(44, 1500, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(45, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(44, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(46, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(44, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(45, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(44, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(46, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(44, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(45, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(44, 200, 0, false, false));
        crawlerAnimation.Add(new FarmerSprite.AnimationFrame(46, 200, 0, false, false));
      }
      return crawlerAnimation;
    }

    private List<FarmerSprite.AnimationFrame> getRandomNewbornAnimation()
    {
      List<FarmerSprite.AnimationFrame> newbornAnimation = new List<FarmerSprite.AnimationFrame>();
      if (Game1.random.NextDouble() < 0.5)
      {
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(0, 400, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(1, 400, 0, false, false));
      }
      else
      {
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(1, 3400, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(2, 100, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(3, 100, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(4, 100, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(5, 100, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(6, 4400, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(5, 3400, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(4, 100, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(3, 100, 0, false, false));
        newbornAnimation.Add(new FarmerSprite.AnimationFrame(2, 100, 0, false, false));
      }
      return newbornAnimation;
    }

    private List<FarmerSprite.AnimationFrame> getRandomBabyAnimation()
    {
      List<FarmerSprite.AnimationFrame> randomBabyAnimation = new List<FarmerSprite.AnimationFrame>();
      if (Game1.random.NextDouble() < 0.5)
      {
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(4, 120, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(5, 120, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(6, 120, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(7, 120, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(4, 100, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(5, 100, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(6, 100, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(7, 100, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(4, 150, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(5, 150, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(6, 150, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(7, 150, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(4, 2000, 0, false, false));
        if (Game1.random.NextDouble() < 0.5)
        {
          randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(8, 1950, 0, false, false));
          randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(9, 1200, 0, false, false));
          randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(10, 180, 0, false, false));
          randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(11, 1500, 0, false, false));
          randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(8, 1500, 0, false, false));
        }
      }
      else
      {
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(8, 250, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(9, 250, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(10, 250, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(11, 250, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(8, 1950, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(9, 1200, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(10, 180, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(11, 1500, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(8, 1500, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(9, 150, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(10, 150, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(11, 150, 0, false, false));
        randomBabyAnimation.Add(new FarmerSprite.AnimationFrame(8, 1500, 0, false, false));
      }
      return randomBabyAnimation;
    }

    public override void update(GameTime time, GameLocation location)
    {
      this.setStateEvent.Poll();
      this.mutex.Update(location);
      base.update(time, location);
      if (this.Age < 2 || !Game1.IsMasterGame && this.Age >= 3)
        return;
      this.MovePosition(time, Game1.viewport, location);
    }

    public void resetForPlayerEntry(GameLocation l)
    {
      if (this.Age == 0)
      {
        this.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
        if (Game1.timeOfDay >= 1800 && this.Sprite != null)
        {
          this.Sprite.StopAnimation();
          this.Sprite.currentFrame = Game1.random.Next(7);
        }
        else if (this.Sprite != null)
          this.Sprite.setCurrentAnimation(this.getRandomNewbornAnimation());
      }
      else if (this.Age == 1)
      {
        this.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -12f);
        if (Game1.timeOfDay >= 1800 && this.Sprite != null)
        {
          this.Sprite.StopAnimation();
          this.Sprite.SpriteHeight = 16;
          this.Sprite.currentFrame = Game1.random.Next(7);
        }
        else if (this.Sprite != null)
        {
          this.Sprite.SpriteHeight = 32;
          this.Sprite.setCurrentAnimation(this.getRandomBabyAnimation());
        }
      }
      else if (this.Age == 2)
      {
        if (this.Sprite != null)
          this.Sprite.SpriteHeight = 16;
        if (Game1.timeOfDay >= 1800)
        {
          this.Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f);
          if (this.Sprite != null)
          {
            this.Sprite.StopAnimation();
            this.Sprite.SpriteHeight = 16;
            this.Sprite.currentFrame = 7;
          }
        }
      }
      if (this.Sprite != null)
        this.Sprite.loop = true;
      if (this.drawOnTop && !this.mutex.IsLocked())
        this.drawOnTop = false;
      this.Sprite.UpdateSourceRect();
    }

    public override void draw(SpriteBatch b, float alpha = 1f)
    {
      Microsoft.Xna.Framework.Rectangle sourceRect1 = this.Sprite.SourceRect;
      int spriteHeight = this.Sprite.SpriteHeight;
      int yJumpOffset = this.yJumpOffset;
      if (this.hat.Value != null && this.hat.Value.hairDrawType.Value != 0)
      {
        Microsoft.Xna.Framework.Rectangle sourceRect2 = this.Sprite.SourceRect;
        int num1 = 17;
        switch (this.Sprite.CurrentFrame)
        {
          case 0:
            num1 = 17;
            break;
          case 1:
            num1 = 18;
            break;
          case 2:
            num1 = 17;
            break;
          case 3:
            num1 = 16;
            break;
          case 4:
            num1 = 17;
            break;
          case 5:
            num1 = 18;
            break;
          case 6:
            num1 = 17;
            break;
          case 7:
            num1 = 16;
            break;
          case 8:
            num1 = 17;
            break;
          case 9:
            num1 = 18;
            break;
          case 10:
            num1 = 17;
            break;
          case 11:
            num1 = 16;
            break;
          case 12:
            num1 = 17;
            break;
          case 13:
            num1 = 16;
            break;
          case 14:
            num1 = 17;
            break;
          case 15:
            num1 = 18;
            break;
          case 16:
            num1 = 17;
            break;
          case 17:
            num1 = 17;
            break;
          case 18:
            num1 = 16;
            break;
          case 19:
            num1 = 16;
            break;
          case 20:
            num1 = 17;
            break;
          case 21:
            num1 = 16;
            break;
          case 22:
            num1 = 15;
            break;
          case 23:
            num1 = 14;
            break;
        }
        int num2 = sourceRect1.Height - num1;
        sourceRect2.Y += sourceRect1.Height - num1;
        sourceRect2.Height = num1;
        this.Sprite.SourceRect = sourceRect2;
        this.Sprite.SpriteHeight = num1;
        this.yJumpOffset = num2;
      }
      base.draw(b, 1f);
      this.Sprite.SpriteHeight = spriteHeight;
      this.Sprite.SourceRect = sourceRect1;
      this.yJumpOffset = yJumpOffset;
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      if (this.IsEmoting && !Game1.eventUp)
      {
        Vector2 localPosition = this.getLocalPosition(Game1.viewport);
        localPosition.Y -= (float) (32 + this.Sprite.SpriteHeight * 4 - (this.Age == 1 || this.Age == 3 ? 64 : 0));
        localPosition.X += this.Age == 1 ? 8f : 0.0f;
        b.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.getStandingY() / 10000f);
      }
      bool flag1 = true;
      if (this.hat.Value == null)
        return;
      Vector2 vector2 = Vector2.Zero * 4f;
      if ((double) vector2.X <= -100.0)
        return;
      float num = (float) this.GetBoundingBox().Center.Y / 10000f;
      vector2.X = -36f;
      vector2.Y = -12f;
      if (!flag1)
        return;
      float layerDepth = num + 1E-07f;
      int direction = 2;
      bool flag2 = this.sprite.Value.CurrentAnimation != null && this.sprite.Value.CurrentAnimation[this.sprite.Value.currentAnimationIndex].flip;
      switch (this.Sprite.CurrentFrame)
      {
        case 1:
          vector2.Y -= 4f;
          direction = 2;
          break;
        case 3:
          vector2.Y += 4f;
          direction = 2;
          break;
        case 4:
          direction = 1;
          break;
        case 5:
          vector2.Y -= 4f;
          direction = 1;
          break;
        case 6:
          direction = 1;
          break;
        case 7:
          vector2.Y += 4f;
          direction = 1;
          break;
        case 8:
          direction = 0;
          break;
        case 9:
          vector2.Y -= 4f;
          direction = 0;
          break;
        case 10:
          direction = 0;
          break;
        case 11:
          vector2.Y += 4f;
          direction = 0;
          break;
        case 12:
          direction = 3;
          break;
        case 13:
          vector2.Y += 4f;
          direction = 3;
          break;
        case 14:
          direction = 3;
          break;
        case 15:
          vector2.Y -= 4f;
          direction = 3;
          break;
        case 18:
        case 19:
          vector2.Y += 4f;
          direction = 2;
          break;
        case 20:
          direction = 1;
          break;
        case 21:
          vector2.Y += 4f;
          direction = flag2 ? 3 : 1;
          vector2.X += (float) ((flag2 ? 1 : -1) * 4);
          break;
        case 22:
          vector2.Y += 8f;
          direction = flag2 ? 3 : 1;
          vector2.X += (float) ((flag2 ? 2 : -2) * 4);
          break;
        case 23:
          vector2.Y += 12f;
          direction = flag2 ? 3 : 1;
          vector2.X += (float) ((flag2 ? 2 : -2) * 4);
          break;
      }
      if (this.shakeTimer > 0)
        vector2 += new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2));
      this.hat.Value.draw(b, this.getLocalPosition(Game1.viewport) + vector2 + new Vector2(30f, -42f), 1.333333f, 1f, layerDepth, direction);
    }

    public override void behaviorOnLocalFarmerLocationEntry(GameLocation location) => this.reloadSprite();
  }
}
