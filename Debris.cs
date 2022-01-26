// Decompiled with JetBrains decompiler
// Type: StardewValley.Debris
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Quests;
using StardewValley.Tools;
using System;

namespace StardewValley
{
  public class Debris : INetObject<NetFields>
  {
    public const int copperDebris = 0;
    public const int ironDebris = 2;
    public const int coalDebris = 4;
    public const int goldDebris = 6;
    public const int coinsDebris = 8;
    public const int iridiumDebris = 10;
    public const int woodDebris = 12;
    public const int stoneDebris = 14;
    public const int fuelDebris = 28;
    public const int quartzDebris = 30;
    public const int bigStoneDebris = 32;
    public const int bigWoodDebris = 34;
    public const int timesToBounce = 2;
    public const int minMoneyPerCoin = 10;
    public const int maxMoneyPerCoin = 40;
    public const float gravity = 0.4f;
    public const float timeToWaitBeforeRemoval = 600f;
    public const int marginForChunkPickup = 64;
    public const int white = 10000;
    public const int green = 100001;
    public const int blue = 100002;
    public const int red = 100003;
    public const int yellow = 100004;
    public const int black = 100005;
    public const int charcoal = 100007;
    public const int gray = 100006;
    private readonly NetObjectShrinkList<Chunk> chunks = new NetObjectShrinkList<Chunk>();
    public readonly NetInt chunkType = new NetInt();
    public readonly NetInt sizeOfSourceRectSquares = new NetInt(8);
    private readonly NetInt netItemQuality = new NetInt();
    private readonly NetInt netChunkFinalYLevel = new NetInt();
    private readonly NetInt netChunkFinalYTarget = new NetInt();
    public float timeSinceDoneBouncing;
    public readonly NetFloat scale = new NetFloat(1f).Interpolated(true, true);
    protected NetBool _chunksMoveTowardsPlayer = new NetBool(false).Interpolated(false, false);
    public readonly NetLong DroppedByPlayerID = new NetLong().Interpolated(false, false);
    private bool movingUp;
    public readonly NetBool floppingFish = new NetBool();
    public bool isFishable;
    public bool movingFinalYLevel;
    public readonly NetEnum<Debris.DebrisType> debrisType = new NetEnum<Debris.DebrisType>(Debris.DebrisType.CHUNKS);
    public readonly NetString debrisMessage = new NetString("");
    public readonly NetColor nonSpriteChunkColor = new NetColor(Color.White);
    public readonly NetColor chunksColor = new NetColor();
    public readonly NetString spriteChunkSheetName = new NetString();
    private Texture2D _spriteChunkSheet;
    private readonly NetRef<Item> netItem = new NetRef<Item>();
    public Character toHover;
    public readonly NetFarmerRef player = new NetFarmerRef();
    private float relativeXPosition;

    public int itemQuality
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netItemQuality;
      set => this.netItemQuality.Value = value;
    }

    public int chunkFinalYLevel
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netChunkFinalYLevel;
      set => this.netChunkFinalYLevel.Value = value;
    }

    public int chunkFinalYTarget
    {
      get => (int) (NetFieldBase<int, NetInt>) this.netChunkFinalYTarget;
      set => this.netChunkFinalYTarget.Value = value;
    }

    public bool chunksMoveTowardPlayer
    {
      get => this._chunksMoveTowardsPlayer.Value;
      set => this._chunksMoveTowardsPlayer.Value = value;
    }

    public Texture2D spriteChunkSheet
    {
      get
      {
        if (this._spriteChunkSheet == null && (NetFieldBase<string, NetString>) this.spriteChunkSheetName != (NetString) null)
          this._spriteChunkSheet = Game1.content.Load<Texture2D>((string) (NetFieldBase<string, NetString>) this.spriteChunkSheetName);
        return this._spriteChunkSheet;
      }
    }

    public Item item
    {
      get => (Item) (NetFieldBase<Item, NetRef<Item>>) this.netItem;
      set => this.netItem.Value = value;
    }

    public NetFields NetFields { get; } = new NetFields();

    public Debris()
    {
      this.NetFields.AddFields((INetSerializable) this.chunks, (INetSerializable) this.chunkType, (INetSerializable) this.sizeOfSourceRectSquares, (INetSerializable) this.netItemQuality, (INetSerializable) this.netChunkFinalYLevel, (INetSerializable) this.netChunkFinalYTarget, (INetSerializable) this.scale, (INetSerializable) this.floppingFish, (INetSerializable) this.debrisType, (INetSerializable) this.debrisMessage, (INetSerializable) this.nonSpriteChunkColor, (INetSerializable) this.chunksColor, (INetSerializable) this.spriteChunkSheetName, (INetSerializable) this.netItem, (INetSerializable) this.player.NetFields, (INetSerializable) this.DroppedByPlayerID, (INetSerializable) this._chunksMoveTowardsPlayer);
      this.player.Delayed(false);
    }

    public NetObjectShrinkList<Chunk> Chunks => this.chunks;

    public Debris(int objectIndex, Vector2 debrisOrigin, Vector2 playerPosition)
      : this(objectIndex, 1, debrisOrigin, playerPosition)
    {
      if ((objectIndex > 0 ? Game1.objectInformation[objectIndex].Split('/')[3].Split(' ')[0] : "Crafting").Equals("Arch"))
        this.debrisType.Value = Debris.DebrisType.ARCHAEOLOGY;
      else
        this.debrisType.Value = Debris.DebrisType.OBJECT;
      if (objectIndex == 92)
        this.debrisType.Value = Debris.DebrisType.RESOURCE;
      if (Game1.player.speed >= 5)
      {
        for (int index = 0; index < this.chunks.Count; ++index)
          this.chunks[index].xVelocity.Value *= Game1.player.FacingDirection == 1 || Game1.player.FacingDirection == 3 ? 1f : 1f;
      }
      this.chunks[0].debrisType = objectIndex;
    }

    public Debris(
      int number,
      Vector2 debrisOrigin,
      Color messageColor,
      float scale,
      Character toHover)
      : this(-1, 1, debrisOrigin, Game1.player.Position)
    {
      this.chunkType.Value = number;
      this.debrisType.Value = Debris.DebrisType.NUMBERS;
      this.nonSpriteChunkColor.Value = messageColor;
      this.chunks[0].scale = scale;
      this.toHover = toHover;
      this.chunks[0].xVelocity.Value = (float) Game1.random.Next(-1, 2);
    }

    public Debris(Item item, Vector2 debrisOrigin)
    {
      Vector2 debrisOrigin1 = debrisOrigin;
      Rectangle boundingBox = Game1.player.GetBoundingBox();
      double x = (double) boundingBox.Center.X;
      boundingBox = Game1.player.GetBoundingBox();
      double y = (double) boundingBox.Center.Y;
      Vector2 playerPosition = new Vector2((float) x, (float) y);
      // ISSUE: explicit constructor call
      this.\u002Ector(-2, 1, debrisOrigin1, playerPosition);
      this.item = item;
      item.resetState();
    }

    public Debris(Item item, Vector2 debrisOrigin, Vector2 targetLocation)
      : this(-2, 1, debrisOrigin, targetLocation)
    {
      this.item = item;
      item.resetState();
    }

    public Debris(
      string message,
      int numberOfChunks,
      Vector2 debrisOrigin,
      Color messageColor,
      float scale,
      float rotation)
      : this(-1, numberOfChunks, debrisOrigin, Game1.player.Position)
    {
      this.debrisType.Value = Debris.DebrisType.LETTERS;
      this.debrisMessage.Value = message;
      this.nonSpriteChunkColor.Value = messageColor;
      this.chunks[0].rotation = rotation;
      this.chunks[0].scale = scale;
    }

    public Debris(string spriteSheet, int numberOfChunks, Vector2 debrisOrigin)
      : this(-1, numberOfChunks, debrisOrigin, Game1.player.Position)
    {
      this.debrisType.Value = Debris.DebrisType.SPRITECHUNKS;
      this.spriteChunkSheetName.Value = spriteSheet;
      for (int index = 0; index < this.chunks.Count; ++index)
      {
        Chunk chunk = this.chunks[index];
        chunk.xSpriteSheet.Value = Game1.random.Next(0, 56);
        chunk.ySpriteSheet.Value = Game1.random.Next(0, 88);
        chunk.scale = 1f;
      }
    }

    public Debris(
      string spriteSheet,
      Rectangle sourceRect,
      int numberOfChunks,
      Vector2 debrisOrigin)
      : this(-1, numberOfChunks, debrisOrigin, Game1.player.Position)
    {
      this.debrisType.Value = Debris.DebrisType.SPRITECHUNKS;
      this.spriteChunkSheetName.Value = spriteSheet;
      for (int index = 0; index < this.chunks.Count; ++index)
      {
        Chunk chunk = this.chunks[index];
        chunk.xSpriteSheet.Value = Game1.random.Next(sourceRect.X, sourceRect.X + sourceRect.Width - 4);
        chunk.ySpriteSheet.Value = Game1.random.Next(sourceRect.Y, sourceRect.Y + sourceRect.Width - 4);
        chunk.scale = 1f;
      }
    }

    public Debris(
      string spriteSheet,
      Rectangle sourceRect,
      int numberOfChunks,
      Vector2 debrisOrigin,
      Vector2 playerPosition,
      int groundLevel,
      int sizeOfSourceRectSquares)
      : this(-1, numberOfChunks, debrisOrigin, Game1.player.Position)
    {
      this.sizeOfSourceRectSquares.Value = sizeOfSourceRectSquares;
      this.debrisType.Value = Debris.DebrisType.SPRITECHUNKS;
      this.spriteChunkSheetName.Value = spriteSheet;
      for (int index = 0; index < this.chunks.Count; ++index)
      {
        Chunk chunk = this.chunks[index];
        chunk.xSpriteSheet.Value = Game1.random.Next(2) * sizeOfSourceRectSquares + sourceRect.X;
        chunk.ySpriteSheet.Value = Game1.random.Next(2) * sizeOfSourceRectSquares + sourceRect.Y;
        chunk.rotationVelocity = Game1.random.NextDouble() < 0.5 ? 3.141593f / (float) Game1.random.Next(-32, -16) : 3.141593f / (float) Game1.random.Next(16, 32);
        chunk.xVelocity.Value *= 1.2f;
        chunk.yVelocity.Value *= 1.2f;
        chunk.scale = 4f;
      }
    }

    public Debris(
      string spriteSheet,
      Rectangle sourceRect,
      int numberOfChunks,
      Vector2 debrisOrigin,
      Vector2 playerPosition,
      int groundLevel)
      : this(-1, numberOfChunks, debrisOrigin, playerPosition)
    {
      this.debrisType.Value = Debris.DebrisType.SPRITECHUNKS;
      this.spriteChunkSheetName.Value = spriteSheet;
      for (int index = 0; index < this.chunks.Count; ++index)
      {
        Chunk chunk = this.chunks[index];
        chunk.xSpriteSheet.Value = Game1.random.Next(sourceRect.X, sourceRect.X + sourceRect.Width - 4);
        chunk.ySpriteSheet.Value = Game1.random.Next(sourceRect.Y, sourceRect.Y + sourceRect.Width - 4);
        chunk.scale = 1f;
      }
      this.chunkFinalYLevel = groundLevel;
    }

    public Debris(
      int type,
      int numberOfChunks,
      Vector2 debrisOrigin,
      Vector2 playerPosition,
      int groundLevel,
      int color = -1)
      : this(-1, numberOfChunks, debrisOrigin, playerPosition)
    {
      this.debrisType.Value = Debris.DebrisType.CHUNKS;
      for (int index = 0; index < this.chunks.Count; ++index)
        this.chunks[index].debrisType = type;
      this.chunkType.Value = type;
      this.chunksColor.Value = this.getColorForDebris(color == -1 ? type : color);
    }

    public virtual bool isEssentialItem() => this.item != null && Utility.IsNormalObjectAtParentSheetIndex(this.item, 73) || this.item != null && !this.item.canBeTrashed();

    public virtual bool collect(Farmer farmer, Chunk chunk = null)
    {
      if (chunk == null)
      {
        if (this.chunks.Count <= 0)
          return false;
        chunk = this.chunks[0];
      }
      int num = (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.ARCHAEOLOGY || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.OBJECT ? chunk.debrisType : chunk.debrisType - chunk.debrisType % 2;
      if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.ARCHAEOLOGY)
        Game1.farmerFindsArtifact(chunk.debrisType);
      else if (this.item != null)
      {
        Item obj = this.item;
        this.item = (Item) null;
        if (!farmer.addItemToInventoryBool(obj))
        {
          this.item = obj;
          return false;
        }
      }
      else if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.CHUNKS || num != 8)
      {
        if (num <= -10000)
        {
          if (!farmer.addItemToInventoryBool((Item) new MeleeWeapon(num)))
            return false;
        }
        else if (num <= 0)
        {
          if (!farmer.addItemToInventoryBool((Item) new Object(Vector2.Zero, -num)))
            return false;
        }
        else
        {
          Farmer farmer1 = farmer;
          Object @object;
          if (num != 93 && num != 94)
            @object = new Object(Vector2.Zero, num, 1)
            {
              Quality = this.itemQuality
            };
          else
            @object = (Object) new Torch(Vector2.Zero, 1, num);
          if (!farmer1.addItemToInventoryBool((Item) @object))
            return false;
        }
      }
      return true;
    }

    public Color getColorForDebris(int type)
    {
      switch (type)
      {
        case 12:
          return new Color(170, 106, 46);
        case 100001:
        case 100006:
          return Color.LightGreen;
        case 100002:
          return Color.LightBlue;
        case 100003:
          return Color.Red;
        case 100004:
          return Color.Yellow;
        case 100005:
          return Color.Black;
        case 100007:
          return Color.DimGray;
        default:
          return Color.White;
      }
    }

    public Debris(
      int debrisType,
      int numberOfChunks,
      Vector2 debrisOrigin,
      Vector2 playerPosition)
      : this(debrisType, numberOfChunks, debrisOrigin, playerPosition, 1f)
    {
    }

    public Debris(
      int debrisType,
      int numberOfChunks,
      Vector2 debrisOrigin,
      Vector2 playerPosition,
      float velocityMultiplyer)
      : this()
    {
      switch (debrisType)
      {
        case 0:
        case 378:
          debrisType = 378;
          this.debrisType.Value = Debris.DebrisType.RESOURCE;
          break;
        case 2:
        case 380:
          debrisType = 380;
          this.debrisType.Value = Debris.DebrisType.RESOURCE;
          break;
        case 4:
        case 382:
          debrisType = 382;
          this.debrisType.Value = Debris.DebrisType.RESOURCE;
          break;
        case 6:
        case 384:
          debrisType = 384;
          this.debrisType.Value = Debris.DebrisType.RESOURCE;
          break;
        case 8:
          this.debrisType.Value = Debris.DebrisType.CHUNKS;
          break;
        case 10:
        case 386:
          debrisType = 386;
          this.debrisType.Value = Debris.DebrisType.RESOURCE;
          break;
        case 12:
        case 388:
          debrisType = 388;
          this.debrisType.Value = Debris.DebrisType.RESOURCE;
          break;
        case 14:
        case 390:
          debrisType = 390;
          this.debrisType.Value = Debris.DebrisType.RESOURCE;
          break;
        default:
          this.debrisType.Value = Debris.DebrisType.OBJECT;
          break;
      }
      if (debrisType != -1)
        playerPosition -= (playerPosition - debrisOrigin) * 2f;
      this.chunkType.Value = debrisType;
      this.floppingFish.Value = Game1.objectInformation.ContainsKey(debrisType) && Game1.objectInformation[debrisType].Split('/')[3].Contains("-4") && !Game1.objectInformation[debrisType].Split('/')[0].Equals("Mussel");
      this.isFishable = Game1.objectInformation.ContainsKey(debrisType) && Game1.objectInformation[debrisType].Split('/')[3].Contains("Fish");
      int num1;
      int num2;
      int num3;
      int num4;
      if ((double) playerPosition.Y >= (double) debrisOrigin.Y - 32.0 && (double) playerPosition.Y <= (double) debrisOrigin.Y + 32.0)
      {
        this.chunkFinalYLevel = (int) debrisOrigin.Y - 32;
        num1 = 220;
        num2 = 250;
        if ((double) playerPosition.X < (double) debrisOrigin.X)
        {
          num3 = 20;
          num4 = 140;
        }
        else
        {
          num3 = -140;
          num4 = -20;
        }
      }
      else if ((double) playerPosition.Y < (double) debrisOrigin.Y - 32.0)
      {
        this.chunkFinalYLevel = (int) debrisOrigin.Y + (int) (32.0 * (double) velocityMultiplyer);
        num1 = 150;
        num2 = 200;
        num3 = -50;
        num4 = 50;
      }
      else
      {
        this.movingFinalYLevel = true;
        this.chunkFinalYLevel = (int) debrisOrigin.Y - 1;
        this.chunkFinalYTarget = (int) debrisOrigin.Y - (int) (96.0 * (double) velocityMultiplyer);
        this.movingUp = true;
        num1 = 350;
        num2 = 400;
        num3 = -50;
        num4 = 50;
      }
      debrisOrigin.X -= 32f;
      debrisOrigin.Y -= 32f;
      int minValue1 = (int) ((double) num3 * (double) velocityMultiplyer);
      int maxValue1 = (int) ((double) num4 * (double) velocityMultiplyer);
      int minValue2 = (int) ((double) num1 * (double) velocityMultiplyer);
      int maxValue2 = (int) ((double) num2 * (double) velocityMultiplyer);
      for (int index = 0; index < numberOfChunks; ++index)
        this.chunks.Add(new Chunk(debrisOrigin, (float) Game1.recentMultiplayerRandom.Next(minValue1, maxValue1) / 40f, (float) Game1.recentMultiplayerRandom.Next(minValue2, maxValue2) / 40f, Game1.recentMultiplayerRandom.Next(debrisType, debrisType + 2)));
    }

    private Vector2 approximatePosition()
    {
      Vector2 vector2 = new Vector2();
      foreach (Chunk chunk in this.Chunks)
        vector2 += chunk.position.Value;
      return vector2 / (float) this.Chunks.Count;
    }

    private bool playerInRange(Vector2 position, Farmer farmer)
    {
      if (this.isEssentialItem())
        return true;
      int appliedMagneticRadius = farmer.GetAppliedMagneticRadius();
      return (double) Math.Abs(position.X + 32f - (float) farmer.getStandingX()) <= (double) appliedMagneticRadius && (double) Math.Abs(position.Y + 32f - (float) farmer.getStandingY()) <= (double) appliedMagneticRadius;
    }

    private Farmer findBestPlayer(GameLocation location)
    {
      if (location != null && location.isTemp())
        return Game1.player;
      Vector2 position = this.approximatePosition();
      float num1 = float.MaxValue;
      Farmer bestPlayer = (Farmer) null;
      foreach (Farmer farmer in location.farmers)
      {
        if ((farmer.UniqueMultiplayerID != (long) this.DroppedByPlayerID || bestPlayer == null) && this.playerInRange(position, farmer))
        {
          float num2 = (farmer.Position - position).LengthSquared();
          if ((double) num2 < (double) num1 || bestPlayer != null && bestPlayer.UniqueMultiplayerID == (long) this.DroppedByPlayerID)
          {
            bestPlayer = farmer;
            num1 = num2;
          }
        }
      }
      return bestPlayer;
    }

    public bool shouldControlThis(GameLocation location)
    {
      if (Game1.IsMasterGame)
        return true;
      return location != null && location.isTemp();
    }

    public bool updateChunks(GameTime time, GameLocation location)
    {
      if (this.chunks.Count == 0)
        return true;
      this.timeSinceDoneBouncing += (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.timeSinceDoneBouncing >= ((bool) (NetFieldBase<bool, NetBool>) this.floppingFish ? 2500.0 : ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.SPRITECHUNKS || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.NUMBERS ? 1800.0 : 600.0)))
      {
        if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.LETTERS || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.NUMBERS || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.SQUARES || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.SPRITECHUNKS || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.CHUNKS && this.chunks[0].debrisType - this.chunks[0].debrisType % 2 != 8)
          return true;
        if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.ARCHAEOLOGY || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.OBJECT || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.RESOURCE || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.CHUNKS)
          this.chunksMoveTowardPlayer = true;
        this.timeSinceDoneBouncing = 0.0f;
      }
      if (!location.farmers.Any() && !location.isTemp())
        return false;
      Vector2 position = this.approximatePosition();
      Farmer farmer = this.player.Value;
      if (this.isEssentialItem() && this.shouldControlThis(location) && farmer == null)
        farmer = this.findBestPlayer(location);
      if (this.chunksMoveTowardPlayer && !this.isEssentialItem())
      {
        if (this.player.Value != null && this.player.Value == Game1.player && !this.playerInRange(position, this.player.Value))
        {
          this.player.Value = (Farmer) null;
          farmer = (Farmer) null;
        }
        if (this.shouldControlThis(location))
        {
          if (this.player.Value != null && this.player.Value.currentLocation != location)
          {
            this.player.Value = (Farmer) null;
            farmer = (Farmer) null;
          }
          if (farmer == null)
            farmer = this.findBestPlayer(location);
        }
      }
      bool flag1 = false;
      for (int index = this.chunks.Count - 1; index >= 0; --index)
      {
        Chunk chunk = this.chunks[index];
        chunk.position.UpdateExtrapolation(chunk.getSpeed());
        if ((double) chunk.alpha > 0.100000001490116 && ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.SPRITECHUNKS || (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.NUMBERS) && (double) this.timeSinceDoneBouncing > 600.0)
          chunk.alpha = (float) ((1800.0 - (double) this.timeSinceDoneBouncing) / 1000.0);
        if ((double) chunk.position.X < (double) sbyte.MinValue || (double) chunk.position.Y < -64.0 || (double) chunk.position.X >= (double) (location.map.DisplayWidth + 64) || (double) chunk.position.Y >= (double) (location.map.DisplayHeight + 64))
        {
          this.chunks.RemoveAt(index);
        }
        else
        {
          bool flag2 = farmer != null;
          if (flag2)
          {
            switch (this.debrisType.Value)
            {
              case Debris.DebrisType.ARCHAEOLOGY:
              case Debris.DebrisType.OBJECT:
                if (this.item != null)
                {
                  flag2 = farmer.couldInventoryAcceptThisItem(this.item);
                  break;
                }
                flag2 = chunk.debrisType >= 0 ? farmer.couldInventoryAcceptThisObject(chunk.debrisType, 1, this.itemQuality) : farmer.couldInventoryAcceptThisItem((Item) new Object(Vector2.Zero, chunk.debrisType * -1));
                if (chunk.debrisType == 102 && (bool) (NetFieldBase<bool, NetBool>) farmer.hasMenuOpen)
                {
                  flag2 = false;
                  break;
                }
                break;
              case Debris.DebrisType.RESOURCE:
                flag2 = farmer.couldInventoryAcceptThisObject(chunk.debrisType - chunk.debrisType % 2, 1);
                break;
              default:
                flag2 = true;
                break;
            }
            flag1 |= flag2;
            if (flag2 && this.shouldControlThis(location))
              this.player.Value = farmer;
          }
          if (((this.chunksMoveTowardPlayer ? 1 : (this.isFishable ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 && this.player.Value != null)
          {
            if (this.player.Value.IsLocalPlayer)
            {
              if ((double) chunk.position.X < (double) this.player.Value.Position.X - 12.0)
                chunk.xVelocity.Value = Math.Min((float) (NetFieldBase<float, NetFloat>) chunk.xVelocity + 0.8f, 8f);
              else if ((double) chunk.position.X > (double) this.player.Value.Position.X + 12.0)
                chunk.xVelocity.Value = Math.Max((float) (NetFieldBase<float, NetFloat>) chunk.xVelocity - 0.8f, -8f);
              if ((double) chunk.position.Y + 32.0 < (double) (this.player.Value.getStandingY() - 12))
                chunk.yVelocity.Value = Math.Max((float) (NetFieldBase<float, NetFloat>) chunk.yVelocity - 0.8f, -8f);
              else if ((double) chunk.position.Y + 32.0 > (double) (this.player.Value.getStandingY() + 12))
                chunk.yVelocity.Value = Math.Min((float) (NetFieldBase<float, NetFloat>) chunk.yVelocity + 0.8f, 8f);
              chunk.position.X += (float) (NetFieldBase<float, NetFloat>) chunk.xVelocity;
              chunk.position.Y -= (float) (NetFieldBase<float, NetFloat>) chunk.yVelocity;
              if ((double) Math.Abs(chunk.position.X + 32f - (float) this.player.Value.getStandingX()) <= 64.0 && (double) Math.Abs(chunk.position.Y + 32f - (float) this.player.Value.getStandingY()) <= 64.0)
              {
                Item obj = this.item;
                if (this.collect((Farmer) this.player, chunk))
                {
                  if ((double) Game1.debrisSoundInterval <= 0.0)
                  {
                    Game1.debrisSoundInterval = 10f;
                    if ((obj == null || (int) (NetFieldBase<int, NetInt>) obj.parentSheetIndex != 73) && chunk.debrisType != 73)
                      location.localSound("coin");
                  }
                  this.chunks.RemoveAt(index);
                }
              }
            }
          }
          else
          {
            if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.NUMBERS && this.toHover != null)
            {
              this.relativeXPosition += (float) (NetFieldBase<float, NetFloat>) chunk.xVelocity;
              chunk.position.X = this.toHover.Position.X + 32f + this.relativeXPosition;
              chunk.scale = Math.Min(2f, Math.Max(1f, (float) (0.899999976158142 + (double) Math.Abs(chunk.position.Y - (float) this.chunkFinalYLevel) / 128.0)));
              this.chunkFinalYLevel = this.toHover.getStandingY() + 8;
              if ((double) this.timeSinceDoneBouncing > 250.0)
                chunk.alpha = Math.Max(0.0f, chunk.alpha - 0.033f);
              if (!(this.toHover is Farmer) && !this.nonSpriteChunkColor.Equals(Color.Yellow) && !this.nonSpriteChunkColor.Equals(Color.Green))
              {
                this.nonSpriteChunkColor.R = (byte) Math.Max((double) Math.Min((int) byte.MaxValue, 200 + (int) (NetFieldBase<int, NetInt>) this.chunkType), Math.Min((double) Math.Min((int) byte.MaxValue, 220 + (int) (NetFieldBase<int, NetInt>) this.chunkType), 400.0 * Math.Sin((double) this.timeSinceDoneBouncing / (256.0 * Math.PI) + Math.PI / 12.0)));
                this.nonSpriteChunkColor.G = (byte) Math.Max((double) (150 - (int) (NetFieldBase<int, NetInt>) this.chunkType), Math.Min((double) ((int) byte.MaxValue - (int) (NetFieldBase<int, NetInt>) this.chunkType), this.nonSpriteChunkColor.R > (byte) 220 ? 300.0 * Math.Sin((double) this.timeSinceDoneBouncing / (256.0 * Math.PI) + Math.PI / 12.0) : 0.0));
                this.nonSpriteChunkColor.B = (byte) Math.Max(0, Math.Min((int) byte.MaxValue, this.nonSpriteChunkColor.G > (byte) 200 ? (int) this.nonSpriteChunkColor.G - 20 : 0));
              }
            }
            chunk.position.X += (float) (NetFieldBase<float, NetFloat>) chunk.xVelocity;
            chunk.position.Y -= (float) (NetFieldBase<float, NetFloat>) chunk.yVelocity;
            if (this.movingFinalYLevel)
            {
              this.chunkFinalYLevel -= (int) Math.Ceiling((double) (float) (NetFieldBase<float, NetFloat>) chunk.yVelocity / 2.0);
              if (this.chunkFinalYLevel <= this.chunkFinalYTarget)
              {
                this.chunkFinalYLevel = this.chunkFinalYTarget;
                this.movingFinalYLevel = false;
              }
            }
            if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType == Debris.DebrisType.SQUARES && (double) chunk.position.Y < (double) (this.chunkFinalYLevel - 96) && Game1.random.NextDouble() < 0.1)
            {
              chunk.position.Y = (float) (this.chunkFinalYLevel - Game1.random.Next(1, 21));
              chunk.yVelocity.Value = (float) Game1.random.Next(30, 80) / 40f;
              chunk.position.X = (float) Game1.random.Next((int) ((double) chunk.position.X - (double) chunk.position.X % 64.0 + 1.0), (int) ((double) chunk.position.X - (double) chunk.position.X % 64.0 + 64.0));
            }
            if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.SQUARES && chunk.bounces <= ((bool) (NetFieldBase<bool, NetBool>) this.floppingFish ? 65 : 2))
              chunk.yVelocity.Value -= 0.4f;
            bool flag3 = false;
            if ((double) chunk.position.Y >= (double) this.chunkFinalYLevel && (bool) (NetFieldBase<bool, NetBool>) chunk.hasPassedRestingLineOnce && chunk.bounces <= ((bool) (NetFieldBase<bool, NetBool>) this.floppingFish ? 65 : 2))
            {
              Point p = new Point((int) chunk.position.X / 64, this.chunkFinalYLevel / 64);
              if (Game1.currentLocation is IslandNorth && (this.debrisType.Value == Debris.DebrisType.ARCHAEOLOGY || this.debrisType.Value == Debris.DebrisType.OBJECT || this.debrisType.Value == Debris.DebrisType.RESOURCE || this.debrisType.Value == Debris.DebrisType.CHUNKS) && Game1.currentLocation.isTileOnMap(p.X, p.Y) && Game1.currentLocation.getTileIndexAt(p, "Back") == -1)
                this.chunkFinalYLevel += 48;
              if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.LETTERS && (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.NUMBERS && (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.SPRITECHUNKS && ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.CHUNKS || chunk.debrisType - chunk.debrisType % 2 == 8) && this.shouldControlThis(location))
                location.playSound("shiny4");
              ++chunk.bounces;
              if ((bool) (NetFieldBase<bool, NetBool>) this.floppingFish)
              {
                chunk.yVelocity.Value = Math.Abs((float) (NetFieldBase<float, NetFloat>) chunk.yVelocity) * (!this.movingUp || chunk.bounces >= 2 ? 0.9f : 0.6f);
                chunk.xVelocity.Value = (float) Game1.random.Next(-250, 250) / 100f;
              }
              else
              {
                chunk.yVelocity.Value = Math.Abs((float) ((double) (float) (NetFieldBase<float, NetFloat>) chunk.yVelocity * 2.0 / 3.0));
                chunk.rotationVelocity = Game1.random.NextDouble() < 0.5 ? chunk.rotationVelocity / 2f : (float) (-(double) chunk.rotationVelocity * 2.0 / 3.0);
                chunk.xVelocity.Value -= (float) (NetFieldBase<float, NetFloat>) chunk.xVelocity / 2f;
              }
              Vector2 chunkTile = new Vector2((float) (int) (((double) chunk.position.X + 32.0) / 64.0), (float) (int) (((double) chunk.position.Y + 32.0) / 64.0));
              if ((Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.LETTERS && (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.SPRITECHUNKS && (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType != Debris.DebrisType.NUMBERS && location.doesTileSinkDebris((int) chunkTile.X, (int) chunkTile.Y, (Debris.DebrisType) (NetFieldBase<Debris.DebrisType, NetEnum<Debris.DebrisType>>) this.debrisType))
                flag3 = location.sinkDebris(this, chunkTile, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) chunk.position);
            }
            int num1 = (int) (((double) chunk.position.X + 32.0) / 64.0);
            int num2 = (int) (((double) chunk.position.Y + 32.0) / 64.0);
            if (!chunk.hitWall && location.Map.GetLayer("Buildings").Tiles[num1, num2] != null && location.doesTileHaveProperty(num1, num2, "Passable", "Buildings") == null || location.Map.GetLayer("Back").Tiles[num1, num2] == null)
            {
              chunk.xVelocity.Value = -(float) (NetFieldBase<float, NetFloat>) chunk.xVelocity;
              chunk.hitWall = true;
            }
            if ((double) chunk.position.Y < (double) this.chunkFinalYLevel)
              chunk.hasPassedRestingLineOnce.Value = true;
            if (chunk.bounces > ((bool) (NetFieldBase<bool, NetBool>) this.floppingFish ? 65 : 2))
            {
              chunk.yVelocity.Value = 0.0f;
              chunk.xVelocity.Value = 0.0f;
              chunk.rotationVelocity = 0.0f;
            }
            chunk.rotation += chunk.rotationVelocity;
            if (flag3)
              this.chunks.RemoveAt(index);
          }
        }
      }
      if (!flag1 && this.shouldControlThis(location))
        this.player.Value = (Farmer) null;
      return this.chunks.Count == 0;
    }

    public static string getNameOfDebrisTypeFromIntId(int id)
    {
      switch (id)
      {
        case 0:
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.621");
        case 2:
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.622");
        case 4:
        case 5:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.623");
        case 6:
        case 7:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.624");
        case 8:
        case 9:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.625");
        case 10:
        case 11:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.626");
        case 12:
        case 13:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.627");
        case 14:
        case 15:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.628");
        case 28:
        case 29:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.629");
        case 30:
        case 31:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.630");
        default:
          return "???";
      }
    }

    public static bool getDebris(int which, int howMuch)
    {
      switch (which)
      {
        case 0:
          Game1.player.CopperPieces += howMuch;
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.621"), howMuch, true, Color.Sienna));
          if (howMuch > 0)
          {
            Game1.stats.CopperFound += (uint) howMuch;
            break;
          }
          break;
        case 2:
          Game1.player.IronPieces += howMuch;
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.622"), howMuch, true, Color.LightSlateGray));
          if (howMuch > 0)
          {
            Game1.stats.IronFound += (uint) howMuch;
            break;
          }
          break;
        case 4:
          Game1.player.CoalPieces += howMuch;
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.623"), howMuch, true, Color.DimGray));
          if (howMuch > 0)
          {
            Game1.stats.CoalFound += (uint) howMuch;
            break;
          }
          break;
        case 6:
          Game1.player.GoldPieces += howMuch;
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.624"), howMuch, true, Color.Gold));
          if (howMuch > 0)
          {
            Game1.stats.GoldFound += (uint) howMuch;
            break;
          }
          break;
        case 8:
          int num = Game1.random.Next(10, 50) * howMuch;
          Game1.player.Money += num - num % 5;
          if (howMuch > 0)
          {
            Game1.stats.CoinsFound += (uint) howMuch;
            break;
          }
          break;
        case 10:
          Game1.player.IridiumPieces += howMuch;
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.626"), howMuch, true, Color.Purple));
          if (howMuch > 0)
          {
            Game1.stats.IridiumFound += (uint) howMuch;
            break;
          }
          break;
        case 12:
          Game1.player.WoodPieces += howMuch;
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.627"), howMuch, true, Color.Tan));
          if (howMuch > 0)
          {
            Game1.stats.SticksChopped += (uint) howMuch;
            break;
          }
          break;
        case 28:
          Game1.player.fuelLantern(howMuch * 2);
          Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Debris.cs.629"), howMuch * 2, true, Color.Goldenrod));
          break;
        default:
          return false;
      }
      if (Game1.questOfTheDay != null && (bool) (NetFieldBase<bool, NetBool>) Game1.questOfTheDay.accepted && !(bool) (NetFieldBase<bool, NetBool>) Game1.questOfTheDay.completed && Game1.questOfTheDay is ResourceCollectionQuest)
        Game1.questOfTheDay.checkIfComplete(number1: which, number2: howMuch);
      return true;
    }

    public enum DebrisType
    {
      CHUNKS,
      LETTERS,
      SQUARES,
      ARCHAEOLOGY,
      OBJECT,
      SPRITECHUNKS,
      RESOURCE,
      NUMBERS,
    }
  }
}
