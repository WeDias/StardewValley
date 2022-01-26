// Decompiled with JetBrains decompiler
// Type: StardewValley.Characters.Pet
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
  public class Pet : NPC
  {
    public const int bedTime = 2000;
    public const int maxFriendship = 1000;
    public const int behavior_Walk = 0;
    public const int behavior_Sleep = 1;
    public const int behavior_SitDown = 2;
    public const int frame_basicSit = 18;
    [XmlElement("whichBreed")]
    public readonly NetInt whichBreed = new NetInt();
    private readonly NetInt netCurrentBehavior = new NetInt();
    [XmlIgnore]
    public readonly NetEvent1Field<string, NetString> petAnimationEvent = new NetEvent1Field<string, NetString>();
    [XmlIgnore]
    protected int _currentBehavior = -1;
    [XmlIgnore]
    protected int _lastDirection = -1;
    [XmlElement("lastPetDay")]
    public NetLongDictionary<int, NetInt> lastPetDay = new NetLongDictionary<int, NetInt>();
    [XmlElement("grantedFriendshipForPet")]
    public NetBool grantedFriendshipForPet = new NetBool(false);
    [XmlElement("friendshipTowardFarmer")]
    public NetInt friendshipTowardFarmer = new NetInt(0);
    public NetBool isSleepingOnFarmerBed = new NetBool(false);
    [XmlIgnore]
    public readonly NetMutex mutex = new NetMutex();
    private int pushingTimer;

    public int CurrentBehavior
    {
      get => this.netCurrentBehavior.Value;
      set
      {
        if (this.netCurrentBehavior.Value == value)
          return;
        this.netCurrentBehavior.Value = value;
      }
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.netCurrentBehavior, (INetSerializable) this.whichBreed, (INetSerializable) this.friendshipTowardFarmer, (INetSerializable) this.grantedFriendshipForPet, (INetSerializable) this.mutex.NetFields, (INetSerializable) this.lastPetDay, (INetSerializable) this.petAnimationEvent, (INetSerializable) this.isSleepingOnFarmerBed);
      this.name.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
      this.petAnimationEvent.onEvent += new AbstractNetEvent1<string>.Event(this.OnPetAnimationEvent);
      this.friendshipTowardFarmer.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((field, old_value, new_value) => this.GrantLoveMailIfNecessary());
      this.isSleepingOnFarmerBed.fieldChangeVisibleEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((a, b, c) => this.UpdateSleepingOnBed());
      this.whichBreed.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((a, b, c) => this.reloadBreedSprite());
    }

    protected void _FlipFrames()
    {
    }

    public virtual void OnPetAnimationEvent(string animation_event)
    {
    }

    public override void behaviorOnFarmerLocationEntry(GameLocation location, Farmer who)
    {
      base.behaviorOnFarmerLocationEntry(location, who);
      if (location is Farm && Game1.timeOfDay >= 2000 && !location.farmers.Any())
      {
        if (this.CurrentBehavior != 1 || this.currentLocation is Farm)
          Game1.player.team.requestPetWarpHomeEvent.Fire(Game1.player.UniqueMultiplayerID);
      }
      else if (Game1.timeOfDay < 2000 && Game1.random.NextDouble() < 0.5 && this._currentBehavior != 1)
      {
        this.CurrentBehavior = 1;
        this._OnNewBehavior();
        this.Sprite.UpdateSourceRect();
      }
      this.UpdateSleepingOnBed();
    }

    public override void behaviorOnLocalFarmerLocationEntry(GameLocation location)
    {
      base.behaviorOnLocalFarmerLocationEntry(location);
      this.netCurrentBehavior.CancelInterpolation();
      if (this.netCurrentBehavior.Value == 1)
      {
        this.position.NetFields.CancelInterpolation();
        if (this._currentBehavior != 1)
        {
          this._OnNewBehavior();
          this.Sprite.UpdateSourceRect();
        }
      }
      this.UpdateSleepingOnBed();
    }

    public override bool canTalk() => false;

    public virtual string getPetTextureName() => "Animals\\dog" + (this.whichBreed.Value == 0 ? "" : this.whichBreed.Value.ToString() ?? "");

    public void reloadBreedSprite()
    {
      if (this.Sprite == null)
        return;
      this.Sprite.LoadTexture(this.getPetTextureName());
    }

    public override void reloadSprite()
    {
      this.reloadBreedSprite();
      this.DefaultPosition = new Vector2(54f, 8f) * 64f;
      this.HideShadow = true;
      this.Breather = false;
      this.setAtFarmPosition();
    }

    public void warpToFarmHouse(Farmer who)
    {
      this.isSleepingOnFarmerBed.Value = false;
      FarmHouse homeOfFarmer = Utility.getHomeOfFarmer(who);
      Vector2 vector2 = Vector2.Zero;
      int num = 0;
      vector2 = new Vector2((float) Game1.random.Next(2, homeOfFarmer.map.Layers[0].LayerWidth - 3), (float) Game1.random.Next(3, homeOfFarmer.map.Layers[0].LayerHeight - 5));
      List<Furniture> list = new List<Furniture>();
      foreach (Furniture furniture in homeOfFarmer.furniture)
      {
        if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type == 12)
          list.Add(furniture);
      }
      BedFurniture playerBed = homeOfFarmer.GetPlayerBed();
      if (playerBed != null && !Game1.newDay && Game1.timeOfDay >= 2000 && (this is Cat && Game1.random.NextDouble() <= 0.75 || Game1.random.NextDouble() <= 0.0500000007450581))
      {
        Vector2 position = Utility.PointToVector2(playerBed.GetBedSpot()) + new Vector2(-1f, 0.0f);
        Game1.warpCharacter((NPC) this, (GameLocation) homeOfFarmer, position);
        this.NetFields.CancelInterpolation();
        this.CurrentBehavior = 1;
        this.isSleepingOnFarmerBed.Value = true;
        foreach (Furniture furniture in homeOfFarmer.furniture)
        {
          if (furniture is BedFurniture && furniture.getBoundingBox(furniture.TileLocation).Intersects(this.GetBoundingBox()))
          {
            (furniture as BedFurniture).ReserveForNPC();
            break;
          }
        }
        this.UpdateSleepingOnBed();
        this._OnNewBehavior();
        this.Sprite.UpdateSourceRect();
      }
      else
      {
        if (Game1.random.NextDouble() <= 0.300000011920929)
          vector2 = Utility.PointToVector2(homeOfFarmer.getBedSpot()) + new Vector2(0.0f, 2f);
        else if (Game1.random.NextDouble() <= 0.5)
        {
          Furniture random = Utility.GetRandom<Furniture>(list, Game1.random);
          if (random != null)
            vector2 = new Vector2((float) (random.boundingBox.Left / 64), (float) (random.boundingBox.Center.Y / 64));
        }
        for (; num < 50 && (!homeOfFarmer.canPetWarpHere(vector2) || !homeOfFarmer.isTileLocationTotallyClearAndPlaceable(vector2) || !homeOfFarmer.isTileLocationTotallyClearAndPlaceable(vector2 + new Vector2(1f, 0.0f)) || homeOfFarmer.isTileOnWall((int) vector2.X, (int) vector2.Y)); ++num)
          vector2 = new Vector2((float) Game1.random.Next(2, homeOfFarmer.map.Layers[0].LayerWidth - 3), (float) Game1.random.Next(3, homeOfFarmer.map.Layers[0].LayerHeight - 4));
        if (num < 50)
        {
          Game1.warpCharacter((NPC) this, (GameLocation) homeOfFarmer, vector2);
          this.CurrentBehavior = 1;
        }
        else
        {
          this.faceDirection(2);
          Game1.warpCharacter((NPC) this, "Farm", (Game1.getLocationFromName("Farm") as Farm).GetPetStartLocation());
        }
        this.UpdateSleepingOnBed();
        this._OnNewBehavior();
        this.Sprite.UpdateSourceRect();
      }
    }

    public virtual void UpdateSleepingOnBed()
    {
      this.drawOnTop = false;
      this.collidesWithOtherCharacters.Value = !this.isSleepingOnFarmerBed.Value;
      this.farmerPassesThrough = this.isSleepingOnFarmerBed.Value;
    }

    public override void dayUpdate(int dayOfMonth)
    {
      this.isSleepingOnFarmerBed.Value = false;
      this.UpdateSleepingOnBed();
      this.DefaultPosition = new Vector2(54f, 8f) * 64f;
      this.Sprite.loop = false;
      this.Breather = false;
      if (Game1.isRaining)
      {
        this.CurrentBehavior = 2;
        this.warpToFarmHouse(Game1.player);
      }
      else if (this.currentLocation is FarmHouse)
        this.setAtFarmPosition();
      if (this.currentLocation is Farm && Game1.IsMasterGame)
      {
        if ((this.currentLocation as Farm).petBowlWatered.Value)
          this.friendshipTowardFarmer.Set(Math.Min(1000, this.friendshipTowardFarmer.Value + 6));
        (this.currentLocation as Farm).petBowlWatered.Set(false);
      }
      this.Halt();
      this.CurrentBehavior = 1;
      this.grantedFriendshipForPet.Set(false);
      this._OnNewBehavior();
      this.Sprite.UpdateSourceRect();
    }

    public void GrantLoveMailIfNecessary()
    {
      if (this.friendshipTowardFarmer.Value < 1000)
        return;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (allFarmer != null && !allFarmer.mailReceived.Contains("petLoveMessage"))
        {
          if (allFarmer == Game1.player)
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Characters:PetLovesYou", (object) this.displayName));
          allFarmer.mailReceived.Add("petLoveMessage");
        }
      }
    }

    public void setAtFarmPosition()
    {
      if (!Game1.IsMasterGame)
        return;
      GameLocation currentLocation = this.currentLocation;
      if (!Game1.isRaining)
      {
        this.faceDirection(2);
        Game1.warpCharacter((NPC) this, "Farm", (Game1.getLocationFromName("Farm") as Farm).GetPetStartLocation());
      }
      else
        this.warpToFarmHouse(Game1.MasterPlayer);
    }

    public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

    public override bool canPassThroughActionTiles() => false;

    public override bool checkAction(Farmer who, GameLocation l)
    {
      if (!this.lastPetDay.ContainsKey(who.UniqueMultiplayerID))
        this.lastPetDay.Add(who.UniqueMultiplayerID, -1);
      if (this.lastPetDay[who.UniqueMultiplayerID] == Game1.Date.TotalDays)
        return false;
      this.lastPetDay[who.UniqueMultiplayerID] = Game1.Date.TotalDays;
      this.mutex.RequestLock((Action) (() =>
      {
        if (!this.grantedFriendshipForPet.Value)
        {
          this.grantedFriendshipForPet.Set(true);
          this.friendshipTowardFarmer.Set(Math.Min(1000, (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + 12));
        }
        this.mutex.ReleaseLock();
      }));
      this.doEmote(20);
      this.playContentSound();
      return true;
    }

    public virtual void playContentSound()
    {
    }

    public void hold(Farmer who)
    {
      this.flip = this.Sprite.CurrentAnimation.Last<FarmerSprite.AnimationFrame>().flip;
      this.Sprite.CurrentFrame = this.Sprite.CurrentAnimation.Last<FarmerSprite.AnimationFrame>().frame;
      this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      this.Sprite.loop = false;
    }

    public override void behaviorOnFarmerPushing()
    {
      if (this is Dog && (this as Dog).CurrentBehavior == 51)
        return;
      this.pushingTimer += 2;
      if (this.pushingTimer <= 100)
        return;
      Vector2 playerTrajectory = Utility.getAwayFromPlayerTrajectory(this.GetBoundingBox(), Game1.player);
      this.setTrajectory((int) playerTrajectory.X / 2, (int) playerTrajectory.Y / 2);
      this.pushingTimer = 0;
      this.Halt();
      this.facePlayer(Game1.player);
      this.FacingDirection += 2;
      this.FacingDirection %= 4;
      this.faceDirection(this.FacingDirection);
      this.CurrentBehavior = 0;
    }

    public override void update(GameTime time, GameLocation location, long id, bool move)
    {
      base.update(time, location, id, move);
      this.pushingTimer = Math.Max(0, this.pushingTimer - 1);
    }

    public override void update(GameTime time, GameLocation location)
    {
      base.update(time, location);
      this.petAnimationEvent.Poll();
      if (this.isSleepingOnFarmerBed.Value && this.CurrentBehavior != 1 && Game1.IsMasterGame)
      {
        this.isSleepingOnFarmerBed.Value = false;
        this.UpdateSleepingOnBed();
      }
      if (this.currentLocation == null)
        this.currentLocation = location;
      this.mutex.Update(location);
      if (Game1.eventUp)
        return;
      if (this._currentBehavior != this.CurrentBehavior)
        this._OnNewBehavior();
      this.RunState(time);
      if (Game1.IsMasterGame && this.Sprite.CurrentAnimation == null)
        this.MovePosition(time, Game1.viewport, location);
      this.flip = false;
      if (this.FacingDirection != 3 || this.Sprite.CurrentFrame < 16)
        return;
      this.flip = true;
    }

    public virtual void RunState(GameTime time)
    {
      if (this._currentBehavior == 0 && Game1.IsMasterGame && this.currentLocation.isCollidingPosition(this.nextPosition(this.FacingDirection), Game1.viewport, (Character) this))
      {
        int direction = Game1.random.Next(0, 4);
        if (!this.currentLocation.isCollidingPosition(this.nextPosition(this.FacingDirection), Game1.viewport, (Character) this))
          this.faceDirection(direction);
      }
      if (!Game1.IsMasterGame || Game1.timeOfDay < 2000 || this.Sprite.CurrentAnimation != null || (double) this.xVelocity != 0.0 || (double) this.yVelocity != 0.0)
        return;
      this.CurrentBehavior = 1;
    }

    protected override void updateSlaveAnimation(GameTime time)
    {
      if (this.Sprite.CurrentAnimation != null)
      {
        this.Sprite.animateOnce(time);
      }
      else
      {
        if (this.CurrentBehavior != 0)
          return;
        this.Sprite.faceDirection(this.FacingDirection);
        if (this.isMoving())
          this.animateInFacingDirection(time);
        else
          this.Sprite.StopAnimation();
      }
    }

    protected void _OnNewBehavior()
    {
      this._currentBehavior = this.CurrentBehavior;
      this.Halt();
      this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      this.OnNewBehavior();
    }

    public virtual void OnNewBehavior()
    {
      this.Sprite.loop = false;
      this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
      switch (this.CurrentBehavior)
      {
        case 0:
          if (Game1.IsMasterGame)
          {
            this.Halt();
            this.faceDirection(Game1.random.Next(4));
            this.setMovingInFacingDirection();
          }
          this.Sprite.loop = true;
          break;
        case 1:
          this.Sprite.loop = true;
          bool flip = Game1.random.NextDouble() < 0.5;
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(28, 1000, false, flip),
            new FarmerSprite.AnimationFrame(29, 1000, false, flip)
          });
          break;
        case 2:
          this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
          {
            new FarmerSprite.AnimationFrame(16, 100, false, false),
            new FarmerSprite.AnimationFrame(17, 100, false, false),
            new FarmerSprite.AnimationFrame(18, 100, false, false, new AnimatedSprite.endOfAnimationBehavior(this.hold))
          });
          break;
      }
    }

    public override Rectangle GetBoundingBox()
    {
      Vector2 position = this.Position;
      return new Rectangle((int) position.X + 16, (int) position.Y + 16, this.Sprite.SpriteWidth * 4 * 3 / 4, 32);
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(this.Sprite.Texture, this.getLocalPosition(Game1.viewport) + new Vector2((float) (this.Sprite.SpriteWidth * 4 / 2), (float) (this.GetBoundingBox().Height / 2)) + (this.shakeTimer > 0 ? new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)) : Vector2.Zero), new Rectangle?(this.Sprite.SourceRect), Color.White, this.rotation, new Vector2((float) (this.Sprite.SpriteWidth / 2), (float) ((double) this.Sprite.SpriteHeight * 3.0 / 4.0)), Math.Max(0.2f, (float) (NetFieldBase<float, NetFloat>) this.scale) * 4f, this.flip || this.Sprite.CurrentAnimation != null && this.Sprite.CurrentAnimation[this.Sprite.currentAnimationIndex].flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, this.isSleepingOnFarmerBed.Value ? (float) (((double) this.getStandingY() + 112.0) / 10000.0) : (float) this.getStandingY() / 10000f));
      if (!this.IsEmoting)
        return;
      Vector2 localPosition = this.getLocalPosition(Game1.viewport);
      localPosition.X += 32f;
      localPosition.Y -= (float) (96 + (this is Dog ? 16 : 0));
      b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.getStandingY() / 10000.0 + 9.99999974737875E-05));
    }

    public override bool withinPlayerThreshold(int threshold)
    {
      if (this.currentLocation != null && !this.currentLocation.farmers.Any())
        return false;
      Vector2 tileLocation1 = this.getTileLocation();
      foreach (Character farmer in this.currentLocation.farmers)
      {
        Vector2 tileLocation2 = farmer.getTileLocation();
        if ((double) Math.Abs(tileLocation1.X - tileLocation2.X) <= (double) threshold && (double) Math.Abs(tileLocation1.Y - tileLocation2.Y) <= (double) threshold)
          return true;
      }
      return false;
    }
  }
}
