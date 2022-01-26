// Decompiled with JetBrains decompiler
// Type: StardewValley.Projectiles.Projectile
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Projectiles
{
  public abstract class Projectile : INetObject<NetFields>
  {
    public const int travelTimeBeforeCollisionPossible = 100;
    public const int goblinsCurseIndex = 0;
    public const int flameBallIndex = 1;
    public const int fearBolt = 2;
    public const int shadowBall = 3;
    public const int bone = 4;
    public const int throwingKnife = 5;
    public const int snowBall = 6;
    public const int shamanBolt = 7;
    public const int frostBall = 8;
    public const int frozenBolt = 9;
    public const int fireball = 10;
    public const int slash = 11;
    public const int arrowBolt = 12;
    public const int launchedSlime = 13;
    public const string projectileSheetName = "TileSheets\\Projectiles";
    public const int timePerTailUpdate = 50;
    public static int boundingBoxWidth = 21;
    public static int boundingBoxHeight = 21;
    public static Texture2D projectileSheet;
    protected readonly NetInt currentTileSheetIndex = new NetInt();
    protected readonly NetPosition position = new NetPosition();
    protected readonly NetInt tailLength = new NetInt();
    protected int tailCounter = 50;
    protected readonly NetInt bouncesLeft = new NetInt();
    protected int travelTime;
    protected float? _rotation;
    [XmlIgnore]
    public float hostTimeUntilAttackable = -1f;
    public readonly NetFloat startingRotation = new NetFloat();
    protected readonly NetFloat rotationVelocity = new NetFloat();
    protected readonly NetFloat xVelocity = new NetFloat();
    protected readonly NetFloat yVelocity = new NetFloat();
    public readonly NetColor color = new NetColor(Color.White);
    private Queue<Vector2> tail = new Queue<Vector2>();
    public readonly NetInt maxTravelDistance = new NetInt(-1);
    public float travelDistance;
    public NetFloat height = new NetFloat(0.0f);
    protected readonly NetBool damagesMonsters = new NetBool();
    protected readonly NetBool spriteFromObjectSheet = new NetBool();
    protected readonly NetCharacterRef theOneWhoFiredMe = new NetCharacterRef();
    public readonly NetBool ignoreTravelGracePeriod = new NetBool(false);
    public readonly NetBool ignoreLocationCollision = new NetBool();
    public readonly NetBool ignoreMeleeAttacks = new NetBool(false);
    public bool destroyMe;
    public readonly NetFloat startingScale = new NetFloat(1f);
    [XmlIgnore]
    protected float? _localScale;
    public readonly NetFloat scaleGrow = new NetFloat(0.0f);
    public NetBool light = new NetBool();
    public bool hasLit;
    private int lightID;
    private float startingAlpha = 1f;

    protected float rotation
    {
      get
      {
        if (!this._rotation.HasValue)
          this._rotation = new float?(this.startingRotation.Value);
        return this._rotation.Value;
      }
      set => this._rotation = new float?(value);
    }

    public bool IgnoreLocationCollision
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.ignoreLocationCollision;
      set => this.ignoreLocationCollision.Value = value;
    }

    public NetFields NetFields { get; } = new NetFields();

    public Projectile() => this.NetFields.AddFields((INetSerializable) this.currentTileSheetIndex, (INetSerializable) this.position.NetFields, (INetSerializable) this.tailLength, (INetSerializable) this.bouncesLeft, (INetSerializable) this.rotationVelocity, (INetSerializable) this.startingRotation, (INetSerializable) this.xVelocity, (INetSerializable) this.yVelocity, (INetSerializable) this.damagesMonsters, (INetSerializable) this.spriteFromObjectSheet, (INetSerializable) this.theOneWhoFiredMe.NetFields, (INetSerializable) this.ignoreLocationCollision, (INetSerializable) this.maxTravelDistance, (INetSerializable) this.ignoreTravelGracePeriod, (INetSerializable) this.ignoreMeleeAttacks, (INetSerializable) this.height, (INetSerializable) this.startingScale, (INetSerializable) this.scaleGrow, (INetSerializable) this.color, (INetSerializable) this.light);

    private bool behaviorOnCollision(GameLocation location)
    {
      if (this.hasLit)
        Utility.removeLightSource(this.lightID);
      foreach (Vector2 vector2 in Utility.getListOfTileLocationsForBordersOfNonTileRectangle(this.getBoundingBox()))
      {
        foreach (Farmer farmer in location.farmers)
        {
          if (!(bool) (NetFieldBase<bool, NetBool>) this.damagesMonsters && farmer.GetBoundingBox().Intersects(this.getBoundingBox()))
          {
            this.behaviorOnCollisionWithPlayer(location, farmer);
            return true;
          }
        }
        if (location.terrainFeatures.ContainsKey(vector2) && !location.terrainFeatures[vector2].isPassable())
        {
          this.behaviorOnCollisionWithTerrainFeature(location.terrainFeatures[vector2], vector2, location);
          return true;
        }
        if ((bool) (NetFieldBase<bool, NetBool>) this.damagesMonsters)
        {
          NPC n = location.doesPositionCollideWithCharacter(this.getBoundingBox());
          if (n != null)
          {
            this.behaviorOnCollisionWithMonster(n, location);
            return true;
          }
        }
      }
      this.behaviorOnCollisionWithOther(location);
      return true;
    }

    public abstract void behaviorOnCollisionWithPlayer(GameLocation location, Farmer player);

    public abstract void behaviorOnCollisionWithTerrainFeature(
      TerrainFeature t,
      Vector2 tileLocation,
      GameLocation location);

    public abstract void behaviorOnCollisionWithMineWall(int tileX, int tileY);

    public abstract void behaviorOnCollisionWithOther(GameLocation location);

    public abstract void behaviorOnCollisionWithMonster(NPC n, GameLocation location);

    [XmlIgnore]
    public virtual float localScale
    {
      get
      {
        if (!this._localScale.HasValue)
          this._localScale = new float?(this.startingScale.Value);
        return this._localScale.Value;
      }
      set => this._localScale = new float?(value);
    }

    public virtual bool update(GameTime time, GameLocation location)
    {
      TimeSpan elapsedGameTime;
      if (Game1.IsMasterGame && (double) this.hostTimeUntilAttackable > 0.0)
      {
        double timeUntilAttackable = (double) this.hostTimeUntilAttackable;
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds = elapsedGameTime.TotalSeconds;
        this.hostTimeUntilAttackable = (float) (timeUntilAttackable - totalSeconds);
        if ((double) this.hostTimeUntilAttackable <= 0.0)
        {
          this.ignoreMeleeAttacks.Value = false;
          this.hostTimeUntilAttackable = -1f;
        }
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.light)
      {
        if (!this.hasLit)
        {
          this.hasLit = true;
          this.lightID = Game1.random.Next(int.MinValue, int.MaxValue);
          Game1.currentLightSources.Add(new LightSource(4, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position + new Vector2(32f, 32f), 1f, new Color(0, 65, 128), this.lightID));
        }
        else
        {
          Vector2 position = (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position;
          Utility.repositionLightSource(this.lightID, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position + new Vector2(32f, 32f));
        }
      }
      this.rotation += (float) (NetFieldBase<float, NetFloat>) this.rotationVelocity;
      int travelTime = this.travelTime;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds = elapsedGameTime.Milliseconds;
      this.travelTime = travelTime + milliseconds;
      if ((double) this.scaleGrow.Value != 0.0)
        this.localScale += this.scaleGrow.Value;
      Vector2 vector2 = this.position.Value;
      this.updatePosition(time);
      this.updateTail(time);
      this.travelDistance += (vector2 - this.position.Value).Length();
      if (this.maxTravelDistance.Value >= 0)
      {
        if ((double) this.travelDistance > (double) ((int) (NetFieldBase<int, NetInt>) this.maxTravelDistance - 128))
          this.startingAlpha = (float) (((double) (int) (NetFieldBase<int, NetInt>) this.maxTravelDistance - (double) this.travelDistance) / 128.0);
        if ((double) this.travelDistance >= (double) (int) (NetFieldBase<int, NetInt>) this.maxTravelDistance)
        {
          if (this.hasLit)
            Utility.removeLightSource(this.lightID);
          return true;
        }
      }
      if (this.isColliding(location) && (this.travelTime > 100 || this.ignoreTravelGracePeriod.Value))
      {
        if ((int) (NetFieldBase<int, NetInt>) this.bouncesLeft <= 0 || Game1.player.GetBoundingBox().Intersects(this.getBoundingBox()))
          return this.behaviorOnCollision(location);
        --this.bouncesLeft.Value;
        bool[] flagArray = Utility.horizontalOrVerticalCollisionDirections(this.getBoundingBox(), this.theOneWhoFiredMe.Get(location), true);
        if (flagArray[0])
          this.xVelocity.Value = -(float) (NetFieldBase<float, NetFloat>) this.xVelocity;
        if (flagArray[1])
          this.yVelocity.Value = -(float) (NetFieldBase<float, NetFloat>) this.yVelocity;
      }
      return false;
    }

    private void updateTail(GameTime time)
    {
      this.tailCounter -= time.ElapsedGameTime.Milliseconds;
      if (this.tailCounter > 0)
        return;
      this.tailCounter = 50;
      this.tail.Enqueue((Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position);
      if (this.tail.Count <= (int) (NetFieldBase<int, NetInt>) this.tailLength)
        return;
      this.tail.Dequeue();
    }

    public virtual bool isColliding(GameLocation location)
    {
      if (!location.isTileOnMap(this.position.Value / 64f) || !(bool) (NetFieldBase<bool, NetBool>) this.ignoreLocationCollision && location.isCollidingPosition(this.getBoundingBox(), Game1.viewport, false, 0, true, this.theOneWhoFiredMe.Get(location), false, true) || !(bool) (NetFieldBase<bool, NetBool>) this.damagesMonsters && Game1.player.GetBoundingBox().Intersects(this.getBoundingBox()))
        return true;
      return (bool) (NetFieldBase<bool, NetBool>) this.damagesMonsters && location.doesPositionCollideWithCharacter(this.getBoundingBox()) != null;
    }

    public abstract void updatePosition(GameTime time);

    public virtual Rectangle getBoundingBox()
    {
      Vector2 vector2 = this.position.Value;
      int num = (int) ((double) (Projectile.boundingBoxWidth + ((bool) (NetFieldBase<bool, NetBool>) this.damagesMonsters ? 8 : 0)) * (double) this.localScale);
      return new Rectangle((int) vector2.X + 32 - num / 2, (int) vector2.Y + 32 - num / 2, num, num);
    }

    public virtual void draw(SpriteBatch b)
    {
      float scale = 4f * this.localScale;
      float startingAlpha = this.startingAlpha;
      b.Draw((bool) (NetFieldBase<bool, NetBool>) this.spriteFromObjectSheet ? Game1.objectSpriteSheet : Projectile.projectileSheet, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position + new Vector2(0.0f, -(float) (NetFieldBase<float, NetFloat>) this.height) + new Vector2(32f, 32f)), new Rectangle?(Game1.getSourceRectForStandardTileSheet((bool) (NetFieldBase<bool, NetBool>) this.spriteFromObjectSheet ? Game1.objectSpriteSheet : Projectile.projectileSheet, (int) (NetFieldBase<int, NetInt>) this.currentTileSheetIndex, 16, 16)), this.color.Value * this.startingAlpha, this.rotation, new Vector2(8f, 8f), scale, SpriteEffects.None, (float) (((double) this.position.Y + 96.0) / 10000.0));
      if ((double) this.height.Value > 0.0)
        b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position + new Vector2(32f, 32f)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * startingAlpha * 0.75f, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 2f, SpriteEffects.None, (float) (((double) this.position.Y - 1.0) / 10000.0));
      for (int index = this.tail.Count - 1; index >= 0; --index)
      {
        b.Draw((bool) (NetFieldBase<bool, NetBool>) this.spriteFromObjectSheet ? Game1.objectSpriteSheet : Projectile.projectileSheet, Game1.GlobalToLocal(Game1.viewport, Vector2.Lerp(index == this.tail.Count - 1 ? (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.position : this.tail.ElementAt<Vector2>(index + 1), this.tail.ElementAt<Vector2>(index), (float) this.tailCounter / 50f) + new Vector2(0.0f, -(float) (NetFieldBase<float, NetFloat>) this.height) + new Vector2(32f, 32f)), new Rectangle?(Game1.getSourceRectForStandardTileSheet((bool) (NetFieldBase<bool, NetBool>) this.spriteFromObjectSheet ? Game1.objectSpriteSheet : Projectile.projectileSheet, (int) (NetFieldBase<int, NetInt>) this.currentTileSheetIndex, 16, 16)), this.color.Value * startingAlpha, this.rotation, new Vector2(8f, 8f), scale, SpriteEffects.None, (float) (((double) this.position.Y - (double) (this.tail.Count - index) + 96.0) / 10000.0));
        startingAlpha -= 1f / (float) this.tail.Count;
        scale = 0.8f * (float) (4 - 4 / (index + 4));
      }
    }
  }
}
