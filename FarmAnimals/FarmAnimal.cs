// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmAnimal
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley
{
  public class FarmAnimal : Character
  {
    public const byte eatGrassBehavior = 0;
    public const short newHome = 0;
    public const short happy = 1;
    public const short neutral = 2;
    public const short unhappy = 3;
    public const short hungry = 4;
    public const short disturbedByDog = 5;
    public const short leftOutAtNight = 6;
    public const int hitsTillDead = 3;
    public const double chancePerUpdateToChangeDirection = 0.007;
    public const byte fullnessValueOfGrass = 60;
    public const int noWarpTimerTime = 3000;
    public new const double chanceForSound = 0.002;
    public const double chanceToGoOutside = 0.002;
    public const int uniqueDownFrame = 16;
    public const int uniqueRightFrame = 18;
    public const int uniqueUpFrame = 20;
    public const int uniqueLeftFrame = 22;
    public const int pushAccumulatorTimeTillPush = 40;
    public const int timePerUniqueFrame = 500;
    public const byte layHarvestType = 0;
    public const byte grabHarvestType = 1;
    public NetBool isSwimming = new NetBool();
    [XmlIgnore]
    public Vector2 hopOffset = new Vector2(0.0f, 0.0f);
    [XmlElement("defaultProduceIndex")]
    public readonly NetInt defaultProduceIndex = new NetInt();
    [XmlElement("deluxeProduceIndex")]
    public readonly NetInt deluxeProduceIndex = new NetInt();
    [XmlElement("currentProduce")]
    public readonly NetInt currentProduce = new NetInt();
    [XmlElement("friendshipTowardFarmer")]
    public readonly NetInt friendshipTowardFarmer = new NetInt();
    [XmlElement("daysSinceLastFed")]
    public readonly NetInt daysSinceLastFed = new NetInt();
    public int pushAccumulator;
    public int uniqueFrameAccumulator = -1;
    [XmlElement("age")]
    public readonly NetInt age = new NetInt();
    [XmlElement("daysOwned")]
    public readonly NetInt daysOwned = new NetInt(-1);
    [XmlElement("meatIndex")]
    public readonly NetInt meatIndex = new NetInt();
    [XmlElement("health")]
    public readonly NetInt health = new NetInt();
    [XmlElement("price")]
    public readonly NetInt price = new NetInt();
    [XmlElement("produceQuality")]
    public readonly NetInt produceQuality = new NetInt();
    [XmlElement("daysToLay")]
    public readonly NetByte daysToLay = new NetByte();
    [XmlElement("daysSinceLastLay")]
    public readonly NetByte daysSinceLastLay = new NetByte();
    [XmlElement("ageWhenMature")]
    public readonly NetByte ageWhenMature = new NetByte();
    [XmlElement("harvestType")]
    public readonly NetByte harvestType = new NetByte();
    [XmlElement("happiness")]
    public readonly NetByte happiness = new NetByte();
    [XmlElement("fullness")]
    public readonly NetByte fullness = new NetByte();
    [XmlElement("happinessDrain")]
    public readonly NetByte happinessDrain = new NetByte();
    [XmlElement("fullnessDrain")]
    public readonly NetByte fullnessDrain = new NetByte();
    [XmlElement("wasAutoPet")]
    public readonly NetBool wasAutoPet = new NetBool();
    [XmlElement("wasPet")]
    public readonly NetBool wasPet = new NetBool();
    [XmlElement("showDifferentTextureWhenReadyForHarvest")]
    public readonly NetBool showDifferentTextureWhenReadyForHarvest = new NetBool();
    [XmlElement("allowReproduction")]
    public readonly NetBool allowReproduction = new NetBool(true);
    [XmlElement("sound")]
    public readonly NetString sound = new NetString();
    [XmlElement("type")]
    public readonly NetString type = new NetString();
    [XmlElement("buildingTypeILiveIn")]
    public readonly NetString buildingTypeILiveIn = new NetString();
    [XmlElement("toolUsedForHarvest")]
    public readonly NetString toolUsedForHarvest = new NetString();
    [XmlElement("frontBackBoundingBox")]
    public readonly NetRectangle frontBackBoundingBox = new NetRectangle();
    [XmlElement("sidewaysBoundingBox")]
    public readonly NetRectangle sidewaysBoundingBox = new NetRectangle();
    [XmlElement("frontBackSourceRect")]
    public readonly NetRectangle frontBackSourceRect = new NetRectangle();
    [XmlElement("sidewaysSourceRect")]
    public readonly NetRectangle sidewaysSourceRect = new NetRectangle();
    [XmlElement("myID")]
    public readonly NetLong myID = new NetLong();
    [XmlElement("ownerID")]
    public readonly NetLong ownerID = new NetLong();
    [XmlElement("parentId")]
    public readonly NetLong parentId = new NetLong(-1L);
    [XmlIgnore]
    private readonly NetBuildingRef netHome = new NetBuildingRef();
    [XmlElement("homeLocation")]
    public readonly NetVector2 homeLocation = new NetVector2();
    [XmlIgnore]
    public int noWarpTimer;
    [XmlIgnore]
    public int hitGlowTimer;
    [XmlIgnore]
    public int pauseTimer;
    [XmlElement("moodMessage")]
    public readonly NetInt moodMessage = new NetInt();
    [XmlElement("isEating")]
    public readonly NetBool isEating = new NetBool();
    [XmlIgnore]
    private readonly NetEvent1Field<int, NetInt> doFarmerPushEvent = new NetEvent1Field<int, NetInt>();
    [XmlIgnore]
    private readonly NetEvent0 doBuildingPokeEvent = new NetEvent0();
    [XmlIgnore]
    private readonly NetEvent0 doDiveEvent = new NetEvent0();
    private string _displayHouse;
    private string _displayType;
    public static int NumPathfindingThisTick = 0;
    public static int MaxPathfindingPerTick = 1;
    [XmlIgnore]
    public int nextRipple;
    [XmlIgnore]
    public int nextFollowDirectionChange;
    protected FarmAnimal _followTarget;
    protected Point? _followTargetPosition;
    protected float _nextFollowTargetScan = 1f;
    [XmlIgnore]
    public int bobOffset;
    [XmlIgnore]
    protected Vector2 _swimmingVelocity = Vector2.Zero;
    [XmlIgnore]
    public static HashSet<Grass> reservedGrass = new HashSet<Grass>();
    [XmlIgnore]
    public Grass foundGrass;

    [XmlIgnore]
    public Building home
    {
      get => this.netHome.Value;
      set => this.netHome.Value = value;
    }

    [XmlIgnore]
    public string displayHouse
    {
      get
      {
        if (this._displayHouse == null)
        {
          string str;
          Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").TryGetValue((string) (NetFieldBase<string, NetString>) this.type, out str);
          this._displayHouse = (string) (NetFieldBase<string, NetString>) this.buildingTypeILiveIn;
          if (str != null && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
            this._displayHouse = str.Split('/')[26];
        }
        return this._displayHouse;
      }
      set => this._displayHouse = value;
    }

    [XmlIgnore]
    public string displayType
    {
      get
      {
        if (this._displayType == null)
        {
          string str;
          Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").TryGetValue((string) (NetFieldBase<string, NetString>) this.type, out str);
          if (str != null)
            this._displayType = str.Split('/')[25];
        }
        return this._displayType;
      }
      set => this._displayType = value;
    }

    public override string displayName
    {
      get => this.Name;
      set
      {
      }
    }

    public FarmAnimal()
    {
    }

    protected override void initNetFields()
    {
      this.bobOffset = Game1.random.Next(0, 1000);
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.defaultProduceIndex, (INetSerializable) this.deluxeProduceIndex, (INetSerializable) this.currentProduce, (INetSerializable) this.friendshipTowardFarmer, (INetSerializable) this.daysSinceLastFed, (INetSerializable) this.age, (INetSerializable) this.meatIndex, (INetSerializable) this.health, (INetSerializable) this.price, (INetSerializable) this.produceQuality, (INetSerializable) this.daysToLay, (INetSerializable) this.daysSinceLastLay, (INetSerializable) this.ageWhenMature, (INetSerializable) this.harvestType, (INetSerializable) this.happiness, (INetSerializable) this.fullness, (INetSerializable) this.happinessDrain, (INetSerializable) this.fullnessDrain, (INetSerializable) this.wasPet, (INetSerializable) this.wasAutoPet, (INetSerializable) this.showDifferentTextureWhenReadyForHarvest, (INetSerializable) this.allowReproduction, (INetSerializable) this.sound, (INetSerializable) this.type, (INetSerializable) this.buildingTypeILiveIn, (INetSerializable) this.toolUsedForHarvest, (INetSerializable) this.frontBackBoundingBox, (INetSerializable) this.sidewaysBoundingBox, (INetSerializable) this.frontBackSourceRect, (INetSerializable) this.sidewaysSourceRect, (INetSerializable) this.myID, (INetSerializable) this.ownerID, (INetSerializable) this.parentId, (INetSerializable) this.netHome.NetFields, (INetSerializable) this.homeLocation, (INetSerializable) this.moodMessage, (INetSerializable) this.isEating, (INetSerializable) this.doFarmerPushEvent, (INetSerializable) this.doBuildingPokeEvent, (INetSerializable) this.isSwimming, this.doDiveEvent.NetFields, (INetSerializable) this.daysOwned);
      this.position.Field.AxisAlignedMovement = true;
      this.doFarmerPushEvent.onEvent += new AbstractNetEvent1<int>.Event(this.doFarmerPush);
      this.doBuildingPokeEvent.onEvent += new NetEvent0.Event(this.doBuildingPoke);
      this.doDiveEvent.onEvent += new NetEvent0.Event(this.doDive);
      this.isSwimming.fieldChangeVisibleEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((a, b, c) =>
      {
        if (this.isSwimming.Value)
          this.position.Field.AxisAlignedMovement = false;
        else
          this.position.Field.AxisAlignedMovement = true;
      });
      this.name.FilterStringEvent += new NetString.FilterString(Utility.FilterDirtyWords);
    }

    public FarmAnimal(string type, long id, long ownerID)
      : base((AnimatedSprite) null, new Vector2((float) (64 * Game1.random.Next(2, 9)), (float) (64 * Game1.random.Next(4, 8))), 2, type)
    {
      this.ownerID.Value = ownerID;
      this.health.Value = 3;
      if (type.Contains("Chicken") && !type.Equals("Void Chicken") && !type.Equals("Golden Chicken"))
      {
        type = Game1.random.NextDouble() < 0.5 || type.Contains("Brown") ? "Brown Chicken" : "White Chicken";
        if (Game1.player.eventsSeen.Contains(3900074) && Game1.random.NextDouble() < 0.25)
          type = "Blue Chicken";
      }
      if (type.Contains("Cow"))
        type = type.Contains("White") || Game1.random.NextDouble() >= 0.5 && !type.Contains("Brown") ? "White Cow" : "Brown Cow";
      this.myID.Value = id;
      this.type.Value = type;
      this.Name = Dialogue.randomName();
      this.displayName = (string) (NetFieldBase<string, NetString>) this.name;
      this.happiness.Value = byte.MaxValue;
      this.fullness.Value = byte.MaxValue;
      this._nextFollowTargetScan = Utility.RandomFloat(1f, 3f);
      this.reloadData();
    }

    public virtual void reloadData()
    {
      string str1;
      Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").TryGetValue(this.type.Value, out str1);
      if (str1 == null)
        return;
      string[] strArray = str1.Split('/');
      this.daysToLay.Value = Convert.ToByte(strArray[0]);
      this.ageWhenMature.Value = Convert.ToByte(strArray[1]);
      this.defaultProduceIndex.Value = Convert.ToInt32(strArray[2]);
      this.deluxeProduceIndex.Value = Convert.ToInt32(strArray[3]);
      this.sound.Value = strArray[4].Equals("none") ? (string) null : strArray[4];
      this.frontBackBoundingBox.Value = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(strArray[5]), Convert.ToInt32(strArray[6]), Convert.ToInt32(strArray[7]), Convert.ToInt32(strArray[8]));
      this.sidewaysBoundingBox.Value = new Microsoft.Xna.Framework.Rectangle(Convert.ToInt32(strArray[9]), Convert.ToInt32(strArray[10]), Convert.ToInt32(strArray[11]), Convert.ToInt32(strArray[12]));
      this.harvestType.Value = Convert.ToByte(strArray[13]);
      this.showDifferentTextureWhenReadyForHarvest.Value = Convert.ToBoolean(strArray[14]);
      this.buildingTypeILiveIn.Value = strArray[15];
      int int32 = Convert.ToInt32(strArray[16]);
      string str2 = (string) (NetFieldBase<string, NetString>) this.type;
      if ((int) (NetFieldBase<int, NetInt>) this.age < (int) (byte) (NetFieldBase<byte, NetByte>) this.ageWhenMature)
        str2 = "Baby" + (this.type.Value.Equals("Duck") ? "White Chicken" : this.type.Value);
      else if ((bool) (NetFieldBase<bool, NetBool>) this.showDifferentTextureWhenReadyForHarvest && (int) (NetFieldBase<int, NetInt>) this.currentProduce <= 0)
        str2 = "Sheared" + this.type.Value;
      this.Sprite = new AnimatedSprite("Animals\\" + str2, 0, int32, Convert.ToInt32(strArray[17]));
      this.frontBackSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(strArray[16]), Convert.ToInt32(strArray[17]));
      this.sidewaysSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(strArray[18]), Convert.ToInt32(strArray[19]));
      this.fullnessDrain.Value = Convert.ToByte(strArray[20]);
      this.happinessDrain.Value = Convert.ToByte(strArray[21]);
      this.toolUsedForHarvest.Value = strArray[22].Length > 0 ? strArray[22] : "";
      this.meatIndex.Value = Convert.ToInt32(strArray[23]);
      this.price.Value = Convert.ToInt32(strArray[24]);
      if (this.isCoopDweller())
        return;
      this.Sprite.textureUsesFlippedRightForLeft = true;
    }

    public string shortDisplayType()
    {
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.en:
          return ((IEnumerable<string>) this.displayType.Split(' ')).Last<string>();
        case LocalizedContentManager.LanguageCode.ja:
          if (this.displayType.Contains("トリ"))
            return "トリ";
          if (this.displayType.Contains("ウシ"))
            return "ウシ";
          return !this.displayType.Contains("ブタ") ? this.displayType : "ブタ";
        case LocalizedContentManager.LanguageCode.ru:
          if (this.displayType.ToLower().Contains("курица"))
            return "Курица";
          return !this.displayType.ToLower().Contains("корова") ? this.displayType : "Корова";
        case LocalizedContentManager.LanguageCode.zh:
          if (this.displayType.Contains("鸡"))
            return "鸡";
          if (this.displayType.Contains("牛"))
            return "牛";
          return !this.displayType.Contains("猪") ? this.displayType : "猪";
        case LocalizedContentManager.LanguageCode.pt:
        case LocalizedContentManager.LanguageCode.es:
          return ((IEnumerable<string>) this.displayType.Split(' ')).First<string>();
        case LocalizedContentManager.LanguageCode.de:
          return ((IEnumerable<string>) ((IEnumerable<string>) this.displayType.Split(' ')).Last<string>().Split('-')).Last<string>();
        default:
          return this.displayType;
      }
    }

    public bool isCoopDweller() => this.home != null && this.home is Coop;

    public Microsoft.Xna.Framework.Rectangle GetHarvestBoundingBox()
    {
      Vector2 position = this.Position;
      return new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + (double) (this.Sprite.getWidth() * 4 / 2) - 32.0 + 4.0), (int) ((double) position.Y + (double) (this.Sprite.getHeight() * 4) - 64.0 - 24.0), 56, 72);
    }

    public Microsoft.Xna.Framework.Rectangle GetCursorPetBoundingBox()
    {
      Vector2 position = this.Position;
      if (this.type.Contains("Chicken"))
        return new Microsoft.Xna.Framework.Rectangle((int) position.X, (int) position.Y - 16, 64, 68);
      if (this.type.Contains("Cow"))
        return this.FacingDirection == 0 || this.FacingDirection == 2 ? new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + 24.0 + 8.0), (int) position.Y, 68, 112) : new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + 4.0), (int) ((double) position.Y + 24.0 - 8.0), 112, 80);
      if (this.type.Contains("Pig"))
        return this.FacingDirection == 0 || this.FacingDirection == 2 ? new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + 24.0), (int) position.Y, 82, 112) : new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + 4.0), (int) ((double) position.Y + 24.0), 116, 72);
      if (this.type.Contains("Duck"))
        return new Microsoft.Xna.Framework.Rectangle((int) position.X, (int) ((double) position.Y - 8.0), 64, 60);
      if (this.type.Contains("Rabbit"))
        return new Microsoft.Xna.Framework.Rectangle((int) position.X, (int) ((double) position.Y - 8.0), 56, 56);
      if (this.type.Contains("Dinosaur"))
        return new Microsoft.Xna.Framework.Rectangle((int) position.X, (int) position.Y, 56, 52);
      if (this.type.Contains("Sheep"))
        return this.FacingDirection == 0 || this.FacingDirection == 2 ? new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + 24.0 + 8.0), (int) position.Y, 72, 112) : new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + 4.0), (int) ((double) position.Y + 24.0), 112, 72);
      if (!this.type.Contains("Goat"))
        return new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + (double) (this.Sprite.getWidth() * 4 / 2) - 32.0 + 4.0), (int) ((double) position.Y + (double) (this.Sprite.getHeight() * 4) - 64.0 - 24.0), 56, 72);
      return this.FacingDirection == 0 || this.FacingDirection == 2 ? new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + 40.0) - 8, (int) position.Y - 4, 64, 112) : new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + 4.0), (int) ((double) position.Y + 24.0) - 4, 112, 80);
    }

    public override Microsoft.Xna.Framework.Rectangle GetBoundingBox()
    {
      Vector2 position = this.Position;
      return new Microsoft.Xna.Framework.Rectangle((int) ((double) position.X + (double) (this.Sprite.getWidth() * 4 / 2) - 32.0 + 8.0), (int) ((double) position.Y + (double) (this.Sprite.getHeight() * 4) - 64.0 + 8.0), 48, 48);
    }

    public void reload(Building home)
    {
      this.home = home;
      this.reloadData();
    }

    public int GetDaysOwned()
    {
      if (this.daysOwned.Value < 0)
        this.daysOwned.Value = this.age.Value;
      return this.daysOwned.Value;
    }

    public void pet(Farmer who, bool is_auto_pet = false)
    {
      if (!is_auto_pet)
      {
        if (who.FarmerSprite.PauseForSingleAnimation)
          return;
        who.Halt();
        who.faceGeneralDirection(this.Position, 0, false, false);
        if (Game1.timeOfDay >= 1900 && !this.isMoving())
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\FarmAnimals:TryingToSleep", (object) this.displayName));
          return;
        }
        this.Halt();
        this.Sprite.StopAnimation();
        this.uniqueFrameAccumulator = -1;
        switch (Game1.player.FacingDirection)
        {
          case 0:
            this.Sprite.currentFrame = 0;
            break;
          case 1:
            this.Sprite.currentFrame = 12;
            break;
          case 2:
            this.Sprite.currentFrame = 8;
            break;
          case 3:
            this.Sprite.currentFrame = 4;
            break;
        }
      }
      else if (this.wasAutoPet.Value)
        return;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.wasPet)
      {
        if (!is_auto_pet)
          this.wasPet.Value = true;
        int num1 = 7;
        if (this.wasAutoPet.Value)
          this.friendshipTowardFarmer.Value = Math.Min(1000, (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + num1);
        else if (is_auto_pet)
          this.friendshipTowardFarmer.Value = Math.Min(1000, (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + (15 - num1));
        else
          this.friendshipTowardFarmer.Value = Math.Min(1000, (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + 15);
        if (is_auto_pet)
          this.wasAutoPet.Value = true;
        if (!is_auto_pet)
        {
          if (who.professions.Contains(3) && !this.isCoopDweller() || who.professions.Contains(2) && this.isCoopDweller())
          {
            this.friendshipTowardFarmer.Value = Math.Min(1000, (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + 15);
            this.happiness.Value = (byte) Math.Min((int) byte.MaxValue, (int) (byte) (NetFieldBase<byte, NetByte>) this.happiness + Math.Max(5, 40 - (int) (byte) (NetFieldBase<byte, NetByte>) this.happinessDrain));
          }
          int num2 = 20;
          if (this.wasAutoPet.Value)
            num2 = 32;
          this.doEmote((int) (NetFieldBase<int, NetInt>) this.moodMessage == 4 ? 12 : num2);
        }
        this.happiness.Value = (byte) Math.Min((int) byte.MaxValue, (int) (byte) (NetFieldBase<byte, NetByte>) this.happiness + Math.Max(5, 40 - (int) (byte) (NetFieldBase<byte, NetByte>) this.happinessDrain));
        if (!is_auto_pet)
        {
          this.makeSound();
          who.gainExperience(0, 5);
        }
      }
      else if (!is_auto_pet && (who.ActiveObject == null || (int) (NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex != 178))
        Game1.activeClickableMenu = (IClickableMenu) new AnimalQueryMenu(this);
      if (!this.type.Value.Equals("Sheep") || (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer < 900)
        return;
      this.daysToLay.Value = (byte) 2;
    }

    public void farmerPushing()
    {
      ++this.pushAccumulator;
      if (this.pushAccumulator <= 40)
        return;
      this.doFarmerPushEvent.Fire(Game1.player.FacingDirection);
      Game1.player.TemporaryPassableTiles.Add(Utility.ExpandRectangle(this.GetBoundingBox(), Utility.GetOppositeFacingDirection(Game1.player.FacingDirection), 6));
      this.pushAccumulator = 0;
    }

    public virtual void doDive()
    {
      this.yJumpVelocity = 8f;
      this.yJumpOffset = 1;
    }

    private void doFarmerPush(int direction)
    {
      if (!Game1.IsMasterGame)
        return;
      switch (direction)
      {
        case 0:
          this.Halt();
          this.SetMovingUp(true);
          break;
        case 1:
          this.Halt();
          this.SetMovingRight(true);
          break;
        case 2:
          this.Halt();
          this.SetMovingDown(true);
          break;
        case 3:
          this.Halt();
          this.SetMovingLeft(true);
          break;
      }
    }

    public void Poke() => this.doBuildingPokeEvent.Fire();

    private void doBuildingPoke()
    {
      if (!Game1.IsMasterGame)
        return;
      this.FacingDirection = Game1.random.Next(4);
      this.setMovingInFacingDirection();
    }

    public void setRandomPosition(GameLocation location)
    {
      this.StopAllActions();
      string[] strArray = location.getMapProperty("ProduceArea").Split(' ');
      int int32_1 = Convert.ToInt32(strArray[0]);
      int int32_2 = Convert.ToInt32(strArray[1]);
      int int32_3 = Convert.ToInt32(strArray[2]);
      int int32_4 = Convert.ToInt32(strArray[3]);
      this.Position = new Vector2((float) (Game1.random.Next(int32_1, int32_1 + int32_3) * 64), (float) (Game1.random.Next(int32_2, int32_2 + int32_4) * 64));
      int num = 0;
      while (this.Position.Equals(Vector2.Zero) || location.Objects.ContainsKey(this.Position) || location.isCollidingPosition(this.GetBoundingBox(), Game1.viewport, false, 0, false, (Character) this))
      {
        this.Position = new Vector2((float) Game1.random.Next(int32_1, int32_1 + int32_3), (float) Game1.random.Next(int32_2, int32_2 + int32_4)) * 64f;
        ++num;
        if (num > 64)
          break;
      }
      this.SleepIfNecessary();
    }

    public virtual void StopAllActions()
    {
      this.foundGrass = (Grass) null;
      this.controller = (PathFindController) null;
      this.isSwimming.Value = false;
      this.hopOffset = Vector2.Zero;
      this._followTarget = (FarmAnimal) null;
      this._followTargetPosition = new Point?();
      this.Halt();
      this.Sprite.StopAnimation();
      this.Sprite.UpdateSourceRect();
    }

    public void dayUpdate(GameLocation environtment)
    {
      if (this.daysOwned.Value < 0)
        this.daysOwned.Value = this.age.Value;
      this.StopAllActions();
      this.health.Value = 3;
      bool flag1 = false;
      if (this.home != null && !(this.home.indoors.Value as AnimalHouse).animals.ContainsKey((long) this.myID) && environtment is Farm)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.home.animalDoorOpen)
        {
          this.moodMessage.Value = 6;
          flag1 = true;
          this.happiness.Value /= (byte) 2;
        }
        else
        {
          (environtment as Farm).animals.Remove((long) this.myID);
          (this.home.indoors.Value as AnimalHouse).animals.Add((long) this.myID, this);
          if (Game1.timeOfDay > 1800 && this.controller == null)
            this.happiness.Value /= (byte) 2;
          environtment = (GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.home.indoors;
          this.setRandomPosition(environtment);
          return;
        }
      }
      ++this.daysSinceLastLay.Value;
      if (!this.wasPet.Value && !this.wasAutoPet.Value)
      {
        this.friendshipTowardFarmer.Value = Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer - (10 - (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer / 200));
        this.happiness.Value = (byte) Math.Max(0, (int) (byte) (NetFieldBase<byte, NetByte>) this.happiness - (int) (byte) (NetFieldBase<byte, NetByte>) this.happinessDrain * 5);
      }
      this.wasPet.Value = false;
      this.wasAutoPet.Value = false;
      ++this.daysOwned.Value;
      if ((byte) (NetFieldBase<byte, NetByte>) this.fullness < (byte) 200 && environtment is AnimalHouse)
      {
        for (int index = environtment.objects.Count() - 1; index >= 0; --index)
        {
          OverlaidDictionary.PairsCollection pairs = environtment.objects.Pairs;
          if (pairs.ElementAt(index).Value.Name.Equals("Hay"))
          {
            OverlaidDictionary objects = environtment.objects;
            pairs = environtment.objects.Pairs;
            Vector2 key = pairs.ElementAt(index).Key;
            objects.Remove(key);
            this.fullness.Value = byte.MaxValue;
            break;
          }
        }
      }
      Random random = new Random((int) (long) this.myID / 2 + (int) Game1.stats.DaysPlayed);
      if ((byte) (NetFieldBase<byte, NetByte>) this.fullness > (byte) 200 || random.NextDouble() < (double) ((int) (byte) (NetFieldBase<byte, NetByte>) this.fullness - 30) / 170.0)
      {
        ++this.age.Value;
        if ((int) (NetFieldBase<int, NetInt>) this.age == (int) (byte) (NetFieldBase<byte, NetByte>) this.ageWhenMature)
        {
          this.Sprite.LoadTexture("Animals\\" + this.type.Value);
          if (this.type.Value.Contains("Sheep"))
            this.currentProduce.Value = (int) (NetFieldBase<int, NetInt>) this.defaultProduceIndex;
          this.daysSinceLastLay.Value = (byte) 99;
        }
        this.happiness.Value = (byte) Math.Min((int) byte.MaxValue, (int) (byte) (NetFieldBase<byte, NetByte>) this.happiness + (int) (byte) (NetFieldBase<byte, NetByte>) this.happinessDrain * 2);
      }
      if (this.fullness.Value < (byte) 200)
      {
        this.happiness.Value = (byte) Math.Max(0, (int) (byte) (NetFieldBase<byte, NetByte>) this.happiness - 100);
        this.friendshipTowardFarmer.Value = Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer - 20);
      }
      bool flag2 = (int) (byte) (NetFieldBase<byte, NetByte>) this.daysSinceLastLay >= (int) (byte) (NetFieldBase<byte, NetByte>) this.daysToLay - (!this.type.Value.Equals("Sheep") || !Game1.getFarmer((long) this.ownerID).professions.Contains(3) ? 0 : 1) && random.NextDouble() < (double) (byte) (NetFieldBase<byte, NetByte>) this.fullness / 200.0 && random.NextDouble() < (double) (byte) (NetFieldBase<byte, NetByte>) this.happiness / 70.0;
      int parentSheetIndex;
      if (!flag2 || (int) (NetFieldBase<int, NetInt>) this.age < (int) (byte) (NetFieldBase<byte, NetByte>) this.ageWhenMature)
      {
        parentSheetIndex = -1;
      }
      else
      {
        parentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.defaultProduceIndex;
        if (random.NextDouble() < (double) (byte) (NetFieldBase<byte, NetByte>) this.happiness / 150.0)
        {
          float num1 = (byte) (NetFieldBase<byte, NetByte>) this.happiness > (byte) 200 ? (float) (byte) (NetFieldBase<byte, NetByte>) this.happiness * 1.5f : ((byte) (NetFieldBase<byte, NetByte>) this.happiness <= (byte) 100 ? (float) ((int) (byte) (NetFieldBase<byte, NetByte>) this.happiness - 100) : 0.0f);
          if (this.type.Value.Equals("Duck") && random.NextDouble() < ((double) (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + (double) num1) / 4750.0 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() * 0.01)
            parentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.deluxeProduceIndex;
          else if (this.type.Value.Equals("Rabbit") && random.NextDouble() < ((double) (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + (double) num1) / 5000.0 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() * 0.02)
            parentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.deluxeProduceIndex;
          this.daysSinceLastLay.Value = (byte) 0;
          switch (parentSheetIndex)
          {
            case 176:
              ++Game1.stats.ChickenEggsLayed;
              break;
            case 180:
              ++Game1.stats.ChickenEggsLayed;
              break;
            case 440:
              ++Game1.stats.RabbitWoolProduced;
              break;
            case 442:
              ++Game1.stats.DuckEggsLayed;
              break;
          }
          if (random.NextDouble() < ((double) (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + (double) num1) / 1200.0 && !this.type.Value.Equals("Duck") && !this.type.Value.Equals("Rabbit") && (int) (NetFieldBase<int, NetInt>) this.deluxeProduceIndex != -1 && (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer >= 200)
            parentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.deluxeProduceIndex;
          double num2 = (double) (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer / 1000.0 - (1.0 - (double) (byte) (NetFieldBase<byte, NetByte>) this.happiness / 225.0);
          if (!this.isCoopDweller() && Game1.getFarmer((long) this.ownerID).professions.Contains(3) || this.isCoopDweller() && Game1.getFarmer((long) this.ownerID).professions.Contains(2))
            num2 += 0.33;
          if (num2 >= 0.95 && random.NextDouble() < num2 / 2.0)
            this.produceQuality.Value = 4;
          else if (random.NextDouble() < num2 / 2.0)
            this.produceQuality.Value = 2;
          else if (random.NextDouble() < num2)
            this.produceQuality.Value = 1;
          else
            this.produceQuality.Value = 0;
        }
      }
      if ((byte) (NetFieldBase<byte, NetByte>) this.harvestType == (byte) 1 & flag2)
      {
        this.currentProduce.Value = parentSheetIndex;
        parentSheetIndex = -1;
      }
      if (parentSheetIndex != -1 && this.home != null)
      {
        bool flag3 = true;
        foreach (Object @object in this.home.indoors.Value.objects.Values)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) @object.bigCraftable && (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex == 165 && @object.heldObject.Value != null)
          {
            if ((@object.heldObject.Value as Chest).addItem((Item) new Object(Vector2.Zero, parentSheetIndex, (string) null, false, true, false, false)
            {
              Quality = (int) (NetFieldBase<int, NetInt>) this.produceQuality
            }) == null)
            {
              @object.showNextIndex.Value = true;
              flag3 = false;
              break;
            }
          }
        }
        if (flag3 && !this.home.indoors.Value.Objects.ContainsKey(this.getTileLocation()))
          this.home.indoors.Value.Objects.Add(this.getTileLocation(), new Object(Vector2.Zero, parentSheetIndex, (string) null, false, true, false, true)
          {
            Quality = (int) (NetFieldBase<int, NetInt>) this.produceQuality
          });
      }
      if (!flag1)
      {
        if ((byte) (NetFieldBase<byte, NetByte>) this.fullness < (byte) 30)
          this.moodMessage.Value = 4;
        else if ((byte) (NetFieldBase<byte, NetByte>) this.happiness < (byte) 30)
          this.moodMessage.Value = 3;
        else if ((byte) (NetFieldBase<byte, NetByte>) this.happiness < (byte) 200)
          this.moodMessage.Value = 2;
        else
          this.moodMessage.Value = 1;
      }
      if (Game1.timeOfDay < 1700)
        this.fullness.Value = (byte) Math.Max(0, (int) (byte) (NetFieldBase<byte, NetByte>) this.fullness - (int) (byte) (NetFieldBase<byte, NetByte>) this.fullnessDrain * (1700 - Game1.timeOfDay) / 100);
      this.fullness.Value = (byte) 0;
      if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
        this.fullness.Value = (byte) 250;
      this.reload(this.home);
    }

    public int getSellPrice() => (int) ((double) (int) (NetFieldBase<int, NetInt>) this.price * ((double) (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer / 1000.0 + 0.3));

    public bool isMale()
    {
      string str = this.type.Value;
      if (str == "Rabbit")
        return (long) this.myID % 2L == 0L;
      return (str == "Truffle Pig" || str == "Hog" || str == "Pig") && (long) this.myID % 2L == 0L;
    }

    public string getMoodMessage()
    {
      if ((byte) (NetFieldBase<byte, NetByte>) this.harvestType == (byte) 2)
        this.Name = "It";
      string str = this.isMale() ? "Male" : "Female";
      switch (this.moodMessage.Value)
      {
        case 0:
          return (long) this.parentId != -1L ? Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_NewHome_Baby_" + str, (object) this.displayName) : Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_NewHome_Adult_" + str + "_" + (Game1.dayOfMonth % 2 + 1).ToString(), (object) this.displayName);
        case 4:
          return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_" + (((long) Game1.dayOfMonth + (long) this.myID) % 2L == 0L ? "Hungry1" : "Hungry2"), (object) this.displayName);
        case 5:
          return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_DisturbedByDog_" + str, (object) this.displayName);
        case 6:
          return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_LeftOutsideAtNight_" + str, (object) this.displayName);
        default:
          if ((byte) (NetFieldBase<byte, NetByte>) this.happiness < (byte) 30)
            this.moodMessage.Value = 3;
          else if ((byte) (NetFieldBase<byte, NetByte>) this.happiness < (byte) 200)
            this.moodMessage.Value = 2;
          else
            this.moodMessage.Value = 1;
          switch ((int) (NetFieldBase<int, NetInt>) this.moodMessage)
          {
            case 1:
              return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_Happy", (object) this.displayName);
            case 2:
              return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_Fine", (object) this.displayName);
            case 3:
              return Game1.content.LoadString("Strings\\FarmAnimals:MoodMessage_Sad", (object) this.displayName);
            default:
              return "";
          }
      }
    }

    public bool isBaby() => (int) (NetFieldBase<int, NetInt>) this.age < (int) (byte) (NetFieldBase<byte, NetByte>) this.ageWhenMature;

    public void warpHome(Farm f, FarmAnimal a)
    {
      if (this.home == null)
        return;
      (this.home.indoors.Value as AnimalHouse).animals.Add((long) this.myID, this);
      f.animals.Remove((long) this.myID);
      this.controller = (PathFindController) null;
      this.setRandomPosition((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) this.home.indoors);
      ++this.home.currentOccupants.Value;
      this.isSwimming.Value = false;
      this.hopOffset = Vector2.Zero;
      this._followTarget = (FarmAnimal) null;
      this._followTargetPosition = new Point?();
    }

    public override void draw(SpriteBatch b)
    {
      if (this.isCoopDweller())
      {
        if (this.IsActuallySwimming())
          this.Sprite.drawShadow(b, Game1.GlobalToLocal(Game1.viewport, this.Position - new Vector2(0.0f, 24f)), this.isBaby() ? 2.5f : 3.5f, 0.5f);
        else
          this.Sprite.drawShadow(b, Game1.GlobalToLocal(Game1.viewport, this.Position - new Vector2(0.0f, 24f)), this.isBaby() ? 3f : 4f);
      }
      Vector2 vector2 = new Vector2();
      if (this.IsActuallySwimming())
      {
        int num = (int) ((Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 4.0 + (double) this.bobOffset) + 0.5) * 3.0);
        vector2.Y += (float) num;
      }
      vector2.Y += (float) this.yJumpOffset;
      float layerDepth = (float) (((double) (this.GetBoundingBox().Center.Y + 4) + (double) this.Position.X / 20000.0) / 10000.0);
      this.Sprite.draw(b, Utility.snapDrawPosition(Game1.GlobalToLocal(Game1.viewport, this.Position - new Vector2(0.0f, 24f) + vector2)), layerDepth, 0, 0, this.hitGlowTimer > 0 ? Color.Red : Color.White, this.FacingDirection == 3, 4f);
      if (!this.isEmoting)
        return;
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.Position + new Vector2((float) (this.frontBackSourceRect.Width / 2 * 4 - 32), this.isCoopDweller() ? -96f : -64f) + vector2);
      b.Draw(Game1.emoteSpriteSheet, local, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(this.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, this.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) this.GetBoundingBox().Bottom / 10000f);
    }

    public virtual void updateWhenNotCurrentLocation(
      Building currentBuilding,
      GameTime time,
      GameLocation environment)
    {
      this.doFarmerPushEvent.Poll();
      this.doBuildingPokeEvent.Poll();
      this.doDiveEvent.Poll();
      if (!Game1.shouldTimePass())
        return;
      this.update(time, environment, (long) this.myID, false);
      if (!Game1.IsMasterGame)
        return;
      if (this.hopOffset != Vector2.Zero)
      {
        this.HandleHop();
      }
      else
      {
        if (currentBuilding != null && Game1.random.NextDouble() < 0.002 && (bool) (NetFieldBase<bool, NetBool>) currentBuilding.animalDoorOpen && Game1.timeOfDay < 1630 && !Game1.isRaining && !Game1.currentSeason.Equals("winter") && !environment.farmers.Any())
        {
          Farm locationFromName = (Farm) Game1.getLocationFromName("Farm");
          if (locationFromName.isCollidingPosition(new Microsoft.Xna.Framework.Rectangle(((int) (NetFieldBase<int, NetInt>) currentBuilding.tileX + currentBuilding.animalDoor.X) * 64 + 2, ((int) (NetFieldBase<int, NetInt>) currentBuilding.tileY + currentBuilding.animalDoor.Y) * 64 + 2, (this.isCoopDweller() ? 64 : 128) - 4, 60), Game1.viewport, false, 0, false, (Character) this, false, false, false) || locationFromName.isCollidingPosition(new Microsoft.Xna.Framework.Rectangle(((int) (NetFieldBase<int, NetInt>) currentBuilding.tileX + currentBuilding.animalDoor.X) * 64 + 2, ((int) (NetFieldBase<int, NetInt>) currentBuilding.tileY + currentBuilding.animalDoor.Y + 1) * 64 + 2, (this.isCoopDweller() ? 64 : 128) - 4, 60), Game1.viewport, false, 0, false, (Character) this, false, false, false))
            return;
          if (locationFromName.animals.ContainsKey((long) this.myID))
          {
            for (int index = locationFromName.animals.Count() - 1; index >= 0; --index)
            {
              if (locationFromName.animals.Pairs.ElementAt(index).Key.Equals((long) this.myID))
              {
                locationFromName.animals.Remove((long) this.myID);
                break;
              }
            }
          }
          (currentBuilding.indoors.Value as AnimalHouse).animals.Remove((long) this.myID);
          locationFromName.animals.Add((long) this.myID, this);
          this.faceDirection(2);
          this.SetMovingDown(true);
          this.Position = new Vector2((float) currentBuilding.getRectForAnimalDoor().X, (float) (((int) (NetFieldBase<int, NetInt>) currentBuilding.tileY + currentBuilding.animalDoor.Y) * 64 - (this.Sprite.getHeight() * 4 - this.GetBoundingBox().Height) + 32));
          if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
          {
            ++FarmAnimal.NumPathfindingThisTick;
            this.controller = new PathFindController((Character) this, (GameLocation) locationFromName, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), Game1.random.Next(4), false, new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero);
          }
          if (this.controller == null || this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count < 3)
          {
            this.SetMovingDown(true);
            this.controller = (PathFindController) null;
          }
          else
          {
            this.faceDirection(2);
            this.Position = new Vector2((float) (this.controller.pathToEndPoint.Peek().X * 64), (float) (this.controller.pathToEndPoint.Peek().Y * 64 - (this.Sprite.getHeight() * 4 - this.GetBoundingBox().Height) + 16));
            if (!this.isCoopDweller())
              this.position.X -= 32f;
          }
          this.noWarpTimer = 3000;
          --currentBuilding.currentOccupants.Value;
          if (Utility.isOnScreen(this.getTileLocationPoint(), 192, (GameLocation) locationFromName))
            locationFromName.localSound("sandyStep");
          if (environment.isTileOccupiedByFarmer(this.getTileLocation()) != null)
            environment.isTileOccupiedByFarmer(this.getTileLocation()).TemporaryPassableTiles.Add(this.GetBoundingBox());
        }
        this.UpdateRandomMovements();
        this.behaviors(time, environment);
      }
    }

    public static void behaviorAfterFindingGrassPatch(Character c, GameLocation environment)
    {
      Vector2 key;
      ref Vector2 local = ref key;
      Microsoft.Xna.Framework.Rectangle boundingBox = c.GetBoundingBox();
      double x = (double) (boundingBox.Center.X / 64);
      boundingBox = c.GetBoundingBox();
      double y = (double) (boundingBox.Center.Y / 64);
      local = new Vector2((float) x, (float) y);
      if (environment.terrainFeatures.ContainsKey(key))
      {
        TerrainFeature terrainFeature = environment.terrainFeatures[key];
        if (terrainFeature is Grass && FarmAnimal.reservedGrass.Contains(terrainFeature as Grass))
          FarmAnimal.reservedGrass.Remove(terrainFeature as Grass);
      }
      if ((byte) (NetFieldBase<byte, NetByte>) ((FarmAnimal) c).fullness >= byte.MaxValue)
        return;
      ((FarmAnimal) c).eatGrass(environment);
    }

    public static bool animalDoorEndPointFunction(
      PathNode currentPoint,
      Point endPoint,
      GameLocation location,
      Character c)
    {
      Vector2 vector2 = new Vector2((float) currentPoint.x, (float) currentPoint.y);
      foreach (Building building in ((BuildableGameLocation) location).buildings)
      {
        if (building.animalDoor.X >= 0 && (double) (building.animalDoor.X + (int) (NetFieldBase<int, NetInt>) building.tileX) == (double) vector2.X && (double) (building.animalDoor.Y + (int) (NetFieldBase<int, NetInt>) building.tileY) == (double) vector2.Y && building.buildingType.Value.Contains((string) (NetFieldBase<string, NetString>) ((FarmAnimal) c).buildingTypeILiveIn) && (int) (NetFieldBase<int, NetInt>) building.currentOccupants < (int) (NetFieldBase<int, NetInt>) building.maxOccupants)
        {
          ++building.currentOccupants.Value;
          location.playSound("dwop");
          return true;
        }
      }
      return false;
    }

    public static bool grassEndPointFunction(
      PathNode currentPoint,
      Point endPoint,
      GameLocation location,
      Character c)
    {
      Vector2 key = new Vector2((float) currentPoint.x, (float) currentPoint.y);
      TerrainFeature terrainFeature;
      if (!location.terrainFeatures.TryGetValue(key, out terrainFeature) || !(terrainFeature is Grass) || ((IEnumerable<TerrainFeature>) FarmAnimal.reservedGrass).Contains<TerrainFeature>(terrainFeature))
        return false;
      FarmAnimal.reservedGrass.Add(terrainFeature as Grass);
      if (c is FarmAnimal)
        (c as FarmAnimal).foundGrass = terrainFeature as Grass;
      return true;
    }

    public virtual void updatePerTenMinutes(int timeOfDay, GameLocation environment)
    {
      if (timeOfDay >= 1800)
      {
        if (environment.IsOutdoors && timeOfDay > 1900 || !environment.IsOutdoors && (byte) (NetFieldBase<byte, NetByte>) this.happiness > (byte) 150 && Game1.currentSeason.Equals("winter") || (bool) (NetFieldBase<bool, NetBool>) environment.isOutdoors && Game1.isRaining || (bool) (NetFieldBase<bool, NetBool>) environment.isOutdoors && Game1.currentSeason.Equals("winter"))
          this.happiness.Value = (byte) Math.Min((int) byte.MaxValue, Math.Max(0, (int) (byte) (NetFieldBase<byte, NetByte>) this.happiness - (environment.numberOfObjectsWithName("Heater") <= 0 || !Game1.currentSeason.Equals("winter") ? (int) (byte) (NetFieldBase<byte, NetByte>) this.happinessDrain : (int) -(byte) (NetFieldBase<byte, NetByte>) this.happinessDrain)));
        else if (environment.IsOutdoors)
          this.happiness.Value = (byte) Math.Min((int) byte.MaxValue, (int) (byte) (NetFieldBase<byte, NetByte>) this.happiness + (int) (byte) (NetFieldBase<byte, NetByte>) this.happinessDrain);
      }
      if (environment.isTileOccupiedByFarmer(this.getTileLocation()) == null)
        return;
      environment.isTileOccupiedByFarmer(this.getTileLocation()).TemporaryPassableTiles.Add(this.GetBoundingBox());
    }

    public void eatGrass(GameLocation environment)
    {
      Vector2 key;
      ref Vector2 local = ref key;
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      double x = (double) (boundingBox.Center.X / 64);
      boundingBox = this.GetBoundingBox();
      double y = (double) (boundingBox.Center.Y / 64);
      local = new Vector2((float) x, (float) y);
      if (!environment.terrainFeatures.ContainsKey(key) || !(environment.terrainFeatures[key] is Grass))
        return;
      TerrainFeature terrainFeature = environment.terrainFeatures[key];
      if (FarmAnimal.reservedGrass.Contains(terrainFeature as Grass))
        FarmAnimal.reservedGrass.Remove(terrainFeature as Grass);
      if (this.foundGrass != null && FarmAnimal.reservedGrass.Contains(this.foundGrass))
        FarmAnimal.reservedGrass.Remove(this.foundGrass);
      this.foundGrass = (Grass) null;
      this.Eat(environment);
    }

    public virtual void Eat(GameLocation location)
    {
      Vector2 vector2;
      ref Vector2 local = ref vector2;
      Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
      double x = (double) (boundingBox.Center.X / 64);
      boundingBox = this.GetBoundingBox();
      double y = (double) (boundingBox.Center.Y / 64);
      local = new Vector2((float) x, (float) y);
      this.isEating.Value = true;
      if (location.terrainFeatures.ContainsKey(vector2) && location.terrainFeatures[vector2] is Grass && ((Grass) location.terrainFeatures[vector2]).reduceBy(this.isCoopDweller() ? 2 : 4, vector2, location.Equals(Game1.currentLocation)))
        location.terrainFeatures.Remove(vector2);
      this.Sprite.loop = false;
      this.fullness.Value = byte.MaxValue;
      if ((int) (NetFieldBase<int, NetInt>) this.moodMessage == 5 || (int) (NetFieldBase<int, NetInt>) this.moodMessage == 6 || Game1.isRaining)
        return;
      this.happiness.Value = byte.MaxValue;
      this.friendshipTowardFarmer.Value = Math.Min(1000, (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer + 8);
    }

    public override void performBehavior(byte which)
    {
      if (which != (byte) 0)
        return;
      this.eatGrass(Game1.currentLocation);
    }

    private bool behaviors(GameTime time, GameLocation location)
    {
      Building home = this.home;
      if (home == null)
        return false;
      if (Game1.IsMasterGame && this.isBaby() && this.CanFollowAdult())
      {
        this._nextFollowTargetScan -= (float) time.ElapsedGameTime.TotalSeconds;
        if ((double) this._nextFollowTargetScan < 0.0)
        {
          this._nextFollowTargetScan = Utility.RandomFloat(1f, 3f);
          if (this.controller != null || !(location is Farm))
          {
            this._followTarget = (FarmAnimal) null;
            this._followTargetPosition = new Point?();
          }
          else if (this._followTarget == null)
          {
            if (location is Farm)
            {
              foreach (FarmAnimal animal in (location as Farm).animals.Values)
              {
                if (!animal.isBaby() && animal.type.Value == this.type.Value && FarmAnimal.GetFollowRange(animal, 4).Contains(Utility.Vector2ToPoint(this.getStandingPosition())))
                {
                  this._followTarget = animal;
                  this.GetNewFollowPosition();
                  return false;
                }
              }
            }
          }
          else
          {
            if (!FarmAnimal.GetFollowRange(this._followTarget).Contains(this._followTargetPosition.Value))
              this.GetNewFollowPosition();
            return false;
          }
        }
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.isEating)
      {
        if (home != null && home.getRectForAnimalDoor().Intersects(this.GetBoundingBox()))
        {
          FarmAnimal.behaviorAfterFindingGrassPatch((Character) this, location);
          this.isEating.Value = false;
          this.Halt();
          return false;
        }
        if (this.buildingTypeILiveIn.Contains("Barn"))
        {
          this.Sprite.Animate(time, 16, 4, 100f);
          if (this.Sprite.currentFrame >= 20)
          {
            this.isEating.Value = false;
            this.Sprite.loop = true;
            this.Sprite.currentFrame = 0;
            this.faceDirection(2);
          }
        }
        else
        {
          this.Sprite.Animate(time, 24, 4, 100f);
          if (this.Sprite.currentFrame >= 28)
          {
            this.isEating.Value = false;
            this.Sprite.loop = true;
            this.Sprite.currentFrame = 0;
            this.faceDirection(2);
          }
        }
        return true;
      }
      if (!Game1.IsClient)
      {
        if (this.controller != null)
          return true;
        if (!this.isSwimming.Value && location.IsOutdoors && (byte) (NetFieldBase<byte, NetByte>) this.fullness < (byte) 195 && Game1.random.NextDouble() < 0.002 && FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
        {
          ++FarmAnimal.NumPathfindingThisTick;
          this.controller = new PathFindController((Character) this, location, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), -1, false, new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero);
          this._followTarget = (FarmAnimal) null;
          this._followTargetPosition = new Point?();
        }
        if (Game1.timeOfDay >= 1700 && location.IsOutdoors && this.controller == null && Game1.random.NextDouble() < 0.002)
        {
          if (!location.farmers.Any())
          {
            (location as Farm).animals.Remove((long) this.myID);
            (home.indoors.Value as AnimalHouse).animals.Add((long) this.myID, this);
            this.setRandomPosition((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) home.indoors);
            this.faceDirection(Game1.random.Next(4));
            this.controller = (PathFindController) null;
            return true;
          }
          if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
          {
            ++FarmAnimal.NumPathfindingThisTick;
            this.controller = new PathFindController((Character) this, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), 0, false, (PathFindController.endBehavior) null, 200, new Point((int) (NetFieldBase<int, NetInt>) home.tileX + home.animalDoor.X, (int) (NetFieldBase<int, NetInt>) home.tileY + home.animalDoor.Y));
            this._followTarget = (FarmAnimal) null;
            this._followTargetPosition = new Point?();
          }
        }
        if (location.IsOutdoors && !Game1.isRaining && !Game1.currentSeason.Equals("winter") && (int) (NetFieldBase<int, NetInt>) this.currentProduce != -1 && (int) (NetFieldBase<int, NetInt>) this.age >= (int) (byte) (NetFieldBase<byte, NetByte>) this.ageWhenMature && this.type.Value.Contains("Pig") && Game1.random.NextDouble() < 0.0002)
        {
          Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
          for (int corner = 0; corner < 4; ++corner)
          {
            Vector2 cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref boundingBox, corner);
            Vector2 key = new Vector2((float) (int) ((double) cornersOfThisRectangle.X / 64.0), (float) (int) ((double) cornersOfThisRectangle.Y / 64.0));
            if (location.terrainFeatures.ContainsKey(key) || location.objects.ContainsKey(key))
              return false;
          }
          if (Game1.player.currentLocation.Equals(location))
          {
            DelayedAction.playSoundAfterDelay("dirtyHit", 450);
            DelayedAction.playSoundAfterDelay("dirtyHit", 900);
            DelayedAction.playSoundAfterDelay("dirtyHit", 1350);
          }
          if (location.Equals(Game1.currentLocation))
          {
            switch (this.FacingDirection)
            {
              case 0:
                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250),
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250),
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(this.findTruffle))
                });
                break;
              case 1:
                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250),
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250),
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(this.findTruffle))
                });
                break;
              case 2:
                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250),
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250),
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(this.findTruffle))
                });
                break;
              case 3:
                this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(5, 250, false, true),
                  new FarmerSprite.AnimationFrame(7, 250, false, true),
                  new FarmerSprite.AnimationFrame(5, 250, false, true),
                  new FarmerSprite.AnimationFrame(7, 250, false, true),
                  new FarmerSprite.AnimationFrame(5, 250, false, true),
                  new FarmerSprite.AnimationFrame(7, 250, false, true, new AnimatedSprite.endOfAnimationBehavior(this.findTruffle))
                });
                break;
            }
            this.Sprite.loop = false;
          }
          else
            this.findTruffle(Game1.player);
        }
      }
      return false;
    }

    private void findTruffle(Farmer who)
    {
      if (Utility.spawnObjectAround(Utility.getTranslatedVector2(this.getTileLocation(), this.FacingDirection, 1f), new Object(this.getTileLocation(), 430, 1), (GameLocation) Game1.getFarm()))
        ++Game1.stats.TrufflesFound;
      if (new Random((int) (long) this.myID / 2 + (int) Game1.stats.DaysPlayed + Game1.timeOfDay).NextDouble() <= (double) (int) (NetFieldBase<int, NetInt>) this.friendshipTowardFarmer / 1500.0)
        return;
      this.currentProduce.Value = -1;
    }

    public static Microsoft.Xna.Framework.Rectangle GetFollowRange(
      FarmAnimal animal,
      int distance = 2)
    {
      Vector2 standingPosition = animal.getStandingPosition();
      return new Microsoft.Xna.Framework.Rectangle((int) ((double) standingPosition.X - (double) (distance * 64)), (int) ((double) standingPosition.Y - (double) (distance * 64)), distance * 64 * 2, 64 * distance * 2);
    }

    public virtual void GetNewFollowPosition()
    {
      if (this._followTarget == null)
        this._followTargetPosition = new Point?();
      else if (this._followTarget.isMoving() && this._followTarget.IsActuallySwimming())
        this._followTargetPosition = new Point?(Utility.Vector2ToPoint(Utility.getRandomPositionInThisRectangle(FarmAnimal.GetFollowRange(this._followTarget, 1), Game1.random)));
      else
        this._followTargetPosition = new Point?(Utility.Vector2ToPoint(Utility.getRandomPositionInThisRectangle(FarmAnimal.GetFollowRange(this._followTarget), Game1.random)));
    }

    public void hitWithWeapon(MeleeWeapon t)
    {
    }

    public void makeSound()
    {
      if (this.sound.Value == null || Game1.soundBank == null || this.currentLocation != Game1.currentLocation || Game1.options.muteAnimalSounds)
        return;
      ICue cue = Game1.soundBank.GetCue(this.sound.Value);
      cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
      cue.Play();
    }

    public virtual bool CanHavePregnancy() => !this.isCoopDweller() && !(this.type.Value == "Ostrich");

    public virtual bool SleepIfNecessary()
    {
      if (Game1.timeOfDay < 2000)
        return false;
      this.isSwimming.Value = false;
      this.hopOffset = Vector2.Zero;
      this._followTarget = (FarmAnimal) null;
      this._followTargetPosition = new Point?();
      if (this.isMoving())
        this.Halt();
      this.Sprite.currentFrame = this.buildingTypeILiveIn.Contains("Coop") ? 16 : 12;
      this.FacingDirection = 2;
      this.Sprite.UpdateSourceRect();
      return true;
    }

    public override bool isMoving()
    {
      if (this._swimmingVelocity != Vector2.Zero)
        return true;
      return (this.IsActuallySwimming() || this.uniqueFrameAccumulator == -1) && base.isMoving();
    }

    public virtual bool updateWhenCurrentLocation(GameTime time, GameLocation location)
    {
      if (!Game1.shouldTimePass())
        return false;
      if (this.health.Value <= 0)
        return true;
      this.doBuildingPokeEvent.Poll();
      this.doDiveEvent.Poll();
      if (this.IsActuallySwimming())
      {
        int num1 = 1;
        if (this.isMoving())
          num1 = 4;
        this.nextRipple -= (int) time.ElapsedGameTime.TotalMilliseconds * num1;
        if (this.nextRipple <= 0)
        {
          this.nextRipple = 2000;
          float scale = 1f;
          if (this.isBaby())
            scale = 0.65f;
          float num2 = this.Position.X - (float) this.getStandingX();
          TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), this.isMoving() ? 75f : 150f, 8, 0, new Vector2((float) this.getStandingX() + num2 * scale, (float) this.getStandingY() - 32f * scale), false, Game1.random.NextDouble() < 0.5, 0.01f, 0.01f, Color.White * 0.75f, scale, 0.0f, 0.0f, 0.0f);
          Vector2 vector2 = Utility.PointToVector2(Utility.getTranslatedPoint(new Point(), this.FacingDirection, -1));
          temporaryAnimatedSprite.motion = vector2 * 0.25f;
          location.TemporarySprites.Add(temporaryAnimatedSprite);
        }
      }
      if (this.hitGlowTimer > 0)
        this.hitGlowTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.Sprite.CurrentAnimation != null)
      {
        if (this.Sprite.animateOnce(time))
          this.Sprite.CurrentAnimation = (List<FarmerSprite.AnimationFrame>) null;
        return false;
      }
      this.update(time, location, (long) this.myID, false);
      if (this.hopOffset != Vector2.Zero)
      {
        this.Sprite.UpdateSourceRect();
        this.HandleHop();
        return false;
      }
      if (Game1.IsMasterGame && this.behaviors(time, location) || this.Sprite.CurrentAnimation != null)
        return false;
      if (this.controller != null && this.controller.timerSinceLastCheckPoint > 10000)
      {
        this.controller = (PathFindController) null;
        this.Halt();
      }
      if (location is Farm && this.noWarpTimer <= 0)
      {
        Building building = this.netHome.Value;
        if (building != null && Game1.IsMasterGame && building.getRectForAnimalDoor().Contains(this.GetBoundingBox().Center.X, this.GetBoundingBox().Top))
        {
          if (Utility.isOnScreen(this.getTileLocationPoint(), 192, location))
            location.localSound("dwoop");
          ((Farm) location).animals.Remove((long) this.myID);
          (building.indoors.Value as AnimalHouse).animals[(long) this.myID] = this;
          this.setRandomPosition((GameLocation) (NetFieldBase<GameLocation, NetRef<GameLocation>>) building.indoors);
          this.faceDirection(Game1.random.Next(4));
          this.controller = (PathFindController) null;
          return true;
        }
      }
      this.noWarpTimer = Math.Max(0, this.noWarpTimer - time.ElapsedGameTime.Milliseconds);
      if (this.pauseTimer > 0)
        this.pauseTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.SleepIfNecessary())
      {
        if (!this.isEmoting && Game1.random.NextDouble() < 0.002)
          this.doEmote(24);
      }
      else if (this.pauseTimer <= 0 && Game1.random.NextDouble() < 0.001 && (int) (NetFieldBase<int, NetInt>) this.age >= (int) (byte) (NetFieldBase<byte, NetByte>) this.ageWhenMature && Game1.gameMode == (byte) 3 && this.sound.Value != null && Utility.isOnScreen(this.Position, 192))
        this.makeSound();
      this.UpdateRandomMovements();
      if (this.uniqueFrameAccumulator != -1 && this._followTarget != null && !FarmAnimal.GetFollowRange(this._followTarget, 1).Contains(Utility.Vector2ToPoint(this.getStandingPosition())))
        this.uniqueFrameAccumulator = -1;
      if (this.uniqueFrameAccumulator != -1 && !Game1.IsClient)
      {
        this.uniqueFrameAccumulator += time.ElapsedGameTime.Milliseconds;
        if (this.uniqueFrameAccumulator > 500)
        {
          if (this.buildingTypeILiveIn.Contains("Coop"))
            this.Sprite.currentFrame = this.Sprite.currentFrame + 1 - this.Sprite.currentFrame % 2 * 2;
          else if (this.Sprite.currentFrame > 12)
          {
            this.Sprite.currentFrame = (this.Sprite.currentFrame - 13) * 4;
          }
          else
          {
            switch (this.FacingDirection)
            {
              case 0:
                this.Sprite.currentFrame = 15;
                break;
              case 1:
                this.Sprite.currentFrame = 14;
                break;
              case 2:
                this.Sprite.currentFrame = 13;
                break;
              case 3:
                this.Sprite.currentFrame = 14;
                break;
            }
          }
          this.uniqueFrameAccumulator = 0;
          if (Game1.random.NextDouble() < 0.4)
            this.uniqueFrameAccumulator = -1;
        }
        if (this.IsActuallySwimming())
          this.MovePosition(time, Game1.viewport, location);
      }
      else if (!Game1.IsClient)
        this.MovePosition(time, Game1.viewport, location);
      if (this.IsActuallySwimming())
      {
        this.Sprite.UpdateSourceRect();
        Microsoft.Xna.Framework.Rectangle sourceRect = this.Sprite.SourceRect;
        sourceRect.Offset(new Point(0, 112));
        this.Sprite.SourceRect = sourceRect;
      }
      return false;
    }

    public virtual void UpdateRandomMovements()
    {
      if (Game1.timeOfDay >= 2000 || this.pauseTimer > 0)
        return;
      if (this.fullness.Value < byte.MaxValue && this.IsActuallySwimming() && Game1.random.NextDouble() < 0.002 && !this.isEating.Value)
        this.Eat(this.currentLocation);
      if (!Game1.IsClient && Game1.random.NextDouble() < 0.007 && this.uniqueFrameAccumulator == -1)
      {
        int direction = Game1.random.Next(5);
        if (direction != (this.FacingDirection + 2) % 4 || this.IsActuallySwimming())
        {
          if (direction < 4)
          {
            int facingDirection = this.FacingDirection;
            this.faceDirection(direction);
            if (!(bool) (NetFieldBase<bool, NetBool>) this.currentLocation.isOutdoors && this.currentLocation.isCollidingPosition(this.nextPosition(direction), Game1.viewport, (Character) this))
            {
              this.faceDirection(facingDirection);
              return;
            }
          }
          switch (direction)
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
            default:
              this.Halt();
              this.Sprite.StopAnimation();
              break;
          }
        }
        else if (this.noWarpTimer <= 0)
        {
          this.Halt();
          this.Sprite.StopAnimation();
        }
      }
      if (Game1.IsClient || !this.isMoving() || Game1.random.NextDouble() >= 0.014 || this.uniqueFrameAccumulator != -1)
        return;
      this.Halt();
      this.Sprite.StopAnimation();
      if (Game1.random.NextDouble() < 0.75)
      {
        this.uniqueFrameAccumulator = 0;
        if (this.buildingTypeILiveIn.Contains("Coop"))
        {
          switch (this.FacingDirection)
          {
            case 0:
              this.Sprite.currentFrame = 20;
              break;
            case 1:
              this.Sprite.currentFrame = 18;
              break;
            case 2:
              this.Sprite.currentFrame = 16;
              break;
            case 3:
              this.Sprite.currentFrame = 22;
              break;
          }
        }
        else if (this.buildingTypeILiveIn.Contains("Barn"))
        {
          switch (this.FacingDirection)
          {
            case 0:
              this.Sprite.currentFrame = 15;
              break;
            case 1:
              this.Sprite.currentFrame = 14;
              break;
            case 2:
              this.Sprite.currentFrame = 13;
              break;
            case 3:
              this.Sprite.currentFrame = 14;
              break;
          }
        }
      }
      this.Sprite.UpdateSourceRect();
    }

    public virtual bool CanSwim() => this.type.Value == "Duck";

    public virtual bool CanFollowAdult() => this.isCoopDweller() && this.isBaby() && (this.type.Value == "Duck" || this.type.Contains("Chicken"));

    public override bool shouldCollideWithBuildingLayer(GameLocation location) => true;

    public virtual void HandleHop()
    {
      int val1 = 4;
      if (!(this.hopOffset != Vector2.Zero))
        return;
      if ((double) this.hopOffset.X != 0.0)
      {
        int delta = (int) Math.Min((float) val1, Math.Abs(this.hopOffset.X));
        this.Position = this.Position + new Vector2((float) (delta * Math.Sign(this.hopOffset.X)), 0.0f);
        this.hopOffset.X = Utility.MoveTowards(this.hopOffset.X, 0.0f, (float) delta);
      }
      if ((double) this.hopOffset.Y != 0.0)
      {
        int delta = (int) Math.Min((float) val1, Math.Abs(this.hopOffset.Y));
        this.Position = this.Position + new Vector2(0.0f, (float) (delta * Math.Sign(this.hopOffset.Y)));
        this.hopOffset.Y = Utility.MoveTowards(this.hopOffset.Y, 0.0f, (float) delta);
      }
      if (!(this.hopOffset == Vector2.Zero) || !this.isSwimming.Value)
        return;
      this.Splash();
      this._swimmingVelocity = Utility.getTranslatedVector2(Vector2.Zero, this.FacingDirection, (float) this.speed);
      this.Position = new Vector2((float) (int) Math.Round((double) this.Position.X), (float) (int) Math.Round((double) this.Position.Y));
    }

    public override void MovePosition(
      GameTime time,
      xTile.Dimensions.Rectangle viewport,
      GameLocation currentLocation)
    {
      if (this.pauseTimer > 0 || Game1.IsClient)
        return;
      Location location = this.nextPositionTile();
      if (!currentLocation.isTileOnMap(new Vector2((float) location.X, (float) location.Y)))
      {
        this.facingDirection.Value = Utility.GetOppositeFacingDirection((int) this.facingDirection);
        this.moveUp = this.facingDirection.Value == 0;
        this.moveLeft = this.facingDirection.Value == 3;
        this.moveDown = this.facingDirection.Value == 2;
        this.moveRight = this.facingDirection.Value == 1;
        this._followTarget = (FarmAnimal) null;
        this._followTargetPosition = new Point?();
        this._swimmingVelocity = Vector2.Zero;
      }
      else
      {
        if (this._followTarget != null && (this._followTarget.currentLocation != currentLocation || (int) (NetFieldBase<int, NetInt>) this._followTarget.health <= 0))
        {
          this._followTarget = (FarmAnimal) null;
          this._followTargetPosition = new Point?();
        }
        if (this._followTargetPosition.HasValue)
        {
          Vector2 standingPosition = this.getStandingPosition();
          Vector2 vector2 = standingPosition - Utility.PointToVector2(this._followTargetPosition.Value);
          if ((double) Math.Abs(vector2.X) <= 64.0 || (double) Math.Abs(vector2.Y) <= 64.0)
          {
            this.moveDown = false;
            this.moveUp = false;
            this.moveLeft = false;
            this.moveRight = false;
            this.GetNewFollowPosition();
          }
          else if (this.nextFollowDirectionChange >= 0)
          {
            this.nextFollowDirectionChange -= (int) time.ElapsedGameTime.TotalMilliseconds;
          }
          else
          {
            this.nextFollowDirectionChange = !this.IsActuallySwimming() ? 500 : 100;
            this.moveDown = false;
            this.moveUp = false;
            this.moveLeft = false;
            this.moveRight = false;
            if ((double) Math.Abs(standingPosition.X - (float) this._followTargetPosition.Value.X) < (double) Math.Abs(standingPosition.Y - (float) this._followTargetPosition.Value.Y))
            {
              if ((double) standingPosition.Y > (double) this._followTargetPosition.Value.Y)
                this.moveUp = true;
              else if ((double) standingPosition.Y < (double) this._followTargetPosition.Value.Y)
                this.moveDown = true;
            }
            else if ((double) standingPosition.X < (double) this._followTargetPosition.Value.X)
              this.moveRight = true;
            else if ((double) standingPosition.X > (double) this._followTargetPosition.Value.X)
              this.moveLeft = true;
          }
        }
        if (this.IsActuallySwimming())
        {
          Vector2 vector2 = new Vector2();
          if (!this.isEating.Value)
          {
            if (this.moveUp)
              vector2.Y = (float) -this.speed;
            else if (this.moveDown)
              vector2.Y = (float) this.speed;
            if (this.moveLeft)
              vector2.X = (float) -this.speed;
            else if (this.moveRight)
              vector2.X = (float) this.speed;
          }
          this._swimmingVelocity = new Vector2(Utility.MoveTowards(this._swimmingVelocity.X, vector2.X, 0.025f), Utility.MoveTowards(this._swimmingVelocity.Y, vector2.Y, 0.025f));
          Vector2 position = this.Position;
          this.Position = this.Position + this._swimmingVelocity;
          Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
          this.Position = position;
          int num = -1;
          if (!currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, (Character) this, false))
          {
            this.Position = this.Position + this._swimmingVelocity;
            if ((double) Math.Abs(this._swimmingVelocity.X) > (double) Math.Abs(this._swimmingVelocity.Y))
            {
              if ((double) this._swimmingVelocity.X < 0.0)
                num = 3;
              else if ((double) this._swimmingVelocity.X > 0.0)
                num = 1;
            }
            else if ((double) this._swimmingVelocity.Y < 0.0)
              num = 0;
            else if ((double) this._swimmingVelocity.Y > 0.0)
              num = 2;
            switch (num)
            {
              case 0:
                this.Sprite.AnimateUp(time);
                this.faceDirection(0);
                break;
              case 1:
                this.Sprite.AnimateRight(time);
                this.faceDirection(1);
                break;
              case 2:
                this.Sprite.AnimateDown(time);
                this.faceDirection(2);
                break;
              case 3:
                this.Sprite.AnimateRight(time);
                this.FacingDirection = 3;
                break;
            }
          }
          else
          {
            if (this.HandleCollision(boundingBox))
              return;
            this.Halt();
            this.Sprite.StopAnimation();
            this._swimmingVelocity *= -1f;
          }
        }
        else if (this.moveUp)
        {
          if (!currentLocation.isCollidingPosition(this.nextPosition(0), Game1.viewport, false, 0, false, (Character) this, false))
          {
            this.position.Y -= (float) this.speed;
            this.Sprite.AnimateUp(time);
          }
          else if (!this.HandleCollision(this.nextPosition(0)))
          {
            this.Halt();
            this.Sprite.StopAnimation();
            if (Game1.random.NextDouble() < 0.6 || this.IsActuallySwimming())
              this.SetMovingDown(true);
          }
          this.faceDirection(0);
        }
        else if (this.moveRight)
        {
          if (!currentLocation.isCollidingPosition(this.nextPosition(1), Game1.viewport, false, 0, false, (Character) this))
          {
            this.position.X += (float) this.speed;
            this.Sprite.AnimateRight(time);
          }
          else if (!this.HandleCollision(this.nextPosition(1)))
          {
            this.Halt();
            this.Sprite.StopAnimation();
            if (Game1.random.NextDouble() < 0.6 || this.IsActuallySwimming())
              this.SetMovingLeft(true);
          }
          this.faceDirection(1);
        }
        else if (this.moveDown)
        {
          if (!currentLocation.isCollidingPosition(this.nextPosition(2), Game1.viewport, false, 0, false, (Character) this))
          {
            this.position.Y += (float) this.speed;
            this.Sprite.AnimateDown(time);
          }
          else if (!this.HandleCollision(this.nextPosition(2)))
          {
            this.Halt();
            this.Sprite.StopAnimation();
            if (Game1.random.NextDouble() < 0.6 || this.IsActuallySwimming())
              this.SetMovingUp(true);
          }
          this.faceDirection(2);
        }
        else
        {
          if (!this.moveLeft)
            return;
          if (!currentLocation.isCollidingPosition(this.nextPosition(3), Game1.viewport, false, 0, false, (Character) this))
          {
            this.position.X -= (float) this.speed;
            this.Sprite.AnimateRight(time);
          }
          else if (!this.HandleCollision(this.nextPosition(3)))
          {
            this.Halt();
            this.Sprite.StopAnimation();
            if (Game1.random.NextDouble() < 0.6 || this.IsActuallySwimming())
              this.SetMovingRight(true);
          }
          this.FacingDirection = 3;
          if (this.isCoopDweller() || this.Sprite.currentFrame <= 7)
            return;
          this.Sprite.currentFrame = 4;
        }
      }
    }

    public virtual bool HandleCollision(Microsoft.Xna.Framework.Rectangle next_position)
    {
      if (this._followTarget != null)
      {
        this._followTarget = (FarmAnimal) null;
        this._followTargetPosition = new Point?();
      }
      if (this.currentLocation is Farm && this.CanSwim() && (this.isSwimming.Value || this.controller == null) && this.wasPet.Value && this.hopOffset == Vector2.Zero)
      {
        this.Position = new Vector2((float) (int) Math.Round((double) this.Position.X), (float) (int) Math.Round((double) this.Position.Y));
        Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
        Vector2 translatedVector2 = Utility.getTranslatedVector2(Vector2.Zero, this.FacingDirection, 1f);
        if (translatedVector2 != Vector2.Zero)
        {
          Point tileLocationPoint = this.getTileLocationPoint();
          tileLocationPoint.X += (int) translatedVector2.X;
          tileLocationPoint.Y += (int) translatedVector2.Y;
          Vector2 v = translatedVector2 * 128f;
          Microsoft.Xna.Framework.Rectangle position = boundingBox;
          position.Offset(Utility.Vector2ToPoint(v));
          Point point = new Point(position.X / 64, position.Y / 64);
          if (this.currentLocation.doesTileHaveProperty(tileLocationPoint.X, tileLocationPoint.Y, "Water", "Back") != null && this.currentLocation.doesTileHaveProperty(tileLocationPoint.X, tileLocationPoint.Y, "Passable", "Buildings") == null && !this.currentLocation.isCollidingPosition(position, Game1.viewport, false, 0, false, (Character) this) && this.currentLocation.isOpenWater(point.X, point.Y) != this.isSwimming.Value)
          {
            this.isSwimming.Value = !this.isSwimming.Value;
            if (!this.isSwimming.Value)
              this.Splash();
            this.hopOffset = v;
            this.pauseTimer = 0;
            this.doDiveEvent.Fire();
          }
          return true;
        }
      }
      return false;
    }

    public virtual bool IsActuallySwimming() => this.isSwimming.Value && this.hopOffset == Vector2.Zero;

    public virtual void Splash()
    {
      if (Utility.isOnScreen(this.getTileLocationPoint(), 192, this.currentLocation))
        this.currentLocation.playSound("dropItemInWater");
      Game1.multiplayer.broadcastSprites(this.currentLocation, new TemporaryAnimatedSprite(28, 100f, 2, 1, this.getStandingPosition() + new Vector2(-0.5f, -0.5f) * 64f, false, false)
      {
        delayBeforeAnimationStart = 0,
        layerDepth = (float) this.getStandingY() / 10000f
      });
    }

    public override void animateInFacingDirection(GameTime time)
    {
      if (this.FacingDirection == 3)
        this.Sprite.AnimateRight(time);
      else
        base.animateInFacingDirection(time);
    }
  }
}
