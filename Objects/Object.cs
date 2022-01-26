// Decompiled with JetBrains decompiler
// Type: StardewValley.Object
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
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
  [XmlInclude(typeof (Fence))]
  [XmlInclude(typeof (Torch))]
  [XmlInclude(typeof (SpecialItem))]
  [XmlInclude(typeof (Wallpaper))]
  [XmlInclude(typeof (Boots))]
  [XmlInclude(typeof (Hat))]
  [XmlInclude(typeof (ItemPedestal))]
  [XmlInclude(typeof (Clothing))]
  [XmlInclude(typeof (CombinedRing))]
  [XmlInclude(typeof (Ring))]
  [XmlInclude(typeof (TV))]
  [XmlInclude(typeof (CrabPot))]
  [XmlInclude(typeof (Chest))]
  [XmlInclude(typeof (Workbench))]
  [XmlInclude(typeof (MiniJukebox))]
  [XmlInclude(typeof (Phone))]
  [XmlInclude(typeof (StorageFurniture))]
  [XmlInclude(typeof (FishTankFurniture))]
  [XmlInclude(typeof (BedFurniture))]
  [XmlInclude(typeof (WoodChipper))]
  [XmlInclude(typeof (Cask))]
  [XmlInclude(typeof (SwitchFloor))]
  [XmlInclude(typeof (ColoredObject))]
  [XmlInclude(typeof (IndoorPot))]
  [XmlInclude(typeof (Sign))]
  public class Object : Item
  {
    public const int copperBar = 334;
    public const int ironBar = 335;
    public const int goldBar = 336;
    public const int iridiumBar = 337;
    public const int wood = 388;
    public const int stone = 390;
    public const int copper = 378;
    public const int iron = 380;
    public const int coal = 382;
    public const int gold = 384;
    public const int iridium = 386;
    public const int inedible = -300;
    public const int GreensCategory = -81;
    public const int GemCategory = -2;
    public const int VegetableCategory = -75;
    public const int FishCategory = -4;
    public const int EggCategory = -5;
    public const int MilkCategory = -6;
    public const int CookingCategory = -7;
    public const int CraftingCategory = -8;
    public const int BigCraftableCategory = -9;
    public const int FruitsCategory = -79;
    public const int SeedsCategory = -74;
    public const int mineralsCategory = -12;
    public const int flowersCategory = -80;
    public const int meatCategory = -14;
    public const int metalResources = -15;
    public const int buildingResources = -16;
    public const int sellAtPierres = -17;
    public const int sellAtPierresAndMarnies = -18;
    public const int fertilizerCategory = -19;
    public const int junkCategory = -20;
    public const int baitCategory = -21;
    public const int tackleCategory = -22;
    public const int sellAtFishShopCategory = -23;
    public const int furnitureCategory = -24;
    public const int ingredientsCategory = -25;
    public const int artisanGoodsCategory = -26;
    public const int syrupCategory = -27;
    public const int monsterLootCategory = -28;
    public const int equipmentCategory = -29;
    public const int clothingCategorySortValue = -94;
    public const int hatCategory = -95;
    public const int ringCategory = -96;
    public const int weaponCategory = -98;
    public const int bootsCategory = -97;
    public const int toolCategory = -99;
    public const int clothingCategory = -100;
    public const int objectInfoNameIndex = 0;
    public const int objectInfoPriceIndex = 1;
    public const int objectInfoEdibilityIndex = 2;
    public const int objectInfoTypeIndex = 3;
    public const int objectInfoDisplayNameIndex = 4;
    public const int objectInfoDescriptionIndex = 5;
    public const int objectInfoMiscIndex = 6;
    public const int objectInfoBuffTypesIndex = 7;
    public const int objectInfoBuffDurationIndex = 8;
    public const int WeedsIndex = 0;
    public const int StoneIndex = 2;
    public const int StickIndex = 4;
    public const int DryDirtTileIndex = 6;
    public const int WateredTileIndex = 7;
    public const int StumpTopLeftIndex = 8;
    public const int BoulderTopLeftIndex = 10;
    public const int StumpBottomLeftIndex = 12;
    public const int BoulderBottomLeftIndex = 14;
    public const int WildHorseradishIndex = 16;
    public const int TulipIndex = 18;
    public const int LeekIndex = 20;
    public const int DandelionIndex = 22;
    public const int ParsnipIndex = 24;
    public const int HandCursorIndex = 26;
    public const int WaterAnimationIndex = 28;
    public const int LumberIndex = 30;
    public const int mineStoneGrey1Index = 32;
    public const int mineStoneBlue1Index = 34;
    public const int mineStoneBlue2Index = 36;
    public const int mineStoneGrey2Index = 38;
    public const int mineStoneBrown1Index = 40;
    public const int mineStoneBrown2Index = 42;
    public const int mineStonePurpleIndex = 44;
    public const int mineStoneMysticIndex = 46;
    public const int mineStoneSnow1 = 48;
    public const int mineStoneSnow2 = 50;
    public const int mineStoneSnow3 = 52;
    public const int mineStonePurpleSnowIndex = 54;
    public const int mineStoneRed1Index = 56;
    public const int mineStoneRed2Index = 58;
    public const int emeraldIndex = 60;
    public const int aquamarineIndex = 62;
    public const int rubyIndex = 64;
    public const int amethystClusterIndex = 66;
    public const int topazIndex = 68;
    public const int sapphireIndex = 70;
    public const int diamondIndex = 72;
    public const int prismaticShardIndex = 74;
    public const int snowHoedDirtIndex = 76;
    public const int beachHoedDirtIndex = 77;
    public const int caveCarrotIndex = 78;
    public const int quartzIndex = 80;
    public const int bobberIndex = 133;
    public const int stardrop = 434;
    public const int spriteSheetTileSize = 16;
    public const int lowQuality = 0;
    public const int medQuality = 1;
    public const int highQuality = 2;
    public const int bestQuality = 4;
    public const int copperPerBar = 10;
    public const int ironPerBar = 10;
    public const int goldPerBar = 10;
    public const int iridiumPerBar = 10;
    public const float wobbleAmountWhenWorking = 10f;
    public const int fragility_Removable = 0;
    public const int fragility_Delicate = 1;
    public const int fragility_Indestructable = 2;
    [XmlElement("tileLocation")]
    public readonly NetVector2 tileLocation = new NetVector2();
    [XmlElement("owner")]
    public readonly NetLong owner = new NetLong();
    [XmlElement("type")]
    public readonly NetString type = new NetString();
    [XmlElement("canBeSetDown")]
    public readonly NetBool canBeSetDown = new NetBool(false);
    [XmlElement("canBeGrabbed")]
    public readonly NetBool canBeGrabbed = new NetBool(true);
    [XmlElement("isHoedirt")]
    public readonly NetBool isHoedirt = new NetBool(false);
    [XmlElement("isSpawnedObject")]
    public readonly NetBool isSpawnedObject = new NetBool(false);
    [XmlElement("questItem")]
    public readonly NetBool questItem = new NetBool(false);
    [XmlElement("questId")]
    public readonly NetInt questId = new NetInt(0);
    [XmlElement("isOn")]
    public readonly NetBool isOn = new NetBool(true);
    [XmlElement("fragility")]
    public readonly NetInt fragility = new NetInt(0);
    private bool isActive;
    [XmlElement("price")]
    public readonly NetInt price = new NetInt();
    [XmlElement("edibility")]
    public readonly NetInt edibility = new NetInt(-300);
    [XmlElement("stack")]
    public readonly NetInt stack = new NetInt(1);
    [XmlElement("quality")]
    public readonly NetInt quality = new NetInt(0);
    [XmlElement("bigCraftable")]
    public readonly NetBool bigCraftable = new NetBool();
    [XmlElement("setOutdoors")]
    public readonly NetBool setOutdoors = new NetBool();
    [XmlElement("setIndoors")]
    public readonly NetBool setIndoors = new NetBool();
    [XmlElement("readyForHarvest")]
    public readonly NetBool readyForHarvest = new NetBool();
    [XmlElement("showNextIndex")]
    public readonly NetBool showNextIndex = new NetBool();
    [XmlElement("flipped")]
    public readonly NetBool flipped = new NetBool();
    [XmlElement("hasBeenPickedUpByFarmer")]
    public readonly NetBool hasBeenPickedUpByFarmer = new NetBool();
    [XmlElement("isRecipe")]
    public readonly NetBool isRecipe = new NetBool();
    [XmlElement("isLamp")]
    public readonly NetBool isLamp = new NetBool();
    [XmlElement("heldObject")]
    public readonly NetRef<Object> heldObject = new NetRef<Object>();
    [XmlElement("minutesUntilReady")]
    public readonly NetIntDelta minutesUntilReady = new NetIntDelta();
    [XmlElement("boundingBox")]
    public readonly NetRectangle boundingBox = new NetRectangle();
    public Vector2 scale;
    [XmlElement("uses")]
    public readonly NetInt uses = new NetInt();
    [XmlIgnore]
    private readonly NetRef<LightSource> netLightSource = new NetRef<LightSource>();
    [XmlIgnore]
    public bool isTemporarilyInvisible;
    [XmlIgnore]
    protected NetBool _destroyOvernight = new NetBool(false);
    [XmlElement("orderData")]
    public readonly NetString orderData = new NetString();
    [XmlIgnore]
    public static Chest autoLoadChest;
    [XmlIgnore]
    public int shakeTimer;
    [XmlIgnore]
    public int lastNoteBlockSoundTime;
    [XmlIgnore]
    public ICue internalSound;
    [XmlElement("preserve")]
    public readonly NetNullableEnum<Object.PreserveType> preserve = new NetNullableEnum<Object.PreserveType>();
    [XmlElement("preservedParentSheetIndex")]
    public readonly NetInt preservedParentSheetIndex = new NetInt();
    [XmlElement("honeyType")]
    public readonly NetNullableEnum<Object.HoneyType> honeyType = new NetNullableEnum<Object.HoneyType>();
    [XmlIgnore]
    public string displayName;
    protected int health = 10;

    public bool destroyOvernight
    {
      get => this._destroyOvernight.Value;
      set => this._destroyOvernight.Value = value;
    }

    [XmlIgnore]
    public LightSource lightSource
    {
      get => (LightSource) (NetFieldBase<LightSource, NetRef<LightSource>>) this.netLightSource;
      set => this.netLightSource.Value = value;
    }

    [XmlIgnore]
    public Vector2 TileLocation
    {
      get => (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation;
      set => this.tileLocation.Value = value;
    }

    [XmlIgnore]
    public string name
    {
      get => this.netName.Value;
      set => this.netName.Value = value;
    }

    [XmlIgnore]
    public override string DisplayName
    {
      get
      {
        if (Game1.objectInformation != null)
        {
          this.displayName = this.loadDisplayName();
          if (this.orderData.Value != null && this.orderData.Value == "QI_COOKING")
            this.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Fresh_Prefix", (object) this.displayName);
        }
        return this.displayName + ((bool) (NetFieldBase<bool, NetBool>) this.isRecipe ? (!CraftingRecipe.craftingRecipes.ContainsKey(this.displayName) || ((IEnumerable<string>) CraftingRecipe.craftingRecipes[this.displayName].Split('/')[2].Split(' ')).Count<string>() <= 1 ? "" : " x" + CraftingRecipe.craftingRecipes[this.displayName].Split('/')[2].Split(' ')[1]) + Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12657") : "");
      }
      set => this.displayName = value;
    }

    [XmlIgnore]
    public override string Name
    {
      get => this.name + ((bool) (NetFieldBase<bool, NetBool>) this.isRecipe ? " Recipe" : "");
      set => this.name = value;
    }

    [XmlIgnore]
    public string Type
    {
      get => (string) (NetFieldBase<string, NetString>) this.type;
      set => this.type.Value = value;
    }

    [XmlIgnore]
    public override int Stack
    {
      get => Math.Max(0, (int) (NetFieldBase<int, NetInt>) this.stack);
      set => this.stack.Value = Math.Min(Math.Max(0, value), value == int.MaxValue ? value : this.maximumStackSize());
    }

    [XmlIgnore]
    public int Quality
    {
      get => (int) (NetFieldBase<int, NetInt>) this.quality;
      set => this.quality.Value = value;
    }

    [XmlIgnore]
    public bool CanBeSetDown
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.canBeSetDown;
      set => this.canBeSetDown.Value = value;
    }

    [XmlIgnore]
    public bool CanBeGrabbed
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.canBeGrabbed;
      set => this.canBeGrabbed.Value = value;
    }

    [XmlIgnore]
    public bool HasBeenPickedUpByFarmer
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.hasBeenPickedUpByFarmer;
      set => this.hasBeenPickedUpByFarmer.Value = value;
    }

    [XmlIgnore]
    public bool IsHoeDirt => (bool) (NetFieldBase<bool, NetBool>) this.isHoedirt;

    [XmlIgnore]
    public bool IsOn
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isOn;
      set => this.isOn.Value = value;
    }

    [XmlIgnore]
    public bool IsSpawnedObject
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isSpawnedObject;
      set => this.isSpawnedObject.Value = value;
    }

    [XmlIgnore]
    public bool IsRecipe
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.isRecipe;
      set => this.isRecipe.Value = value;
    }

    [XmlIgnore]
    public bool Flipped
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.flipped;
      set => this.flipped.Value = value;
    }

    [XmlIgnore]
    public int Price
    {
      get => (int) (NetFieldBase<int, NetInt>) this.price;
      set => this.price.Value = value;
    }

    [XmlIgnore]
    public int Edibility
    {
      get => (int) (NetFieldBase<int, NetInt>) this.edibility;
      set => this.edibility.Value = value;
    }

    [XmlIgnore]
    public int Fragility
    {
      get => (int) (NetFieldBase<int, NetInt>) this.fragility;
      set => this.fragility.Value = value;
    }

    [XmlIgnore]
    public Vector2 Scale
    {
      get => this.scale;
      set => this.scale = value;
    }

    [XmlIgnore]
    public int MinutesUntilReady
    {
      get => (int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady;
      set => this.minutesUntilReady.Value = value;
    }

    protected virtual void initNetFields() => this.NetFields.AddFields((INetSerializable) this.tileLocation, (INetSerializable) this.owner, (INetSerializable) this.type, (INetSerializable) this.canBeSetDown, (INetSerializable) this.canBeGrabbed, (INetSerializable) this.isHoedirt, (INetSerializable) this.isSpawnedObject, (INetSerializable) this.questItem, (INetSerializable) this.questId, (INetSerializable) this.isOn, (INetSerializable) this.fragility, (INetSerializable) this.price, (INetSerializable) this.edibility, (INetSerializable) this.stack, (INetSerializable) this.quality, (INetSerializable) this.uses, (INetSerializable) this.bigCraftable, (INetSerializable) this.setOutdoors, (INetSerializable) this.setIndoors, (INetSerializable) this.readyForHarvest, (INetSerializable) this.showNextIndex, (INetSerializable) this.flipped, (INetSerializable) this.hasBeenPickedUpByFarmer, (INetSerializable) this.isRecipe, (INetSerializable) this.isLamp, (INetSerializable) this.heldObject, (INetSerializable) this.minutesUntilReady, (INetSerializable) this.boundingBox, (INetSerializable) this.preserve, (INetSerializable) this.preservedParentSheetIndex, (INetSerializable) this.honeyType, (INetSerializable) this.netLightSource, (INetSerializable) this.orderData, (INetSerializable) this._destroyOvernight);

    public Object() => this.initNetFields();

    /// <summary>constructor for big craftables</summary>
    /// <param name="tileLocation"></param>
    /// <param name="parentSheetIndex"></param>
    /// <param name="isRecipe"></param>
    public Object(Vector2 tileLocation, int parentSheetIndex, bool isRecipe = false)
      : this()
    {
      this.isRecipe.Value = isRecipe;
      this.tileLocation.Value = tileLocation;
      this.ParentSheetIndex = parentSheetIndex;
      this.canBeSetDown.Value = true;
      this.bigCraftable.Value = true;
      string str;
      Game1.bigCraftablesInformation.TryGetValue(parentSheetIndex, out str);
      if (str != null)
      {
        string[] strArray1 = str.Split('/');
        this.name = strArray1[0];
        this.price.Value = Convert.ToInt32(strArray1[1]);
        this.edibility.Value = Convert.ToInt32(strArray1[2]);
        string[] strArray2 = strArray1[3].Split(' ');
        this.type.Value = strArray2[0];
        if (strArray2.Length > 1)
          this.Category = Convert.ToInt32(strArray2[1]);
        this.setOutdoors.Value = Convert.ToBoolean(strArray1[5]);
        this.setIndoors.Value = Convert.ToBoolean(strArray1[6]);
        this.fragility.Value = Convert.ToInt32(strArray1[7]);
        this.isLamp.Value = strArray1.Length > 8 && strArray1[8].Equals("true");
      }
      this.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 64, 64);
    }

    public Object(int parentSheetIndex, int initialStack, bool isRecipe = false, int price = -1, int quality = 0)
      : this(Vector2.Zero, parentSheetIndex, initialStack)
    {
      this.isRecipe.Value = isRecipe;
      if (price != -1)
        this.price.Value = price;
      this.quality.Value = quality;
    }

    public Object(Vector2 tileLocation, int parentSheetIndex, int initialStack)
      : this(tileLocation, parentSheetIndex, (string) null, true, true, false, false)
    {
      this.stack.Value = initialStack;
    }

    public Object(
      Vector2 tileLocation,
      int parentSheetIndex,
      string Givenname,
      bool canBeSetDown,
      bool canBeGrabbed,
      bool isHoedirt,
      bool isSpawnedObject)
      : this()
    {
      this.tileLocation.Value = tileLocation;
      this.ParentSheetIndex = parentSheetIndex;
      string str;
      Game1.objectInformation.TryGetValue(parentSheetIndex, out str);
      try
      {
        if (str != null)
        {
          string[] strArray1 = str.Split('/');
          this.name = strArray1[0];
          this.price.Value = Convert.ToInt32(strArray1[1]);
          this.edibility.Value = Convert.ToInt32(strArray1[2]);
          string[] strArray2 = strArray1[3].Split(' ');
          this.type.Value = strArray2[0];
          if (strArray2.Length > 1)
            this.Category = Convert.ToInt32(strArray2[1]);
        }
      }
      catch (Exception ex)
      {
      }
      if (this.name == null && Givenname != null)
        this.name = Givenname;
      else if (this.name == null)
        this.name = "Error Item";
      this.canBeSetDown.Value = canBeSetDown;
      this.canBeGrabbed.Value = canBeGrabbed;
      this.isHoedirt.Value = isHoedirt;
      this.isSpawnedObject.Value = isSpawnedObject;
      if (Game1.random.NextDouble() < 0.5 && parentSheetIndex > 52 && (parentSheetIndex < 8 || parentSheetIndex > 15) && (parentSheetIndex < 384 || parentSheetIndex > 391))
        this.flipped.Value = true;
      if (this.name.Contains("Block"))
        this.scale = new Vector2(1f, 1f);
      if (parentSheetIndex == 449 || this.name.Contains("Weed") || this.name.Contains("Twig"))
        this.fragility.Value = 2;
      else if (this.name.Contains("Fence"))
      {
        this.scale = new Vector2(10f, 0.0f);
        canBeSetDown = false;
      }
      else if (this.name.Contains("Stone"))
      {
        switch (parentSheetIndex)
        {
          case 8:
            this.minutesUntilReady.Value = 4;
            break;
          case 10:
            this.minutesUntilReady.Value = 8;
            break;
          case 12:
            this.minutesUntilReady.Value = 16;
            break;
          case 14:
            this.minutesUntilReady.Value = 12;
            break;
          case 25:
            this.minutesUntilReady.Value = 8;
            break;
          default:
            this.minutesUntilReady.Value = 1;
            break;
        }
      }
      if (parentSheetIndex >= 75 && parentSheetIndex <= 77)
        isSpawnedObject = false;
      this.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      if (this.Category == -22)
        this.scale.Y = 1f;
      this.boundingBox.Value = new Microsoft.Xna.Framework.Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 64, 64);
    }

    protected override void _PopulateContextTags(HashSet<string> tags)
    {
      base._PopulateContextTags(tags);
      if (this.quality.Value == 0)
        tags.Add("quality_none");
      else if (this.quality.Value == 1)
        tags.Add("quality_silver");
      else if (this.quality.Value == 2)
        tags.Add("quality_gold");
      else if (this.quality.Value == 4)
        tags.Add("quality_iridium");
      if (this.orderData.Value == "QI_COOKING")
        tags.Add("quality_qi");
      if ((NetFieldBase<Object.PreserveType?, NetNullableEnum<Object.PreserveType>>) this.preserve != (NetNullableEnum<Object.PreserveType>) null && this.preserve.Value.HasValue)
      {
        Object.PreserveType? nullable = this.preserve.Value;
        Object.PreserveType preserveType1 = Object.PreserveType.Jelly;
        if (nullable.GetValueOrDefault() == preserveType1 & nullable.HasValue)
        {
          tags.Add("jelly_item");
        }
        else
        {
          nullable = this.preserve.Value;
          Object.PreserveType preserveType2 = Object.PreserveType.Juice;
          if (nullable.GetValueOrDefault() == preserveType2 & nullable.HasValue)
          {
            tags.Add("juice_item");
          }
          else
          {
            nullable = this.preserve.Value;
            Object.PreserveType preserveType3 = Object.PreserveType.Wine;
            if (nullable.GetValueOrDefault() == preserveType3 & nullable.HasValue)
            {
              tags.Add("wine_item");
            }
            else
            {
              nullable = this.preserve.Value;
              Object.PreserveType preserveType4 = Object.PreserveType.Pickle;
              if (nullable.GetValueOrDefault() == preserveType4 & nullable.HasValue)
                tags.Add("pickle_item");
            }
          }
        }
      }
      if (this.preservedParentSheetIndex.Value <= 0)
        return;
      tags.Add("preserve_sheet_index_" + this.preservedParentSheetIndex.Value.ToString());
    }

    protected virtual string loadDisplayName()
    {
      if (this.preserve.Value.HasValue)
      {
        string str;
        Game1.objectInformation.TryGetValue((int) (NetFieldBase<int, NetInt>) this.preservedParentSheetIndex, out str);
        if (!string.IsNullOrEmpty(str))
        {
          string sub1 = str.Split('/')[4];
          Object.PreserveType? nullable = this.preserve.Value;
          if (nullable.HasValue)
          {
            switch (nullable.GetValueOrDefault())
            {
              case Object.PreserveType.Wine:
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12730", (object) sub1);
              case Object.PreserveType.Jelly:
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12739", (object) sub1);
              case Object.PreserveType.Pickle:
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12735", (object) sub1);
              case Object.PreserveType.Juice:
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12726", (object) sub1);
              case Object.PreserveType.Roe:
                return Game1.content.LoadString("Strings\\StringsFromCSFiles:Roe_DisplayName", (object) sub1);
              case Object.PreserveType.AgedRoe:
                if (this.preservedParentSheetIndex.Value > 0)
                  return Game1.content.LoadString("Strings\\StringsFromCSFiles:AgedRoe_DisplayName", (object) sub1);
                break;
            }
          }
        }
      }
      else
      {
        if (this.name != null && this.name.Contains("Honey"))
        {
          int num = this.preservedParentSheetIndex.Value;
          if (this.preservedParentSheetIndex.Value == -1)
          {
            if (this.Name == "Honey")
              this.Name = "Wild Honey";
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12750");
          }
          if (this.preservedParentSheetIndex.Value == 0)
          {
            string str;
            Game1.objectInformation.TryGetValue((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, out str);
            if (!string.IsNullOrEmpty(str))
              return str.Split('/')[4];
          }
          string sub1 = Game1.objectInformation[this.preservedParentSheetIndex.Value].Split('/')[4];
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12760", (object) sub1);
        }
        if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
        {
          string str;
          Game1.bigCraftablesInformation.TryGetValue((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, out str);
          if (!string.IsNullOrEmpty(str))
          {
            string[] strArray = str.Split('/');
            return strArray[strArray.Length - 1];
          }
        }
        else
        {
          string str;
          Game1.objectInformation.TryGetValue((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, out str);
          if (!string.IsNullOrEmpty(str))
            return str.Split('/')[4];
        }
      }
      return this.name;
    }

    public Vector2 getLocalPosition(xTile.Dimensions.Rectangle viewport) => new Vector2(this.tileLocation.X * 64f - (float) viewport.X, this.tileLocation.Y * 64f - (float) viewport.Y);

    public static Microsoft.Xna.Framework.Rectangle getSourceRectForBigCraftable(int index) => new Microsoft.Xna.Framework.Rectangle(index % (Game1.bigCraftableSpriteSheet.Width / 16) * 16, index * 16 / Game1.bigCraftableSpriteSheet.Width * 16 * 2, 16, 32);

    public virtual bool performToolAction(Tool t, GameLocation location)
    {
      if (this.isTemporarilyInvisible)
        return false;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 165 && this.heldObject.Value != null && this.heldObject.Value is Chest && !(this.heldObject.Value as Chest).isEmpty())
      {
        (this.heldObject.Value as Chest).clearNulls();
        if (t != null && t.isHeavyHitter() && !(t is MeleeWeapon))
        {
          location.playSound("hammer");
          this.shakeTimer = 100;
        }
        return false;
      }
      if (t == null)
      {
        if (location.objects.ContainsKey((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation) && location.objects[(Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation].Equals((object) this))
        {
          if (location.farmers.Count > 0)
            Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 10), false);
          location.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
        }
        return false;
      }
      if (this.name.Equals("Stone") && t is Pickaxe)
      {
        int num = 1;
        switch ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel)
        {
          case 1:
            num = 2;
            break;
          case 2:
            num = 3;
            break;
          case 3:
            num = 4;
            break;
          case 4:
            num = 5;
            break;
        }
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 12 && (int) (NetFieldBase<int, NetInt>) t.upgradeLevel == 1 || ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 12 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 14) && (int) (NetFieldBase<int, NetInt>) t.upgradeLevel == 0)
        {
          num = 0;
          location.playSound("crafting");
        }
        this.minutesUntilReady.Value -= num;
        if ((int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady <= 0)
          return true;
        location.playSound("hammer");
        this.shakeTimer = 100;
        return false;
      }
      if (this.name.Equals("Stone") && t is Pickaxe)
        return false;
      if (this.name.Equals("Boulder") && ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel != 4 || !(t is Pickaxe)))
      {
        if (t.isHeavyHitter())
          location.playSound("hammer");
        return false;
      }
      if (this.name.Contains("Weeds") && t.isHeavyHitter())
      {
        if (this.ParentSheetIndex != 319 && this.ParentSheetIndex != 320 && this.ParentSheetIndex != 321 && t.getLastFarmerToUse() != null)
        {
          foreach (BaseEnchantment enchantment in t.getLastFarmerToUse().enchantments)
            enchantment.OnCutWeed((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, location, t.getLastFarmerToUse());
        }
        this.cutWeed(t.getLastFarmerToUse(), location);
        return true;
      }
      if (this.name.Contains("Twig") && t is Axe)
      {
        this.fragility.Value = 2;
        location.playSound("axchop");
        t.getLastFarmerToUse().currentLocation.debris.Add(new Debris((Item) new Object(388, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
        Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 10), false);
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
        return true;
      }
      if (this.name.Contains("SupplyCrate") && t.isHeavyHitter())
      {
        this.MinutesUntilReady -= (int) (NetFieldBase<int, NetInt>) t.upgradeLevel + 1;
        if (this.MinutesUntilReady <= 0)
        {
          this.fragility.Value = 2;
          location.playSound("barrelBreak");
          Random random = new Random((int) ((double) Game1.uniqueIDForThisGame + (double) this.tileLocation.X * 777.0 + (double) this.tileLocation.Y * 7.0));
          int houseUpgradeLevel = t.getLastFarmerToUse().HouseUpgradeLevel;
          int x = (int) this.tileLocation.X;
          int y = (int) this.tileLocation.Y;
          switch (houseUpgradeLevel)
          {
            case 0:
              switch (random.Next(6))
              {
                case 0:
                  Game1.createMultipleObjectDebris(770, x, y, random.Next(3, 6), location);
                  break;
                case 1:
                  Game1.createMultipleObjectDebris(371, x, y, random.Next(5, 8), location);
                  break;
                case 2:
                  Game1.createMultipleObjectDebris(535, x, y, random.Next(2, 5), location);
                  break;
                case 3:
                  Game1.createMultipleObjectDebris(241, x, y, random.Next(1, 3), location);
                  break;
                case 4:
                  Game1.createMultipleObjectDebris(395, x, y, random.Next(1, 3), location);
                  break;
                case 5:
                  Game1.createMultipleObjectDebris(286, x, y, random.Next(3, 6), location);
                  break;
              }
              break;
            case 1:
              switch (random.Next(9))
              {
                case 0:
                  Game1.createMultipleObjectDebris(770, x, y, random.Next(3, 6), location);
                  break;
                case 1:
                  Game1.createMultipleObjectDebris(371, x, y, random.Next(5, 8), location);
                  break;
                case 2:
                  Game1.createMultipleObjectDebris(749, x, y, random.Next(2, 5), location);
                  break;
                case 3:
                  Game1.createMultipleObjectDebris(253, x, y, random.Next(1, 3), location);
                  break;
                case 4:
                  Game1.createMultipleObjectDebris(237, x, y, random.Next(1, 3), location);
                  break;
                case 5:
                  Game1.createMultipleObjectDebris(246, x, y, random.Next(4, 8), location);
                  break;
                case 6:
                  Game1.createMultipleObjectDebris(247, x, y, random.Next(2, 5), location);
                  break;
                case 7:
                  Game1.createMultipleObjectDebris(245, x, y, random.Next(4, 8), location);
                  break;
                case 8:
                  Game1.createMultipleObjectDebris(287, x, y, random.Next(3, 6), location);
                  break;
              }
              break;
            default:
              switch (random.Next(8))
              {
                case 0:
                  Game1.createMultipleObjectDebris(770, x, y, random.Next(3, 6), location);
                  break;
                case 1:
                  Game1.createMultipleObjectDebris(920, x, y, random.Next(5, 8), location);
                  break;
                case 2:
                  Game1.createMultipleObjectDebris(749, x, y, random.Next(2, 5), location);
                  break;
                case 3:
                  Game1.createMultipleObjectDebris(253, x, y, random.Next(2, 4), location);
                  break;
                case 4:
                  Game1.createMultipleObjectDebris(random.Next(904, 906), x, y, random.Next(1, 3), location);
                  break;
                case 5:
                  Game1.createMultipleObjectDebris(246, x, y, random.Next(4, 8), location);
                  Game1.createMultipleObjectDebris(247, x, y, random.Next(2, 5), location);
                  Game1.createMultipleObjectDebris(245, x, y, random.Next(4, 8), location);
                  break;
                case 6:
                  Game1.createMultipleObjectDebris(275, x, y, 2, location);
                  break;
                case 7:
                  Game1.createMultipleObjectDebris(288, x, y, random.Next(3, 6), location);
                  break;
              }
              break;
          }
          Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 10), false);
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
          return true;
        }
        this.shakeTimer = 200;
        location.playSound("woodWhack");
        return false;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 590)
      {
        if (t is Hoe)
        {
          location.digUpArtifactSpot((int) this.tileLocation.X, (int) this.tileLocation.Y, t.getLastFarmerToUse());
          if (!location.terrainFeatures.ContainsKey((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation))
            location.makeHoeDirt((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, true);
          location.playSound("hoeHit");
          if (location.objects.ContainsKey((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation))
            location.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
        }
        return false;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.fragility == 2 || !((NetFieldBase<string, NetString>) this.type != (NetString) null) || !this.type.Equals((object) "Crafting") || t is MeleeWeapon || !t.isHeavyHitter() || t is Hoe && this.IsSprinkler())
        return false;
      location.playSound("hammer");
      if ((int) (NetFieldBase<int, NetInt>) this.fragility == 1)
      {
        Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(3, 6), false);
        Game1.createRadialDebris(location, 14, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(3, 6), false);
        DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
        {
          Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(2, 5), false);
          Game1.createRadialDebris(location, 14, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(2, 5), false);
        }), 80);
        Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(this.tileLocation.X * 64f, this.tileLocation.Y * 64f), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
        this.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, location);
        if (location.objects.ContainsKey((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation))
          location.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
        return false;
      }
      if (this.name.Contains("Tapper") && t.getLastFarmerToUse().currentLocation.terrainFeatures.ContainsKey((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation) && t.getLastFarmerToUse().currentLocation.terrainFeatures[(Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation] is Tree)
        (t.getLastFarmerToUse().currentLocation.terrainFeatures[(Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation] as Tree).tapped.Value = false;
      if (this.Name == "Ostrich Incubator")
      {
        if (this.heldObject.Value == null)
          return true;
        --this.ParentSheetIndex;
        t.getLastFarmerToUse().currentLocation.debris.Add(new Debris((Item) (Object) (NetFieldBase<Object, NetRef<Object>>) this.heldObject, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
        this.heldObject.Value = (Object) null;
        return true;
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && this.ParentSheetIndex == 21 && this.heldObject.Value != null)
      {
        t.getLastFarmerToUse().currentLocation.debris.Add(new Debris((Item) (Object) (NetFieldBase<Object, NetRef<Object>>) this.heldObject, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
        this.heldObject.Value = (Object) null;
      }
      if (this.IsSprinkler() && this.heldObject.Value != null)
      {
        if (this.heldObject.Value.heldObject.Value != null)
        {
          Chest chest = this.heldObject.Value.heldObject.Value as Chest;
          if (chest != null)
            chest.GetMutex().RequestLock((Action) (() =>
            {
              List<Item> objList = new List<Item>((IEnumerable<Item>) chest.items);
              chest.items.Clear();
              foreach (Item obj in objList)
              {
                if (obj != null)
                  t.getLastFarmerToUse().currentLocation.debris.Add(new Debris(obj, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
              }
              Object @object = this.heldObject.Value;
              this.heldObject.Value = (Object) null;
              t.getLastFarmerToUse().currentLocation.debris.Add(new Debris((Item) @object, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
              chest.GetMutex().ReleaseLock();
            }));
          return false;
        }
        t.getLastFarmerToUse().currentLocation.debris.Add(new Debris((Item) (Object) (NetFieldBase<Object, NetRef<Object>>) this.heldObject, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
        this.heldObject.Value = (Object) null;
        return false;
      }
      if (this.heldObject.Value != null && (bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest)
        t.getLastFarmerToUse().currentLocation.debris.Add(new Debris((Item) (Object) (NetFieldBase<Object, NetRef<Object>>) this.heldObject, this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 157)
      {
        this.ParentSheetIndex = 156;
        this.heldObject.Value = (Object) null;
        this.minutesUntilReady.Value = -1;
      }
      if (this.name.Contains("Seasonal"))
        this.ParentSheetIndex -= this.ParentSheetIndex % 4;
      return true;
    }

    protected virtual void cutWeed(Farmer who, GameLocation location = null)
    {
      if (location == null && who != null)
        location = who.currentLocation;
      Color color = Color.Green;
      string audioName = "cut";
      int rowInAnimationTexture = 50;
      this.fragility.Value = 2;
      int parentSheetIndex = -1;
      if (Game1.random.NextDouble() < 0.5)
        parentSheetIndex = 771;
      else if (Game1.random.NextDouble() < 0.05)
        parentSheetIndex = 770;
      switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
      {
        case 313:
        case 314:
        case 315:
          color = new Color(84, 101, 27);
          break;
        case 316:
        case 317:
        case 318:
          color = new Color(109, 49, 196);
          break;
        case 319:
          color = new Color(30, 216, (int) byte.MaxValue);
          audioName = "breakingGlass";
          rowInAnimationTexture = 47;
          location.playSound("drumkit2");
          parentSheetIndex = -1;
          break;
        case 320:
          color = new Color(175, 143, (int) byte.MaxValue);
          audioName = "breakingGlass";
          rowInAnimationTexture = 47;
          location.playSound("drumkit2");
          parentSheetIndex = -1;
          break;
        case 321:
          color = new Color(73, (int) byte.MaxValue, 158);
          audioName = "breakingGlass";
          rowInAnimationTexture = 47;
          location.playSound("drumkit2");
          parentSheetIndex = -1;
          break;
        case 678:
          color = new Color(228, 109, 159);
          break;
        case 679:
          color = new Color(253, 191, 46);
          break;
        case 792:
        case 793:
        case 794:
          parentSheetIndex = 770;
          break;
        case 882:
        case 883:
        case 884:
          color = new Color(30, 97, 68);
          if (Game1.MasterPlayer.hasOrWillReceiveMail("islandNorthCaveOpened") && Game1.random.NextDouble() < 0.1 && !Game1.MasterPlayer.hasOrWillReceiveMail("gotMummifiedFrog"))
          {
            Game1.addMailForTomorrow("gotMummifiedFrog", true, true);
            parentSheetIndex = 828;
            break;
          }
          if (Game1.random.NextDouble() < 0.01)
          {
            parentSheetIndex = 828;
            break;
          }
          if (Game1.random.NextDouble() < 0.08)
          {
            parentSheetIndex = 831;
            break;
          }
          break;
      }
      if (audioName.Equals("breakingGlass") && Game1.random.NextDouble() < 1.0 / 400.0)
        parentSheetIndex = 338;
      location.playSound(audioName);
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(rowInAnimationTexture, this.tileLocation.Value * 64f, color));
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(rowInAnimationTexture, this.tileLocation.Value * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), color * 0.75f)
      {
        scale = 0.75f,
        flipped = true
      });
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(rowInAnimationTexture, this.tileLocation.Value * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), color * 0.75f)
      {
        scale = 0.75f,
        delayBeforeAnimationStart = 50
      });
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(rowInAnimationTexture, this.tileLocation.Value * 64f + new Vector2((float) Game1.random.Next(-16, 16), (float) Game1.random.Next(-48, 48)), color * 0.75f)
      {
        scale = 0.75f,
        flipped = true,
        delayBeforeAnimationStart = 100
      });
      if (!audioName.Equals("breakingGlass"))
      {
        if (Game1.random.NextDouble() < 1E-05)
          location.debris.Add(new Debris((Item) new Hat(40), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
        if (Game1.random.NextDouble() <= 0.01 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
          location.debris.Add(new Debris((Item) new Object(890, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
      }
      if (parentSheetIndex != -1)
        location.debris.Add(new Debris((Item) new Object(parentSheetIndex, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
      if (Game1.random.NextDouble() < 0.02)
        location.addJumperFrog((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      if (!who.currentLocation.HasUnlockedAreaSecretNotes(who) || Game1.random.NextDouble() >= 0.009)
        return;
      Object unseenSecretNote = location.tryToCreateUnseenSecretNote(who);
      if (unseenSecretNote == null)
        return;
      Game1.createItemDebris((Item) unseenSecretNote, new Vector2(this.tileLocation.X + 0.5f, this.tileLocation.Y + 0.75f) * 64f, (int) Game1.player.facingDirection, location);
    }

    public virtual bool isAnimalProduct() => this.Category == -18 || this.Category == -5 || this.Category == -6 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 430;

    public virtual bool onExplosion(Farmer who, GameLocation location)
    {
      if (who == null)
        return false;
      if (this.name.Contains("Weed"))
      {
        this.fragility.Value = 0;
        this.cutWeed(who, location);
        location.removeObject((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, false);
      }
      if (this.name.Contains("Twig"))
      {
        this.fragility.Value = 0;
        Game1.createRadialDebris(location, 12, (int) this.tileLocation.X, (int) this.tileLocation.Y, Game1.random.Next(4, 10), false);
        location.debris.Add(new Debris((Item) new Object(388, 1), this.tileLocation.Value * 64f + new Vector2(32f, 32f)));
      }
      if (this.name.Contains("Stone"))
        this.fragility.Value = 0;
      this.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, location);
      return true;
    }

    public virtual bool canBeShipped() => !(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && (NetFieldBase<string, NetString>) this.type != (NetString) null && !this.type.Equals((object) "Quest") && this.canBeTrashed() && !(this is Furniture) && !(this is Wallpaper);

    public virtual void ApplySprinkler(GameLocation location, Vector2 tile)
    {
      if (location.doesTileHavePropertyNoNull((int) tile.X, (int) tile.Y, "NoSprinklers", "Back") == "T" || !location.terrainFeatures.ContainsKey(tile) || !(location.terrainFeatures[tile] is HoeDirt) || (int) (NetFieldBase<int, NetInt>) (location.terrainFeatures[tile] as HoeDirt).state == 2)
        return;
      (location.terrainFeatures[tile] as HoeDirt).state.Value = 1;
    }

    public virtual void ApplySprinklerAnimation(GameLocation location)
    {
      int radiusForSprinkler = this.GetModifiedRadiusForSprinkler();
      if (radiusForSprinkler < 0)
        return;
      if (radiusForSprinkler == 0)
      {
        int num = Game1.random.Next(1000);
        location.temporarySprites.Add(new TemporaryAnimatedSprite(29, this.tileLocation.Value * 64f + new Vector2(0.0f, -48f), Color.White * 0.5f, 4, animationInterval: 60f, numberOfLoops: 100)
        {
          delayBeforeAnimationStart = num,
          id = this.tileLocation.X * 4000f + this.tileLocation.Y
        });
        location.temporarySprites.Add(new TemporaryAnimatedSprite(29, this.tileLocation.Value * 64f + new Vector2(48f, 0.0f), Color.White * 0.5f, 4, animationInterval: 60f, numberOfLoops: 100)
        {
          rotation = 1.570796f,
          delayBeforeAnimationStart = num,
          id = this.tileLocation.X * 4000f + this.tileLocation.Y
        });
        location.temporarySprites.Add(new TemporaryAnimatedSprite(29, this.tileLocation.Value * 64f + new Vector2(0.0f, 48f), Color.White * 0.5f, 4, animationInterval: 60f, numberOfLoops: 100)
        {
          rotation = 3.141593f,
          delayBeforeAnimationStart = num,
          id = this.tileLocation.X * 4000f + this.tileLocation.Y
        });
        location.temporarySprites.Add(new TemporaryAnimatedSprite(29, this.tileLocation.Value * 64f + new Vector2(-48f, 0.0f), Color.White * 0.5f, 4, animationInterval: 60f, numberOfLoops: 100)
        {
          rotation = 4.712389f,
          delayBeforeAnimationStart = num,
          id = this.tileLocation.X * 4000f + this.tileLocation.Y
        });
      }
      else if (radiusForSprinkler == 1)
      {
        location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 1984, 192, 192), 60f, 3, 100, this.tileLocation.Value * 64f + new Vector2(-64f, -64f), false, false)
        {
          color = Color.White * 0.4f,
          delayBeforeAnimationStart = Game1.random.Next(1000),
          id = this.tileLocation.X * 4000f + this.tileLocation.Y
        });
      }
      else
      {
        float num = (float) radiusForSprinkler / 2f;
        location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 2176, 320, 320), 60f, 4, 100, this.tileLocation.Value * 64f + new Vector2(32f, 32f) + new Vector2(-160f, -160f) * num, false, false)
        {
          color = Color.White * 0.4f,
          delayBeforeAnimationStart = Game1.random.Next(1000),
          id = this.tileLocation.X * 4000f + this.tileLocation.Y,
          scale = num
        });
      }
    }

    public virtual List<Vector2> GetSprinklerTiles()
    {
      int radiusForSprinkler = this.GetModifiedRadiusForSprinkler();
      if (radiusForSprinkler == 0)
        return Utility.getAdjacentTileLocations((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      if (radiusForSprinkler <= 0)
        return new List<Vector2>();
      List<Vector2> sprinklerTiles = new List<Vector2>();
      for (int x = (int) this.tileLocation.X - radiusForSprinkler; (double) x <= (double) this.tileLocation.X + (double) radiusForSprinkler; ++x)
      {
        for (int y = (int) this.tileLocation.Y - radiusForSprinkler; (double) y <= (double) this.tileLocation.Y + (double) radiusForSprinkler; ++y)
        {
          if (x != 0 || y != 0)
            sprinklerTiles.Add(new Vector2((float) x, (float) y));
        }
      }
      return sprinklerTiles;
    }

    public virtual bool IsInSprinklerRangeBroadphase(Vector2 target)
    {
      int num = this.GetModifiedRadiusForSprinkler();
      if (num == 0)
        num = 1;
      return (double) Math.Abs(target.X - this.TileLocation.X) <= (double) num && (double) Math.Abs(target.Y - this.TileLocation.Y) <= (double) num;
    }

    public virtual void DayUpdate(GameLocation location)
    {
      this.health = 10;
      if (this.IsSprinkler() && (!Game1.IsRainingHere(location) || !(bool) (NetFieldBase<bool, NetBool>) location.isOutdoors) && this.GetModifiedRadiusForSprinkler() >= 0)
        location.postFarmEventOvernightActions.Add((Action) (() =>
        {
          if (Game1.player.team.SpecialOrderRuleActive("NO_SPRINKLER"))
            return;
          foreach (Vector2 sprinklerTile in this.GetSprinklerTiles())
            this.ApplySprinkler(location, sprinklerTile);
          this.ApplySprinklerAnimation(location);
        }));
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
        {
          case 10:
            if (location.GetSeasonForLocation().Equals("winter"))
            {
              this.heldObject.Value = (Object) null;
              this.readyForHarvest.Value = false;
              this.showNextIndex.Value = false;
              this.minutesUntilReady.Value = -1;
              break;
            }
            if (this.heldObject.Value == null)
            {
              this.heldObject.Value = new Object(Vector2.Zero, 340, (string) null, false, true, false, false);
              this.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
              break;
            }
            break;
          case 104:
            if (Game1.currentSeason.Equals("winter"))
            {
              this.minutesUntilReady.Value = 9999;
              break;
            }
            this.minutesUntilReady.Value = -1;
            break;
          case 108:
          case 109:
            this.ParentSheetIndex = 108;
            if (Game1.currentSeason.Equals("winter") || Game1.currentSeason.Equals("fall"))
            {
              this.ParentSheetIndex = 109;
              break;
            }
            break;
          case 117:
            this.heldObject.Value = new Object(167, 1);
            break;
          case (int) sbyte.MaxValue:
            NPC todaysBirthdayNpc = Utility.getTodaysBirthdayNPC(Game1.currentSeason, Game1.dayOfMonth);
            this.minutesUntilReady.Value = 1;
            if (todaysBirthdayNpc != null)
            {
              this.heldObject.Value = todaysBirthdayNpc.getFavoriteItem();
              break;
            }
            int parentSheetIndex = 80;
            switch (Game1.random.Next(4))
            {
              case 0:
                parentSheetIndex = 72;
                break;
              case 1:
                parentSheetIndex = 337;
                break;
              case 2:
                parentSheetIndex = 749;
                break;
              case 3:
                parentSheetIndex = 336;
                break;
            }
            this.heldObject.Value = new Object(parentSheetIndex, 1);
            break;
          case 128:
            if (this.heldObject.Value == null)
            {
              this.heldObject.Value = new Object(Game1.random.NextDouble() >= 0.025 ? (Game1.random.NextDouble() >= 0.075 ? (Game1.random.NextDouble() >= 0.09 ? (Game1.random.NextDouble() >= 0.15 ? 404 : 420) : 257) : 281) : 422, 1);
              Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 2);
              break;
            }
            break;
          case 157:
            if ((int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady <= 0 && this.heldObject.Value != null && location.canSlimeHatchHere())
            {
              GreenSlime c = (GreenSlime) null;
              Vector2 position = new Vector2((float) (int) this.tileLocation.X, (float) ((int) this.tileLocation.Y + 1)) * 64f;
              switch ((int) (NetFieldBase<int, NetInt>) this.heldObject.Value.parentSheetIndex)
              {
                case 413:
                  c = new GreenSlime(position, 40);
                  break;
                case 437:
                  c = new GreenSlime(position, 80);
                  break;
                case 439:
                  c = new GreenSlime(position, 121);
                  break;
                case 680:
                  c = new GreenSlime(position, 0);
                  break;
                case 857:
                  c = new GreenSlime(position, 121);
                  c.makeTigerSlime();
                  break;
              }
              if (c != null)
              {
                Game1.showGlobalMessage((bool) (NetFieldBase<bool, NetBool>) c.cute ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12689") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12691"));
                Vector2 tileForCharacter = Utility.recursiveFindOpenTileForCharacter((Character) c, location, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation + new Vector2(0.0f, 1f), 10, false);
                c.setTilePosition((int) tileForCharacter.X, (int) tileForCharacter.Y);
                location.characters.Add((NPC) c);
                this.heldObject.Value = (Object) null;
                this.ParentSheetIndex = 156;
                this.minutesUntilReady.Value = -1;
                break;
              }
              break;
            }
            break;
          case 160:
            this.minutesUntilReady.Value = 1;
            this.heldObject.Value = new Object(386, Game1.random.Next(2, 9));
            break;
          case 164:
            if (location is Town)
            {
              if (Game1.random.NextDouble() < 0.9)
              {
                if (Game1.getLocationFromName("ManorHouse").isTileLocationTotallyClearAndPlaceable(22, 6))
                {
                  if (!Game1.player.hasOrWillReceiveMail("lewisStatue"))
                    Game1.mailbox.Add("lewisStatue");
                  this.rot();
                  Game1.getLocationFromName("ManorHouse").objects.Add(new Vector2(22f, 6f), new Object(Vector2.Zero, 164));
                  break;
                }
                break;
              }
              if (Game1.getLocationFromName("AnimalShop").isTileLocationTotallyClearAndPlaceable(11, 6))
              {
                if (!Game1.player.hasOrWillReceiveMail("lewisStatue"))
                  Game1.mailbox.Add("lewisStatue");
                this.rot();
                Game1.getLocationFromName("AnimalShop").objects.Add(new Vector2(11f, 6f), new Object(Vector2.Zero, 164));
                break;
              }
              break;
            }
            break;
          case 165:
            if (location != null && location is AnimalHouse)
            {
              using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection.Enumerator enumerator = (location as AnimalHouse).animals.Pairs.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  KeyValuePair<long, FarmAnimal> current = enumerator.Current;
                  if ((byte) (NetFieldBase<byte, NetByte>) current.Value.harvestType == (byte) 1 && (int) (NetFieldBase<int, NetInt>) current.Value.currentProduce > 0 && (int) (NetFieldBase<int, NetInt>) current.Value.currentProduce != 430 && this.heldObject.Value != null && this.heldObject.Value is Chest)
                  {
                    if ((this.heldObject.Value as Chest).addItem((Item) new Object(Vector2.Zero, current.Value.currentProduce.Value, (string) null, false, true, false, false)
                    {
                      Quality = (int) (NetFieldBase<int, NetInt>) current.Value.produceQuality
                    }) == null)
                    {
                      Utility.RecordAnimalProduce(current.Value, (int) (NetFieldBase<int, NetInt>) current.Value.currentProduce);
                      current.Value.currentProduce.Value = -1;
                      if ((bool) (NetFieldBase<bool, NetBool>) current.Value.showDifferentTextureWhenReadyForHarvest)
                        current.Value.Sprite.LoadTexture("Animals\\Sheared" + current.Value.type.Value);
                      this.showNextIndex.Value = true;
                    }
                  }
                }
                break;
              }
            }
            else
              break;
          case 231:
            if (!Game1.IsRainingHere(location) && location.IsOutdoors)
            {
              this.MinutesUntilReady -= 2400;
              if (this.MinutesUntilReady <= 0)
              {
                this.readyForHarvest.Value = true;
                break;
              }
              break;
            }
            break;
          case 246:
            this.heldObject.Value = new Object(395, 1);
            this.readyForHarvest.Value = true;
            break;
          case 272:
            if (location is AnimalHouse)
            {
              using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection.Enumerator enumerator = (location as AnimalHouse).animals.Pairs.GetEnumerator())
              {
                while (enumerator.MoveNext())
                  enumerator.Current.Value.pet(Game1.player, true);
                break;
              }
            }
            else
              break;
          case 280:
            this.minutesUntilReady.Value = 1;
            this.heldObject.Value = new Object(74, 1);
            break;
        }
        if (this.name.Contains("Seasonal"))
          this.ParentSheetIndex = this.ParentSheetIndex - this.ParentSheetIndex % 4 + Utility.getSeasonNumber(Game1.currentSeason);
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
        return;
      string seasonForLocation = location.GetSeasonForLocation();
      switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
      {
        case 674:
        case 675:
          if (Game1.dayOfMonth != 1 || !seasonForLocation.Equals("summer") || !(bool) (NetFieldBase<bool, NetBool>) location.isOutdoors)
            break;
          this.ParentSheetIndex += 2;
          break;
        case 676:
        case 677:
          if (Game1.dayOfMonth != 1 || !seasonForLocation.Equals("fall") || !(bool) (NetFieldBase<bool, NetBool>) location.isOutdoors)
            break;
          this.ParentSheetIndex += 2;
          break;
        case 746:
          if (!seasonForLocation.Equals("winter"))
            break;
          this.rot();
          break;
        case 784:
        case 785:
          if (Game1.dayOfMonth != 1 || seasonForLocation.Equals("spring") || !(bool) (NetFieldBase<bool, NetBool>) location.isOutdoors)
            break;
          ++this.ParentSheetIndex;
          break;
      }
    }

    public virtual void rot()
    {
      this.ParentSheetIndex = new Random(Game1.year * 999 + Game1.dayOfMonth + Utility.getSeasonNumber(Game1.currentSeason)).Next(747, 749);
      this.price.Value = 0;
      this.quality.Value = 0;
      this.name = "Rotten Plant";
      this.displayName = (string) null;
      this.lightSource = (LightSource) null;
      this.bigCraftable.Value = false;
    }

    public override void actionWhenBeingHeld(Farmer who)
    {
      if (Game1.eventUp && Game1.CurrentEvent != null && Game1.CurrentEvent.isFestival)
      {
        if (this.lightSource != null && who.currentLocation != null && who.currentLocation.hasLightSource((int) who.UniqueMultiplayerID))
          who.currentLocation.removeLightSource((int) who.UniqueMultiplayerID);
        base.actionWhenBeingHeld(who);
      }
      else
      {
        if (this.lightSource != null && (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable || (bool) (NetFieldBase<bool, NetBool>) this.isLamp) && who.currentLocation != null)
        {
          if (!who.currentLocation.hasLightSource((int) who.UniqueMultiplayerID))
            who.currentLocation.sharedLights[(int) who.UniqueMultiplayerID] = new LightSource((int) (NetFieldBase<int, NetInt>) this.lightSource.textureIndex, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.lightSource.position, (float) (NetFieldBase<float, NetFloat>) this.lightSource.radius, (Color) (NetFieldBase<Color, NetColor>) this.lightSource.color, (int) who.UniqueMultiplayerID, playerID: ((long) who.uniqueMultiplayerID));
          who.currentLocation.repositionLightSource((int) who.UniqueMultiplayerID, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) who.position + new Vector2(32f, -64f));
        }
        base.actionWhenBeingHeld(who);
      }
    }

    public override void actionWhenStopBeingHeld(Farmer who)
    {
      if (this.lightSource != null && who.currentLocation != null && who.currentLocation.hasLightSource((int) who.UniqueMultiplayerID))
        who.currentLocation.removeLightSource((int) who.UniqueMultiplayerID);
      base.actionWhenStopBeingHeld(who);
    }

    public virtual void ConsumeInventoryItem(Farmer who, int parent_sheet_index, int amount)
    {
      IList<Item> items = who.Items;
      if (Object.autoLoadChest != null)
        items = (IList<Item>) Object.autoLoadChest.items;
      for (int index = items.Count - 1; index >= 0; --index)
      {
        if (Utility.IsNormalObjectAtParentSheetIndex(items[index], parent_sheet_index))
        {
          --items[index].Stack;
          if (items[index].Stack > 0)
            break;
          if (who.ActiveObject == items[index])
            who.ActiveObject = (Object) null;
          items[index] = (Item) null;
          break;
        }
      }
    }

    public virtual void ConsumeInventoryItem(Farmer who, Item drop_in, int amount)
    {
      drop_in.Stack -= amount;
      if (drop_in.Stack > 0)
        return;
      if (Object.autoLoadChest != null)
      {
        bool flag = false;
        for (int index = 0; index < Object.autoLoadChest.items.Count; ++index)
        {
          if (Object.autoLoadChest.items[index] == drop_in)
          {
            Object.autoLoadChest.items[index] = (Item) null;
            flag = true;
            break;
          }
        }
        if (!flag)
          return;
        Object.autoLoadChest.clearNulls();
      }
      else
        who.removeItemFromInventory(drop_in);
    }

    public virtual int GetTallyOfObject(Farmer who, int index, bool big_craftable)
    {
      if (Object.autoLoadChest == null)
        return who.getTallyOfObject(index, big_craftable);
      int tallyOfObject = 0;
      foreach (Item obj in (NetList<Item, NetRef<Item>>) Object.autoLoadChest.items)
      {
        if (obj != null && obj is Object && (obj as Object).ParentSheetIndex == index && (bool) (NetFieldBase<bool, NetBool>) (obj as Object).bigCraftable == big_craftable)
          tallyOfObject += obj.Stack;
      }
      return tallyOfObject;
    }

    public virtual Object GetDeconstructorOutput(Item item)
    {
      if (!CraftingRecipe.craftingRecipes.ContainsKey(item.Name))
        return (Object) null;
      if (((IEnumerable<string>) CraftingRecipe.craftingRecipes[item.Name].Split('/')[2].Split(' ')).Count<string>() > 1)
        return (Object) null;
      if (Utility.IsNormalObjectAtParentSheetIndex(item, 710))
        return new Object(334, 2);
      string[] source1 = CraftingRecipe.craftingRecipes[item.Name].Split('/')[0].Split(' ');
      List<Object> source2 = new List<Object>();
      for (int index = 0; index < ((IEnumerable<string>) source1).Count<string>(); index += 2)
        source2.Add(new Object(Convert.ToInt32(source1[index]), Convert.ToInt32(source1[index + 1])));
      if (source2.Count == 0)
        return (Object) null;
      source2.Sort((Comparison<Object>) ((a, b) => a.sellToStorePrice() * a.Stack - b.sellToStorePrice() * b.Stack));
      return source2.Last<Object>();
    }

    public virtual bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
    {
      if (this.isTemporarilyInvisible || !(dropInItem is Object))
        return false;
      Object object1 = dropInItem as Object;
      if (this.IsSprinkler() && this.heldObject.Value == null && (Utility.IsNormalObjectAtParentSheetIndex(dropInItem, 915) || Utility.IsNormalObjectAtParentSheetIndex(dropInItem, 913)))
      {
        if (probe)
          return true;
        if (who.currentLocation is MineShaft || who.currentLocation is VolcanoDungeon && Utility.IsNormalObjectAtParentSheetIndex(dropInItem, 913))
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
          return false;
        }
        Object one = object1.getOne() as Object;
        if (Utility.IsNormalObjectAtParentSheetIndex((Item) one, 913) && one.heldObject.Value == null)
          one.heldObject.Value = (Object) new Chest()
          {
            SpecialChestType = Chest.SpecialChestTypes.Enricher
          };
        who.currentLocation.playSound("axe");
        this.heldObject.Value = one;
        this.minutesUntilReady.Value = -1;
        return true;
      }
      if (dropInItem is Wallpaper)
        return false;
      if (object1 != null && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 872 && Object.autoLoadChest == null)
      {
        if (this.Name == "Ostrich Incubator" || this.Name == "Slime Incubator" || this.Name == "Incubator")
          return false;
        if (this.MinutesUntilReady > 0)
        {
          if (probe)
            return true;
          Utility.addSprinklesToLocation(who.currentLocation, (int) this.tileLocation.X, (int) this.tileLocation.Y, 1, 2, 400, 40, Color.White);
          Game1.playSound("yoba");
          this.MinutesUntilReady = 10;
          who.reduceActiveItemByOne();
          DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() => this.minutesElapsed(10, who.currentLocation)), 50);
        }
      }
      if (this.heldObject.Value != null && !this.name.Equals("Recycling Machine") && !this.name.Equals("Crystalarium") || object1 != null && (bool) (NetFieldBase<bool, NetBool>) object1.bigCraftable && !this.name.Equals("Deconstructor"))
        return false;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && !probe && object1 != null && this.heldObject.Value == null)
        this.scale.X = 5f;
      if (probe && this.MinutesUntilReady > 0)
        return false;
      if (this.name.Equals("Incubator"))
      {
        if (this.heldObject.Value == null && object1.ParentSheetIndex != 289 && (object1.Category == -5 || Utility.IsNormalObjectAtParentSheetIndex((Item) object1, 107)))
        {
          this.heldObject.Value = new Object((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex, 1);
          if (!probe)
          {
            who.currentLocation.playSound("coin");
            this.minutesUntilReady.Value = 9000 * ((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 107 ? 2 : 1);
            if (who.professions.Contains(2))
              this.minutesUntilReady.Value /= 2;
            if (object1.ParentSheetIndex == 180 || object1.ParentSheetIndex == 182 || object1.ParentSheetIndex == 305)
              this.ParentSheetIndex += 2;
            else
              ++this.ParentSheetIndex;
            if (who != null && who.currentLocation != null && who.currentLocation is AnimalHouse)
              (who.currentLocation as AnimalHouse).hasShownIncubatorBuildingFullMessage = false;
          }
          return true;
        }
      }
      else if (this.name.Equals("Ostrich Incubator"))
      {
        if (this.heldObject.Value == null && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 289)
        {
          this.heldObject.Value = new Object((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex, 1);
          if (!probe)
          {
            who.currentLocation.playSound("coin");
            this.minutesUntilReady.Value = 15000;
            if (who.professions.Contains(2))
              this.minutesUntilReady.Value /= 2;
            ++this.ParentSheetIndex;
            if (who != null && who.currentLocation != null && who.currentLocation is AnimalHouse)
              (who.currentLocation as AnimalHouse).hasShownIncubatorBuildingFullMessage = false;
          }
          return true;
        }
      }
      else if (this.name.Equals("Slime Incubator"))
      {
        if (this.heldObject.Value == null && object1.name.Contains("Slime Egg"))
        {
          this.heldObject.Value = new Object((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex, 1);
          if (!probe)
          {
            who.currentLocation.playSound("coin");
            this.minutesUntilReady.Value = 4000;
            if (who.professions.Contains(2))
              this.minutesUntilReady.Value /= 2;
            ++this.ParentSheetIndex;
          }
          return true;
        }
      }
      else if (this.name.Equals("Deconstructor"))
      {
        Object deconstructorOutput = this.GetDeconstructorOutput((Item) object1);
        if (deconstructorOutput != null)
        {
          this.heldObject.Value = new Object((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex, 1);
          if (probe)
            return true;
          this.heldObject.Value = deconstructorOutput;
          this.MinutesUntilReady = 60;
          Game1.playSound("furnace");
          return true;
        }
        if (!probe)
        {
          if (Object.autoLoadChest == null)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Deconstructor_fail"));
          return false;
        }
      }
      else if (this.name.Equals("Bone Mill"))
      {
        int amount = 0;
        switch ((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex)
        {
          case 579:
          case 580:
          case 581:
          case 582:
          case 583:
          case 584:
          case 585:
          case 586:
          case 587:
          case 588:
          case 589:
          case 820:
          case 821:
          case 822:
          case 823:
          case 824:
          case 825:
          case 826:
          case 827:
          case 828:
            amount = 1;
            break;
          case 881:
            amount = 5;
            break;
        }
        if (amount == 0)
          return false;
        if (probe)
          return true;
        if (object1.Stack < amount)
        {
          if (Object.autoLoadChest == null)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:bonemill_5"));
          return false;
        }
        int parentSheetIndex = -1;
        int initialStack = 1;
        switch (Game1.random.Next(4))
        {
          case 0:
            parentSheetIndex = 466;
            initialStack = 3;
            break;
          case 1:
            parentSheetIndex = 465;
            initialStack = 5;
            break;
          case 2:
            parentSheetIndex = 369;
            initialStack = 10;
            break;
          case 3:
            parentSheetIndex = 805;
            initialStack = 5;
            break;
        }
        if (Game1.random.NextDouble() < 0.1)
          initialStack *= 2;
        this.heldObject.Value = new Object(parentSheetIndex, initialStack);
        if (!probe)
        {
          this.ConsumeInventoryItem(who, (Item) object1, amount);
          this.minutesUntilReady.Value = 240;
          who.currentLocation.playSound("skeletonStep");
          DelayedAction.playSoundAfterDelay("skeletonHit", 150);
        }
      }
      else if (this.name.Equals("Keg"))
      {
        switch ((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex)
        {
          case 262:
            this.heldObject.Value = new Object(Vector2.Zero, 346, "Beer", false, true, false, false);
            if (!probe)
            {
              this.heldObject.Value.name = "Beer";
              who.currentLocation.playSound("Ship");
              who.currentLocation.playSound("bubbles");
              this.minutesUntilReady.Value = 1750;
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          case 304:
            this.heldObject.Value = new Object(Vector2.Zero, 303, "Pale Ale", false, true, false, false);
            if (!probe)
            {
              this.heldObject.Value.name = "Pale Ale";
              who.currentLocation.playSound("Ship");
              who.currentLocation.playSound("bubbles");
              this.minutesUntilReady.Value = 2250;
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          case 340:
            this.heldObject.Value = new Object(Vector2.Zero, 459, "Mead", false, true, false, false);
            if (!probe)
            {
              this.heldObject.Value.name = "Mead";
              who.currentLocation.playSound("Ship");
              who.currentLocation.playSound("bubbles");
              this.minutesUntilReady.Value = 600;
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          case 433:
            if (object1.Stack < 5 && !probe)
            {
              if (Object.autoLoadChest == null)
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12721"));
              return false;
            }
            this.heldObject.Value = new Object(Vector2.Zero, 395, "Coffee", false, true, false, false);
            if (!probe)
            {
              this.heldObject.Value.name = "Coffee";
              who.currentLocation.playSound("Ship");
              who.currentLocation.playSound("bubbles");
              this.ConsumeInventoryItem(who, (Item) object1, 4);
              this.minutesUntilReady.Value = 120;
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.DarkGray * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          case 815:
            this.heldObject.Value = new Object(Vector2.Zero, 614, "Green Tea", false, true, false, false);
            if (!probe)
            {
              this.heldObject.Value.name = "Green Tea";
              who.currentLocation.playSound("Ship");
              who.currentLocation.playSound("bubbles");
              this.minutesUntilReady.Value = 180;
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Lime * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          default:
            switch (object1.Category)
            {
              case -79:
                this.heldObject.Value = new Object(Vector2.Zero, 348, object1.Name + " Wine", false, true, false, false);
                this.heldObject.Value.Price = object1.Price * 3;
                if (!probe)
                {
                  this.heldObject.Value.name = object1.Name + " Wine";
                  this.heldObject.Value.preserve.Value = new Object.PreserveType?(Object.PreserveType.Wine);
                  this.heldObject.Value.preservedParentSheetIndex.Value = (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex;
                  who.currentLocation.playSound("Ship");
                  who.currentLocation.playSound("bubbles");
                  this.minutesUntilReady.Value = 10000;
                  Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Lavender * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
                  {
                    alphaFade = 0.005f
                  });
                }
                return true;
              case -75:
                this.heldObject.Value = new Object(Vector2.Zero, 350, object1.Name + " Juice", false, true, false, false);
                this.heldObject.Value.Price = (int) ((double) object1.Price * 2.25);
                if (!probe)
                {
                  this.heldObject.Value.name = object1.Name + " Juice";
                  this.heldObject.Value.preserve.Value = new Object.PreserveType?(Object.PreserveType.Juice);
                  this.heldObject.Value.preservedParentSheetIndex.Value = (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex;
                  who.currentLocation.playSound("bubbles");
                  who.currentLocation.playSound("Ship");
                  this.minutesUntilReady.Value = 6000;
                  Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.White * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
                  {
                    alphaFade = 0.005f
                  });
                }
                return true;
            }
            break;
        }
      }
      else if (this.name.Equals("Preserves Jar"))
      {
        switch (object1.Category)
        {
          case -79:
            this.heldObject.Value = new Object(Vector2.Zero, 344, object1.Name + " Jelly", false, true, false, false);
            this.heldObject.Value.Price = 50 + object1.Price * 2;
            if (!probe)
            {
              this.minutesUntilReady.Value = 4000;
              this.heldObject.Value.name = object1.Name + " Jelly";
              this.heldObject.Value.preserve.Value = new Object.PreserveType?(Object.PreserveType.Jelly);
              this.heldObject.Value.preservedParentSheetIndex.Value = (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex;
              who.currentLocation.playSound("Ship");
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.LightBlue * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          case -75:
            this.heldObject.Value = new Object(Vector2.Zero, 342, "Pickled " + object1.Name, false, true, false, false);
            this.heldObject.Value.Price = 50 + object1.Price * 2;
            if (!probe)
            {
              this.heldObject.Value.name = "Pickled " + object1.Name;
              this.heldObject.Value.preserve.Value = new Object.PreserveType?(Object.PreserveType.Pickle);
              this.heldObject.Value.preservedParentSheetIndex.Value = (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex;
              who.currentLocation.playSound("Ship");
              this.minutesUntilReady.Value = 4000;
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.White * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          default:
            switch ((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex)
            {
              case 812:
                if ((int) (NetFieldBase<int, NetInt>) object1.preservedParentSheetIndex == 698)
                {
                  this.heldObject.Value = new Object(445, 1);
                  if (!probe)
                  {
                    this.minutesUntilReady.Value = 6000;
                    who.currentLocation.playSound("Ship");
                    Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.LightBlue * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
                    {
                      alphaFade = 0.005f
                    });
                  }
                  return true;
                }
                this.heldObject.Value = !(object1 is ColoredObject coloredObject) ? new Object(447, 1) : (Object) new ColoredObject(447, 1, (Color) (NetFieldBase<Color, NetColor>) coloredObject.color);
                this.heldObject.Value.Price = object1.Price * 2;
                if (!probe)
                {
                  this.minutesUntilReady.Value = 4000;
                  this.heldObject.Value.name = "Aged " + object1.Name;
                  this.heldObject.Value.preserve.Value = new Object.PreserveType?(Object.PreserveType.AgedRoe);
                  this.heldObject.Value.Category = -26;
                  this.heldObject.Value.preservedParentSheetIndex.Value = (int) (NetFieldBase<int, NetInt>) object1.preservedParentSheetIndex;
                  who.currentLocation.playSound("Ship");
                  Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.LightBlue * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
                  {
                    alphaFade = 0.005f
                  });
                }
                return true;
              case 829:
                this.heldObject.Value = new Object(Vector2.Zero, 342, "Pickled " + object1.Name, false, true, false, false);
                this.heldObject.Value.Price = 50 + object1.Price * 2;
                if (!probe)
                {
                  this.heldObject.Value.name = "Pickled " + object1.Name;
                  this.heldObject.Value.preserve.Value = new Object.PreserveType?(Object.PreserveType.Pickle);
                  this.heldObject.Value.preservedParentSheetIndex.Value = (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex;
                  who.currentLocation.playSound("Ship");
                  this.minutesUntilReady.Value = 4000;
                  Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.White * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
                  {
                    alphaFade = 0.005f
                  });
                }
                return true;
            }
            break;
        }
      }
      else if (this.name.Equals("Cheese Press"))
      {
        int num = 1;
        switch (object1.ParentSheetIndex)
        {
          case 184:
            NetRef<Object> heldObject1 = this.heldObject;
            Object object2 = new Object(Vector2.Zero, 424, (string) null, false, true, false, false);
            object2.Stack = num;
            heldObject1.Value = object2;
            if (!probe)
            {
              this.minutesUntilReady.Value = 200;
              who.currentLocation.playSound("Ship");
            }
            return true;
          case 186:
            NetRef<Object> heldObject2 = this.heldObject;
            Object object3 = new Object(Vector2.Zero, 424, "Cheese (=)", false, true, false, false);
            object3.Quality = 2;
            object3.Stack = num;
            heldObject2.Value = object3;
            if (!probe)
            {
              this.minutesUntilReady.Value = 200;
              who.currentLocation.playSound("Ship");
            }
            return true;
          case 436:
            NetRef<Object> heldObject3 = this.heldObject;
            Object object4 = new Object(Vector2.Zero, 426, (string) null, false, true, false, false);
            object4.Stack = num;
            heldObject3.Value = object4;
            if (!probe)
            {
              this.minutesUntilReady.Value = 200;
              who.currentLocation.playSound("Ship");
            }
            return true;
          case 438:
            NetRef<Object> heldObject4 = this.heldObject;
            Object object5 = new Object(Vector2.Zero, 426, (string) null, false, true, false, false);
            object5.Quality = 2;
            object5.Stack = num;
            heldObject4.Value = object5;
            if (!probe)
            {
              this.minutesUntilReady.Value = 200;
              who.currentLocation.playSound("Ship");
            }
            return true;
        }
      }
      else if (this.name.Equals("Mayonnaise Machine"))
      {
        switch (object1.ParentSheetIndex)
        {
          case 107:
            this.heldObject.Value = new Object(Vector2.Zero, 807, (string) null, false, true, false, false);
            if (!probe)
            {
              this.minutesUntilReady.Value = 180;
              who.currentLocation.playSound("Ship");
            }
            return true;
          case 174:
          case 182:
            this.heldObject.Value = new Object(Vector2.Zero, 306, (string) null, false, true, false, false)
            {
              Quality = 2
            };
            if (!probe)
            {
              this.minutesUntilReady.Value = 180;
              who.currentLocation.playSound("Ship");
            }
            return true;
          case 176:
          case 180:
            this.heldObject.Value = new Object(Vector2.Zero, 306, (string) null, false, true, false, false);
            if (!probe)
            {
              this.minutesUntilReady.Value = 180;
              who.currentLocation.playSound("Ship");
            }
            return true;
          case 289:
            this.heldObject.Value = new Object(Vector2.Zero, 306, (string) null, false, true, false, false);
            if (!probe)
            {
              this.minutesUntilReady.Value = 180;
              who.currentLocation.playSound("Ship");
              this.heldObject.Value.Stack = 10;
              this.heldObject.Value.Quality = object1.Quality;
            }
            return true;
          case 305:
            this.heldObject.Value = new Object(Vector2.Zero, 308, (string) null, false, true, false, false);
            if (!probe)
            {
              this.minutesUntilReady.Value = 180;
              who.currentLocation.playSound("Ship");
            }
            return true;
          case 442:
            this.heldObject.Value = new Object(Vector2.Zero, 307, (string) null, false, true, false, false);
            if (!probe)
            {
              this.minutesUntilReady.Value = 180;
              who.currentLocation.playSound("Ship");
            }
            return true;
          case 928:
            this.heldObject.Value = new Object(Vector2.Zero, 306, (string) null, false, true, false, false)
            {
              Quality = 2
            };
            if (!probe)
            {
              this.minutesUntilReady.Value = 180;
              this.heldObject.Value.Stack = 3;
              who.currentLocation.playSound("Ship");
            }
            return true;
        }
      }
      else if (this.name.Equals("Loom"))
      {
        float num1 = (int) (NetFieldBase<int, NetInt>) object1.quality == 0 ? 0.0f : ((int) (NetFieldBase<int, NetInt>) object1.quality == 2 ? 0.25f : ((int) (NetFieldBase<int, NetInt>) object1.quality == 4 ? 0.5f : 0.1f));
        int num2 = Game1.random.NextDouble() <= (double) num1 ? 2 : 1;
        if (object1.ParentSheetIndex == 440)
        {
          NetRef<Object> heldObject = this.heldObject;
          Object object6 = new Object(Vector2.Zero, 428, (string) null, false, true, false, false);
          object6.Stack = num2;
          heldObject.Value = object6;
          if (!probe)
          {
            this.minutesUntilReady.Value = 240;
            who.currentLocation.playSound("Ship");
          }
          return true;
        }
      }
      else if (this.name.Equals("Oil Maker"))
      {
        switch (object1.ParentSheetIndex)
        {
          case 270:
            this.heldObject.Value = new Object(Vector2.Zero, 247, (string) null, false, true, false, false);
            if (!probe)
            {
              this.minutesUntilReady.Value = 1000;
              who.currentLocation.playSound("bubbles");
              who.currentLocation.playSound("sipTea");
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          case 421:
            this.heldObject.Value = new Object(Vector2.Zero, 247, (string) null, false, true, false, false);
            if (!probe)
            {
              this.minutesUntilReady.Value = 60;
              who.currentLocation.playSound("bubbles");
              who.currentLocation.playSound("sipTea");
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          case 430:
            this.heldObject.Value = new Object(Vector2.Zero, 432, (string) null, false, true, false, false);
            if (!probe)
            {
              this.minutesUntilReady.Value = 360;
              who.currentLocation.playSound("bubbles");
              who.currentLocation.playSound("sipTea");
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
          case 431:
            this.heldObject.Value = new Object(247, 1);
            if (!probe)
            {
              this.minutesUntilReady.Value = 3200;
              who.currentLocation.playSound("bubbles");
              who.currentLocation.playSound("sipTea");
              Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            return true;
        }
      }
      else if (this.name.Equals("Seed Maker"))
      {
        if (object1 != null && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 433 || object1 != null && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 771)
          return false;
        Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
        bool flag = false;
        int parentSheetIndex = -1;
        foreach (KeyValuePair<int, string> keyValuePair in dictionary)
        {
          if (Convert.ToInt32(keyValuePair.Value.Split('/')[3]) == object1.ParentSheetIndex)
          {
            flag = true;
            parentSheetIndex = keyValuePair.Key;
            break;
          }
        }
        if (flag)
        {
          Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + (int) this.tileLocation.X + (int) this.tileLocation.Y * 77 + Game1.timeOfDay);
          this.heldObject.Value = new Object(parentSheetIndex, random.Next(1, 4));
          if (!probe)
          {
            if (random.NextDouble() < 0.005)
              this.heldObject.Value = new Object(499, 1);
            else if (random.NextDouble() < 0.02)
              this.heldObject.Value = new Object(770, random.Next(1, 5));
            this.minutesUntilReady.Value = 20;
            who.currentLocation.playSound("Ship");
            DelayedAction.playSoundAfterDelay("dirtyHit", 250);
          }
          return true;
        }
      }
      else if (this.name.Equals("Crystalarium"))
      {
        if ((object1.Category == -2 || object1.Category == -12) && object1.ParentSheetIndex != 74 && (this.heldObject.Value == null || this.heldObject.Value.ParentSheetIndex != object1.ParentSheetIndex) && (this.heldObject.Value == null || (int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady > 0))
        {
          this.heldObject.Value = (Object) object1.getOne();
          if (!probe)
          {
            who.currentLocation.playSound("select");
            this.minutesUntilReady.Value = this.getMinutesForCrystalarium(object1.ParentSheetIndex);
          }
          return true;
        }
      }
      else if (this.name.Equals("Recycling Machine"))
      {
        if (object1.ParentSheetIndex >= 168 && object1.ParentSheetIndex <= 172 && this.heldObject.Value == null)
        {
          Random random = new Random((int) Game1.uniqueIDForThisGame / 2 + (int) Game1.stats.DaysPlayed + Game1.timeOfDay + (int) this.tileLocation.X * 200 + (int) this.tileLocation.Y);
          switch (object1.ParentSheetIndex)
          {
            case 168:
              this.heldObject.Value = new Object(random.NextDouble() < 0.3 ? 382 : (random.NextDouble() < 0.3 ? 380 : 390), random.Next(1, 4));
              break;
            case 169:
              this.heldObject.Value = new Object(random.NextDouble() < 0.25 ? 382 : 388, random.Next(1, 4));
              break;
            case 170:
              this.heldObject.Value = new Object(338, 1);
              break;
            case 171:
              this.heldObject.Value = new Object(338, 1);
              break;
            case 172:
              this.heldObject.Value = random.NextDouble() < 0.1 ? new Object(428, 1) : (Object) new Torch(Vector2.Zero, 3);
              break;
          }
          if (!probe)
          {
            who.currentLocation.playSound("trashcan");
            this.minutesUntilReady.Value = 60;
            ++Game1.stats.PiecesOfTrashRecycled;
          }
          return true;
        }
      }
      else if (this.name.Equals("Furnace"))
      {
        if (who.IsLocalPlayer && this.GetTallyOfObject(who, 382, false) <= 0)
        {
          if (!probe && who.IsLocalPlayer && Object.autoLoadChest == null)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12772"));
          return false;
        }
        if (this.heldObject.Value == null)
        {
          if ((int) (NetFieldBase<int, NetInt>) object1.stack < 5 && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex != 80 && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex != 82 && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex != 330 && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex != 458)
          {
            if (!probe && who.IsLocalPlayer && Object.autoLoadChest == null)
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12777"));
            return false;
          }
          int num = 5;
          switch (object1.ParentSheetIndex)
          {
            case 80:
              this.heldObject.Value = new Object(Vector2.Zero, 338, "Refined Quartz", false, true, false, false);
              if (!probe)
              {
                this.minutesUntilReady.Value = 90;
                num = 1;
                break;
              }
              break;
            case 82:
              this.heldObject.Value = new Object(338, 3);
              if (!probe)
              {
                this.minutesUntilReady.Value = 90;
                num = 1;
                break;
              }
              break;
            case 378:
              this.heldObject.Value = new Object(Vector2.Zero, 334, 1);
              if (!probe)
              {
                this.minutesUntilReady.Value = 30;
                break;
              }
              break;
            case 380:
              this.heldObject.Value = new Object(Vector2.Zero, 335, 1);
              if (!probe)
              {
                this.minutesUntilReady.Value = 120;
                break;
              }
              break;
            case 384:
              this.heldObject.Value = new Object(Vector2.Zero, 336, 1);
              if (!probe)
              {
                this.minutesUntilReady.Value = 300;
                break;
              }
              break;
            case 386:
              this.heldObject.Value = new Object(Vector2.Zero, 337, 1);
              if (!probe)
              {
                this.minutesUntilReady.Value = 480;
                break;
              }
              break;
            case 458:
              this.heldObject.Value = new Object(277, 1);
              if (!probe)
              {
                this.minutesUntilReady.Value = 10;
                num = 1;
                break;
              }
              break;
            case 909:
              this.heldObject.Value = new Object(910, 1);
              if (!probe)
              {
                this.minutesUntilReady.Value = 560;
                break;
              }
              break;
            default:
              return false;
          }
          if (probe)
            return true;
          who.currentLocation.playSound("furnace");
          this.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
          this.showNextIndex.Value = true;
          Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(30, this.tileLocation.Value * 64f + new Vector2(0.0f, -16f), Color.White, 4, animationInterval: 50f, numberOfLoops: 10, sourceRectWidth: 64, layerDepth: ((float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05)))
          {
            alphaFade = 0.005f
          });
          this.ConsumeInventoryItem(who, 382, 1);
          object1.Stack -= num;
          return object1.Stack <= 0;
        }
        if (probe)
          return true;
      }
      else if (this.name.Equals("Geode Crusher"))
      {
        if (who.IsLocalPlayer && this.GetTallyOfObject(who, 382, false) <= 0)
        {
          if (!probe && who.IsLocalPlayer && Object.autoLoadChest == null)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12772"));
          return false;
        }
        if (this.heldObject.Value == null)
        {
          if (!Utility.IsGeode((Item) object1, true))
            return false;
          Object treasureFromGeode = (Object) Utility.getTreasureFromGeode((Item) object1);
          if (treasureFromGeode == null)
            return false;
          this.heldObject.Value = treasureFromGeode;
          if (!probe)
          {
            ++Game1.stats.GeodesCracked;
            this.minutesUntilReady.Value = 60;
          }
          if (probe)
            return true;
          this.showNextIndex.Value = true;
          Utility.addSmokePuff(who.currentLocation, this.tileLocation.Value * 64f + new Vector2(4f, -48f), 200);
          Utility.addSmokePuff(who.currentLocation, this.tileLocation.Value * 64f + new Vector2(-16f, -56f), 300);
          Utility.addSmokePuff(who.currentLocation, this.tileLocation.Value * 64f + new Vector2(16f, -52f), 400);
          Utility.addSmokePuff(who.currentLocation, this.tileLocation.Value * 64f + new Vector2(32f, -56f), 200);
          Utility.addSmokePuff(who.currentLocation, this.tileLocation.Value * 64f + new Vector2(40f, -44f), 500);
          Game1.playSound("drumkit4");
          Game1.playSound("stoneCrack");
          DelayedAction.playSoundAfterDelay("steam", 200);
          this.ConsumeInventoryItem(who, 382, 1);
          --object1.Stack;
          if (object1.Stack <= 0)
            return true;
        }
        else if (probe)
          return true;
      }
      else if (this.name.Equals("Charcoal Kiln"))
      {
        if (who.IsLocalPlayer && ((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex != 388 || object1.Stack < 10))
        {
          if (!probe && who.IsLocalPlayer && Object.autoLoadChest == null)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12783"));
          return false;
        }
        if (this.heldObject.Value == null && !probe && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 388 && object1.Stack >= 10)
        {
          this.ConsumeInventoryItem(who, (Item) object1, 10);
          who.currentLocation.playSound("openBox");
          DelayedAction.playSoundAfterDelay("fireball", 50);
          this.showNextIndex.Value = true;
          Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(27, this.tileLocation.Value * 64f + new Vector2(-16f, (float) sbyte.MinValue), Color.White, 4, animationInterval: 50f, numberOfLoops: 10, sourceRectWidth: 64, layerDepth: ((float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05)))
          {
            alphaFade = 0.005f
          });
          this.heldObject.Value = new Object(382, 1);
          this.minutesUntilReady.Value = 30;
        }
        else if (this.heldObject.Value == null & probe && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 388 && object1.Stack >= 10)
        {
          this.heldObject.Value = new Object();
          return true;
        }
      }
      else if (this.name.Equals("Slime Egg-Press"))
      {
        if (who.IsLocalPlayer && ((int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex != 766 || object1.Stack < 100))
        {
          if (!probe && who.IsLocalPlayer && Object.autoLoadChest == null)
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12787"));
          return false;
        }
        if (this.heldObject.Value == null && !probe && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 766 && object1.Stack >= 100)
        {
          this.ConsumeInventoryItem(who, (Item) object1, 100);
          who.currentLocation.playSound("slimeHit");
          DelayedAction.playSoundAfterDelay("bubbles", 50);
          Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, -160f), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Lime, 1f, 0.0f, 0.0f, 0.0f)
          {
            alphaFade = 0.005f
          });
          int parentSheetIndex = 680;
          if (Game1.random.NextDouble() < 0.05)
            parentSheetIndex = 439;
          else if (Game1.random.NextDouble() < 0.1)
            parentSheetIndex = 437;
          else if (Game1.random.NextDouble() < 0.25)
            parentSheetIndex = 413;
          this.heldObject.Value = new Object(parentSheetIndex, 1);
          this.minutesUntilReady.Value = 1200;
        }
        else if (this.heldObject.Value == null & probe && (int) (NetFieldBase<int, NetInt>) object1.parentSheetIndex == 766 && object1.Stack >= 100)
        {
          this.heldObject.Value = new Object();
          return true;
        }
      }
      else if (this.name.Contains("Feed Hopper") && object1.ParentSheetIndex == 178)
      {
        if (!probe)
        {
          if (Utility.numSilos() <= 0)
          {
            if (Object.autoLoadChest == null)
              Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:NeedSilo"));
            return false;
          }
          who.currentLocation.playSound("Ship");
          DelayedAction.playSoundAfterDelay("grassyStep", 100);
          if (object1.Stack == 0)
            object1.Stack = 1;
          int piecesOfHay1 = (int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay;
          int addHay = (Game1.getLocationFromName("Farm") as Farm).tryToAddHay(object1.Stack);
          int piecesOfHay2 = (int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay;
          if (piecesOfHay1 <= 0 && piecesOfHay2 > 0)
            this.showNextIndex.Value = true;
          else if (piecesOfHay2 <= 0)
            this.showNextIndex.Value = false;
          object1.Stack = addHay;
          if (addHay <= 0)
            return true;
        }
        else
        {
          this.heldObject.Value = new Object();
          return true;
        }
      }
      if (this.name.Contains("Table") && this.heldObject.Value == null && !(bool) (NetFieldBase<bool, NetBool>) object1.bigCraftable && !object1.Name.Contains("Table"))
      {
        this.heldObject.Value = (Object) object1.getOne();
        if (!probe)
          who.currentLocation.playSound("woodyStep");
        return true;
      }
      Object object7 = this.heldObject.Value;
      return false;
    }

    public virtual void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      Object @object = this.heldObject.Get();
      if ((bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest && @object == null)
        this.readyForHarvest.Value = false;
      LightSource lightSource1 = this.netLightSource.Get();
      if (lightSource1 != null && (bool) (NetFieldBase<bool, NetBool>) this.isOn && !environment.hasLightSource(lightSource1.Identifier))
        environment.sharedLights[(int) (NetFieldBase<int, NetInt>) lightSource1.identifier] = lightSource1.Clone();
      if (@object != null)
      {
        if (@object.ParentSheetIndex == 913 && this.IsSprinkler() && @object.heldObject.Value is Chest)
        {
          Chest chest = @object.heldObject.Value as Chest;
          chest.mutex.Update(environment);
          if (Game1.activeClickableMenu == null && chest.GetMutex().IsLockHeld())
            chest.GetMutex().ReleaseLock();
        }
        LightSource lightSource2 = @object.netLightSource.Get();
        if (lightSource2 != null && !environment.hasLightSource(lightSource2.Identifier))
          environment.sharedLights[(int) (NetFieldBase<int, NetInt>) lightSource2.identifier] = lightSource2.Clone();
      }
      if (this.shakeTimer > 0)
      {
        this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
        if (this.shakeTimer <= 0)
          this.health = 10;
      }
      if (this.parentSheetIndex.Get() == 590 && Game1.random.NextDouble() < 0.01)
        this.shakeTimer = 100;
      if (!this.bigCraftable.Get() || !this.name.Equals("Slime Ball", StringComparison.Ordinal))
        return;
      this.ParentSheetIndex = 56 + (int) (time.TotalGameTime.TotalMilliseconds % 600.0 / 100.0);
    }

    public virtual void actionOnPlayerEntry()
    {
      this.isTemporarilyInvisible = false;
      this.health = 10;
      if (this.name == null || !this.name.Contains("Feed Hopper"))
        return;
      this.showNextIndex.Value = (int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay > 0;
    }

    public override bool canBeTrashed() => !(bool) (NetFieldBase<bool, NetBool>) this.questItem && ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 460) && !Utility.IsNormalObjectAtParentSheetIndex((Item) this, 911) && base.canBeTrashed();

    public virtual bool isForage(GameLocation location) => this.Category == -79 || this.Category == -81 || this.Category == -80 || this.Category == -75 || location is Beach || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 430 || this.Category == -23;

    public virtual void initializeLightSource(Vector2 tileLocation, bool mineShaft = false)
    {
      if (this.name == null)
        return;
      int identifier = (int) ((double) tileLocation.X * 2000.0 + (double) tileLocation.Y);
      if (this is Furniture && (int) (NetFieldBase<int, NetInt>) (this as Furniture).furniture_type == 14 && (bool) (NetFieldBase<bool, NetBool>) (this as Furniture).isOn)
        this.lightSource = new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 - 64.0)), 2.5f, new Color(0, 80, 160), identifier);
      else if (this is Furniture && (int) (NetFieldBase<int, NetInt>) (this as Furniture).furniture_type == 16 && (bool) (NetFieldBase<bool, NetBool>) (this as Furniture).isOn)
        this.lightSource = new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 - 64.0)), 1.5f, new Color(0, 80, 160), identifier);
      else if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        if (this is Torch && (bool) (NetFieldBase<bool, NetBool>) this.isOn)
        {
          float num = -64f;
          if (this.Name.Contains("Campfire"))
            num = 32f;
          this.lightSource = new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), tileLocation.Y * 64f + num), 2.5f, new Color(0, 80, 160), identifier);
        }
        else if ((bool) (NetFieldBase<bool, NetBool>) this.isLamp)
          this.lightSource = new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 - 64.0)), 3f, new Color(0, 40, 80), identifier);
        else if (this.name.Equals("Furnace") && (int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady > 0 || this.name.Equals("Bonfire"))
        {
          this.lightSource = new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), tileLocation.Y * 64f), 1.5f, Color.DarkCyan, identifier);
        }
        else
        {
          if (!this.name.Equals("Strange Capsule"))
            return;
          this.lightSource = new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), tileLocation.Y * 64f), 1f, Color.HotPink * 0.75f, identifier);
        }
      }
      else
      {
        if (!Utility.IsNormalObjectAtParentSheetIndex((Item) this, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex) && !(this is Torch))
          return;
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 93 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 94 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 95)
        {
          Color color = Color.White;
          switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
          {
            case 93:
              color = new Color(1, 1, 1) * 0.9f;
              break;
            case 94:
              color = Color.Yellow;
              break;
            case 95:
              color = new Color(70, 0, 150) * 0.9f;
              break;
          }
          this.lightSource = new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 16.0), (float) ((double) tileLocation.Y * 64.0 + 16.0)), mineShaft ? 1.5f : 1.25f, color, identifier);
        }
        else
        {
          if (!Utility.IsNormalObjectAtParentSheetIndex((Item) this, 746))
            return;
          this.lightSource = new LightSource(4, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 48.0)), 0.5f, new Color(1, 1, 1) * 0.65f, identifier);
        }
      }
    }

    public virtual void performRemoveAction(Vector2 tileLocation, GameLocation environment)
    {
      if (this.lightSource != null)
      {
        environment.removeLightSource((int) (NetFieldBase<int, NetInt>) this.lightSource.identifier);
        environment.removeLightSource((int) Game1.player.UniqueMultiplayerID);
      }
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        if ((this.ParentSheetIndex == 105 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 264) && environment != null && environment.terrainFeatures != null && environment.terrainFeatures.ContainsKey(tileLocation) && environment.terrainFeatures[tileLocation] is Tree)
          (environment.terrainFeatures[tileLocation] as Tree).tapped.Value = false;
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 126 && (int) (NetFieldBase<int, NetInt>) this.quality != 0)
          Game1.createItemDebris((Item) new Hat((int) (NetFieldBase<int, NetInt>) this.quality - 1), tileLocation * 64f, (Game1.player.FacingDirection + 2) % 4);
        this.quality.Value = 0;
      }
      if (this.name != null && this.name.Contains("Sprinkler"))
        environment.removeTemporarySpritesWithID((int) tileLocation.X * 4000 + (int) tileLocation.Y);
      if (!this.name.Contains("Seasonal") || !this.bigCraftable.Value)
        return;
      this.ParentSheetIndex -= this.ParentSheetIndex % 4;
    }

    public virtual void dropItem(GameLocation location, Vector2 origin, Vector2 destination)
    {
      if (!this.type.Equals((object) "Crafting") && !this.Type.Equals("interactive") || (int) (NetFieldBase<int, NetInt>) this.fragility == 2)
        return;
      location.debris.Add(new Debris((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable ? -this.ParentSheetIndex : this.ParentSheetIndex, origin, destination));
    }

    public virtual bool isPassable()
    {
      if (this.isTemporarilyInvisible)
        return true;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
        return false;
      if (Utility.IsNormalObjectAtParentSheetIndex((Item) this, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex))
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
        {
          case 93:
          case 286:
          case 287:
          case 288:
          case 293:
          case 297:
          case 328:
          case 329:
          case 331:
          case 333:
          case 401:
          case 405:
          case 407:
          case 409:
          case 411:
          case 415:
          case 590:
          case 840:
          case 841:
            return true;
        }
      }
      if (this.Category != -74 && this.Category != -19 || this.isSapling())
        return false;
      switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
      {
        case 301:
        case 302:
        case 473:
          return false;
        default:
          return true;
      }
    }

    public virtual void reloadSprite() => this.initializeLightSource((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);

    public virtual void consumeRecipe(Farmer who)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isRecipe)
        return;
      if (this.Category == -7)
        who.cookingRecipes.Add(this.name, 0);
      else
        who.craftingRecipes.Add(this.name, 0);
    }

    public virtual Microsoft.Xna.Framework.Rectangle getBoundingBox(Vector2 tileLocation)
    {
      Microsoft.Xna.Framework.Rectangle newValue = this.boundingBox.Value;
      Microsoft.Xna.Framework.Rectangle boundingBox = newValue;
      if (this is Torch && !(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 590)
      {
        boundingBox.X = (int) tileLocation.X * 64 + 24;
        boundingBox.Y = (int) tileLocation.Y * 64 + 24;
      }
      else
      {
        boundingBox.X = (int) tileLocation.X * 64;
        boundingBox.Y = (int) tileLocation.Y * 64;
      }
      if (boundingBox != newValue)
        this.boundingBox.Set(newValue);
      return boundingBox;
    }

    public override bool canBeGivenAsGift() => !Utility.IsNormalObjectAtParentSheetIndex((Item) this, 911) && !(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && !(this is Furniture) && !(this is Wallpaper);

    public virtual bool performDropDownAction(Farmer who)
    {
      if (who == null)
        who = Game1.getFarmer((long) this.owner);
      if (this.name.Equals("Worm Bin"))
      {
        if (this.heldObject.Value == null)
        {
          this.heldObject.Value = new Object(685, Game1.random.Next(2, 6));
          this.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
        }
        return false;
      }
      if (this.name.Equals("Bee House"))
      {
        if (this.heldObject.Value == null)
        {
          this.heldObject.Value = new Object(Vector2.Zero, 340, (string) null, false, true, false, false);
          this.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
        }
        return false;
      }
      if (this.name.Equals("Solar Panel"))
      {
        if (this.heldObject.Value == null)
        {
          this.heldObject.Value = new Object(Vector2.Zero, 787, (string) null, false, true, false, false);
          this.minutesUntilReady.Value = 16800;
        }
        return false;
      }
      if (this.name.Contains("Strange Capsule"))
        this.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 3);
      else if (this.name.Contains("Feed Hopper"))
      {
        this.showNextIndex.Value = false;
        if ((int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay >= 0)
          this.showNextIndex.Value = true;
      }
      return false;
    }

    private void totemWarp(Farmer who)
    {
      for (int index = 0; index < 12; ++index)
        Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.Position.X - 256, (int) who.Position.X + 192), (float) Game1.random.Next((int) who.Position.Y - 256, (int) who.Position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
      who.currentLocation.playSound("wand");
      Game1.displayFarmer = false;
      Game1.player.temporarilyInvincible = true;
      Game1.player.temporaryInvincibilityTimer = -2000;
      Game1.player.freezePause = 1000;
      Game1.flashAlpha = 1f;
      DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(this.totemWarpForReal), 1000);
      new Microsoft.Xna.Framework.Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
      int num = 0;
      for (int x = who.getTileX() + 8; x >= who.getTileX() - 8; --x)
      {
        Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) who.getTileY()) * 64f, Color.White, animationInterval: 50f)
        {
          layerDepth = 1f,
          delayBeforeAnimationStart = num * 25,
          motion = new Vector2(-0.25f, 0.0f)
        });
        ++num;
      }
    }

    private void totemWarpForReal()
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
      {
        case 261:
          Game1.warpFarmer("Desert", 35, 43, false);
          break;
        case 688:
          int default_x = 48;
          int default_y = 7;
          switch (Game1.whichFarm)
          {
            case 5:
              default_x = 48;
              default_y = 39;
              break;
            case 6:
              default_x = 82;
              default_y = 29;
              break;
          }
          Point propertyPosition = Game1.getFarm().GetMapPropertyPosition("WarpTotemEntry", default_x, default_y);
          Game1.warpFarmer("Farm", propertyPosition.X, propertyPosition.Y, false);
          break;
        case 689:
          Game1.warpFarmer("Mountain", 31, 20, false);
          break;
        case 690:
          Game1.warpFarmer("Beach", 20, 4, false);
          break;
        case 886:
          Game1.warpFarmer("IslandSouth", 11, 11, false);
          break;
      }
      Game1.fadeToBlackAlpha = 0.99f;
      Game1.screenGlow = false;
      Game1.player.temporarilyInvincible = false;
      Game1.player.temporaryInvincibilityTimer = 0;
      Game1.displayFarmer = true;
    }

    public void MonsterMusk(Farmer who)
    {
      who.FarmerSprite.PauseForSingleAnimation = false;
      who.FarmerSprite.StopAnimation();
      who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[4]
      {
        new FarmerSprite.AnimationFrame(104, 350, false, false),
        new FarmerSprite.AnimationFrame(105, 350, false, false),
        new FarmerSprite.AnimationFrame(104, 350, false, false),
        new FarmerSprite.AnimationFrame(105, 350, false, false)
      });
      who.currentLocation.playSound("croak");
      Game1.buffsDisplay.addOtherBuff(new Buff(24));
    }

    public override string[] ModifyItemBuffs(string[] buffs)
    {
      if (buffs != null && this.Category == -7)
      {
        int num = 0;
        if (this.Quality != 0)
          num = 1;
        if (num > 0)
        {
          int result = 0;
          for (int index = 0; index < buffs.Length; ++index)
          {
            if (index != 9 && buffs[index] != "0" && int.TryParse(buffs[index], out result))
            {
              result += num;
              buffs[index] = result.ToString();
            }
          }
        }
      }
      return base.ModifyItemBuffs(buffs);
    }

    private void rainTotem(Farmer who)
    {
      GameLocation.LocationContext locationContext = Game1.currentLocation.GetLocationContext();
      if (locationContext == GameLocation.LocationContext.Default)
      {
        if (!Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
        {
          Game1.netWorldState.Value.WeatherForTomorrow = Game1.weatherForTomorrow = 1;
          Game1.pauseThenMessage(2000, Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"), false);
        }
      }
      else
      {
        Game1.netWorldState.Value.GetWeatherForLocation(locationContext).weatherForTomorrow.Value = 1;
        Game1.pauseThenMessage(2000, Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822"), false);
      }
      Game1.screenGlow = false;
      who.currentLocation.playSound("thunder");
      who.canMove = false;
      Game1.screenGlowOnce(Color.SlateBlue, false);
      Game1.player.faceDirection(2);
      Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
      {
        new FarmerSprite.AnimationFrame(57, 2000, false, false, new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), true)
      });
      for (int index = 0; index < 6; ++index)
      {
        Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, 1f, 0.01f, Color.White * 0.8f, 2f, 0.01f, 0.0f, 0.0f)
        {
          motion = new Vector2((float) Game1.random.Next(-10, 11) / 10f, -2f),
          delayBeforeAnimationStart = index * 200
        });
        Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, 1f, 0.01f, Color.White * 0.8f, 1f, 0.01f, 0.0f, 0.0f)
        {
          motion = new Vector2((float) Game1.random.Next(-30, -10) / 10f, -1f),
          delayBeforeAnimationStart = 100 + index * 200
        });
        Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33), 9999f, 1, 999, who.Position + new Vector2(0.0f, (float) sbyte.MinValue), false, false, 1f, 0.01f, Color.White * 0.8f, 1f, 0.01f, 0.0f, 0.0f)
        {
          motion = new Vector2((float) Game1.random.Next(10, 30) / 10f, -1f),
          delayBeforeAnimationStart = 200 + index * 200
        });
      }
      Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 9999f, 1, 999, Game1.player.Position + new Vector2(0.0f, -96f), false, false, false, 0.0f)
      {
        motion = new Vector2(0.0f, -7f),
        acceleration = new Vector2(0.0f, 0.1f),
        scaleChange = 0.015f,
        alpha = 1f,
        alphaFade = 0.0075f,
        shakeIntensity = 1f,
        initialPosition = Game1.player.Position + new Vector2(0.0f, -96f),
        xPeriodic = true,
        xPeriodicLoopTime = 1000f,
        xPeriodicRange = 4f,
        layerDepth = 1f
      });
      DelayedAction.playSoundAfterDelay("rainsound", 2000);
    }

    public virtual bool performUseAction(GameLocation location)
    {
      if (!Game1.player.canMove || this.isTemporarilyInvisible)
        return false;
      bool flag = !Game1.eventUp && !Game1.isFestival() && !Game1.fadeToBlack && !(bool) (NetFieldBase<bool, NetBool>) Game1.player.swimming && !(bool) (NetFieldBase<bool, NetBool>) Game1.player.bathingClothes && !Game1.player.onBridge.Value;
      if (this.name != null && this.name.Contains("Totem"))
      {
        if (flag)
        {
          switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
          {
            case 261:
            case 688:
            case 689:
            case 690:
            case 886:
              Game1.player.jitterStrength = 1f;
              Color glowColor = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 681 ? Color.SlateBlue : ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 688 ? Color.LimeGreen : ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 689 ? Color.OrangeRed : ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 261 ? new Color((int) byte.MaxValue, 200, 0) : Color.LightBlue)));
              location.playSound("warrior");
              Game1.player.faceDirection(2);
              Game1.player.CanMove = false;
              Game1.player.temporarilyInvincible = true;
              Game1.player.temporaryInvincibilityTimer = -4000;
              Game1.changeMusicTrack("none");
              if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 681)
                Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
                {
                  new FarmerSprite.AnimationFrame(57, 2000, false, false),
                  new FarmerSprite.AnimationFrame((int) (short) Game1.player.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.rainTotem), true)
                });
              else
                Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
                {
                  new FarmerSprite.AnimationFrame(57, 2000, false, false),
                  new FarmerSprite.AnimationFrame((int) (short) Game1.player.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.totemWarp), true)
                });
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 9999f, 1, 999, Game1.player.Position + new Vector2(0.0f, -96f), false, false, false, 0.0f)
              {
                motion = new Vector2(0.0f, -1f),
                scaleChange = 0.01f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                initialPosition = Game1.player.Position + new Vector2(0.0f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 1f
              });
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 9999f, 1, 999, Game1.player.Position + new Vector2(-64f, -96f), false, false, false, 0.0f)
              {
                motion = new Vector2(0.0f, -0.5f),
                scaleChange = 0.005f,
                scale = 0.5f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                delayBeforeAnimationStart = 10,
                initialPosition = Game1.player.Position + new Vector2(-64f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 0.9999f
              });
              Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 9999f, 1, 999, Game1.player.Position + new Vector2(64f, -96f), false, false, false, 0.0f)
              {
                motion = new Vector2(0.0f, -0.5f),
                scaleChange = 0.005f,
                scale = 0.5f,
                alpha = 1f,
                alphaFade = 0.0075f,
                delayBeforeAnimationStart = 20,
                shakeIntensity = 1f,
                initialPosition = Game1.player.Position + new Vector2(64f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 0.9988f
              });
              Game1.screenGlowOnce(glowColor, false);
              Utility.addSprinklesToLocation(location, Game1.player.getTileX(), Game1.player.getTileY(), 16, 16, 1300, 20, Color.White, motionTowardCenter: true);
              return true;
            case 681:
              this.rainTotem(Game1.player);
              return true;
          }
        }
      }
      else
      {
        if (this.name != null && this.name.Contains("Secret Note"))
        {
          int secretNoteIndex = this.name.Contains('#') ? Convert.ToInt32(this.name.Split('#')[1]) : 1;
          if (!Game1.player.secretNotesSeen.Contains(secretNoteIndex))
          {
            Game1.player.secretNotesSeen.Add(secretNoteIndex);
            if (secretNoteIndex == 23 && !Game1.player.eventsSeen.Contains(2120303))
              Game1.player.addQuest(29);
            else if (secretNoteIndex == 10 && !Game1.player.mailReceived.Contains("qiCave"))
              Game1.player.addQuest(30);
          }
          Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(secretNoteIndex);
          return true;
        }
        if (this.name != null && this.name.Contains("Journal Scrap"))
        {
          int secretNoteIndex = (this.name.Contains('#') ? Convert.ToInt32(this.name.Split('#')[1]) : 1) + GameLocation.JOURNAL_INDEX;
          if (!Game1.player.secretNotesSeen.Contains(secretNoteIndex))
            Game1.player.secretNotesSeen.Add(secretNoteIndex);
          Game1.activeClickableMenu = (IClickableMenu) new LetterViewerMenu(secretNoteIndex);
          return true;
        }
      }
      if (this.ParentSheetIndex == 911)
      {
        if (!flag)
          return false;
        switch (Utility.GetHorseWarpRestrictionsForFarmer(Game1.player).FirstOrDefault<int>())
        {
          case 0:
            Horse horse1 = (Horse) null;
            foreach (Character character in Game1.player.currentLocation.characters)
            {
              if (character is Horse)
              {
                Horse horse2 = character as Horse;
                if (horse2.getOwner() == Game1.player)
                {
                  horse1 = horse2;
                  break;
                }
              }
            }
            if (horse1 == null || Math.Abs(Game1.player.getTileX() - horse1.getTileX()) > 1 || Math.Abs(Game1.player.getTileY() - horse1.getTileY()) > 1)
            {
              Game1.player.faceDirection(2);
              Game1.soundBank.PlayCue("horse_flute");
              Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[6]
              {
                new FarmerSprite.AnimationFrame(98, 400, true, false),
                new FarmerSprite.AnimationFrame(99, 200, true, false),
                new FarmerSprite.AnimationFrame(100, 200, true, false),
                new FarmerSprite.AnimationFrame(99, 200, true, false),
                new FarmerSprite.AnimationFrame(98, 400, true, false),
                new FarmerSprite.AnimationFrame(99, 200, true, false)
              });
              Game1.player.freezePause = 1500;
              DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
              {
                switch (Utility.GetHorseWarpRestrictionsForFarmer(Game1.player).FirstOrDefault<int>())
                {
                  case 0:
                    Game1.player.team.requestHorseWarpEvent.Fire(Game1.player.UniqueMultiplayerID);
                    break;
                  case 1:
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_NoHorse"));
                    break;
                  case 2:
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_InvalidLocation"));
                    break;
                  case 3:
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_NoClearance"));
                    break;
                }
              }), 1500);
            }
            ++this.stack.Value;
            return true;
          case 1:
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_NoHorse"));
            break;
          case 2:
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_InvalidLocation"));
            break;
          case 3:
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_NoClearance"));
            break;
          case 4:
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HorseFlute_InUse"));
            break;
          default:
            ++this.stack.Value;
            return true;
        }
      }
      if (this.ParentSheetIndex == 879)
      {
        if (!flag)
          return false;
        Game1.player.faceDirection(2);
        Game1.player.freezePause = 1750;
        Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
        {
          new FarmerSprite.AnimationFrame(57, 750, false, false),
          new FarmerSprite.AnimationFrame((int) (short) Game1.player.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior(this.MonsterMusk), true)
        });
        for (int index = 0; index < 3; ++index)
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(16f, (float) (32 * index - 64)), Color.Purple)
          {
            motion = new Vector2(Utility.RandomFloat(-1f, 1f), -0.5f),
            scaleChange = 0.005f,
            scale = 0.5f,
            alpha = 1f,
            alphaFade = 0.0075f,
            shakeIntensity = 1f,
            delayBeforeAnimationStart = 100 * index,
            layerDepth = 0.9999f,
            positionFollowsAttachedCharacter = true,
            attachedCharacter = (Character) Game1.player
          });
        location.playSound("steam");
        return true;
      }
      string name = this.name;
      return false;
    }

    public override Color getCategoryColor()
    {
      if (this is Furniture)
        return new Color(100, 25, 190);
      if ((NetFieldBase<string, NetString>) this.type != (NetString) null && this.type.Equals((object) "Arch"))
        return new Color(110, 0, 90);
      switch (this.Category)
      {
        case -81:
          return new Color(10, 130, 50);
        case -80:
          return new Color(219, 54, 211);
        case -79:
          return Color.DeepPink;
        case -75:
          return Color.Green;
        case -74:
          return Color.Brown;
        case -28:
          return new Color(50, 10, 70);
        case -27:
        case -26:
          return new Color(0, 155, 111);
        case -24:
          return Color.Plum;
        case -22:
          return Color.DarkCyan;
        case -21:
          return Color.DarkRed;
        case -20:
          return Color.DarkGray;
        case -19:
          return Color.SlateGray;
        case -18:
        case -14:
        case -6:
        case -5:
          return new Color((int) byte.MaxValue, 0, 100);
        case -16:
        case -15:
          return new Color(64, 102, 114);
        case -12:
        case -2:
          return new Color(110, 0, 90);
        case -8:
          return new Color(148, 61, 40);
        case -7:
          return new Color(220, 60, 0);
        case -4:
          return Color.DarkBlue;
        default:
          return Color.Black;
      }
    }

    public override string getCategoryName()
    {
      if (this is Furniture)
      {
        if ((this as Furniture).placementRestriction == 1)
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Outdoors");
        return (this as Furniture).placementRestriction == 2 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Furniture_Decoration") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12847");
      }
      if (this.type.Value != null && this.type.Value.Equals("Arch"))
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12849");
      switch (this.Category)
      {
        case -81:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12869");
        case -80:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12866");
        case -79:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12854");
        case -75:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12851");
        case -74:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12855");
        case -28:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12867");
        case -27:
        case -26:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12862");
        case -25:
        case -7:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12853");
        case -24:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12859");
        case -22:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12858");
        case -21:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12857");
        case -20:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12860");
        case -19:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12856");
        case -18:
        case -14:
        case -6:
        case -5:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12864");
        case -16:
        case -15:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12868");
        case -12:
        case -2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12850");
        case -8:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12863");
        case -4:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12852");
        default:
          return "";
      }
    }

    public virtual bool isActionable(Farmer who) => !this.isTemporarilyInvisible && this.checkForAction(who, true);

    public int getHealth() => this.health;

    public void setHealth(int health) => this.health = health;

    protected virtual void grabItemFromAutoGrabber(Item item, Farmer who)
    {
      if (this.heldObject.Value == null || !(this.heldObject.Value is Chest))
        return;
      if (who.couldInventoryAcceptThisItem(item))
      {
        (this.heldObject.Value as Chest).items.Remove(item);
        (this.heldObject.Value as Chest).clearNulls();
        Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) (this.heldObject.Value as Chest).items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect((this.heldObject.Value as Chest).grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromAutoGrabber), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, sourceItem: ((Item) this), context: ((object) this));
      }
      if (!(this.heldObject.Value as Chest).isEmpty())
        return;
      this.showNextIndex.Value = false;
    }

    public static bool HighlightFertilizers(Item i) => i is Object && (int) (NetFieldBase<int, NetInt>) (i as Object).category == -19;

    private void AttachToSprinklerAttachment(Item i, Farmer who)
    {
      if (i == null || !(i is Object) || !this.IsSprinkler() || this.heldObject.Value == null)
        return;
      who.removeItemFromInventory(i);
      this.heldObject.Value.heldObject.Value = i as Object;
      if (Game1.player.ActiveObject != null)
        return;
      Game1.player.showNotCarrying();
      Game1.player.Halt();
    }

    public override int healthRecoveredOnConsumption()
    {
      if (this.Edibility < 0)
        return 0;
      return this.ParentSheetIndex == 874 ? (int) ((double) this.staminaRecoveredOnConsumption() * 0.680000007152557) : (int) ((double) this.staminaRecoveredOnConsumption() * 0.449999988079071);
    }

    public override int staminaRecoveredOnConsumption() => (int) Math.Ceiling((double) this.Edibility * 2.5) + this.Quality * this.Edibility;

    public virtual bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (this.isTemporarilyInvisible)
        return true;
      if (!justCheckingForActivity && who != null && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1) && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1) && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY()) && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY()) && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
        this.performToolAction((Tool) null, who.currentLocation);
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        if (justCheckingForActivity)
          return true;
        switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
        {
          case 165:
            if (this.heldObject.Value != null && this.heldObject.Value is Chest && !(this.heldObject.Value as Chest).isEmpty())
            {
              if (justCheckingForActivity)
                return true;
              Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) (this.heldObject.Value as Chest).items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect((this.heldObject.Value as Chest).grabItemFromInventory), (string) null, new ItemGrabMenu.behaviorOnItemSelect(this.grabItemFromAutoGrabber), canBeExitedWithKey: true, showOrganizeButton: true, source: 1, context: ((object) this));
              return true;
            }
            break;
          case 231:
            if (this.readyForHarvest.Value && who.IsLocalPlayer)
            {
              Object @object = this.heldObject.Value;
              this.heldObject.Value = (Object) null;
              if (!who.addItemToInventoryBool((Item) @object))
              {
                this.heldObject.Value = @object;
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                return false;
              }
              this.heldObject.Value = new Object(Vector2.Zero, 787, (string) null, false, true, false, false);
              this.minutesUntilReady.Value = 16800;
              Game1.playSound("coin");
              this.readyForHarvest.Value = false;
              return true;
            }
            break;
          case 238:
            if (justCheckingForActivity)
              return true;
            Vector2 vector2_1 = Vector2.Zero;
            Vector2 vector2_2 = Vector2.Zero;
            foreach (KeyValuePair<Vector2, Object> pair in who.currentLocation.objects.Pairs)
            {
              if ((bool) (NetFieldBase<bool, NetBool>) pair.Value.bigCraftable && pair.Value.ParentSheetIndex == 238)
              {
                if (vector2_1.Equals(Vector2.Zero))
                  vector2_1 = pair.Key;
                else if (vector2_2.Equals(Vector2.Zero))
                {
                  vector2_2 = pair.Key;
                  break;
                }
              }
            }
            if (vector2_2.Equals(Vector2.Zero))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MiniObelisk_NeedsPair"));
              return false;
            }
            Vector2 vector2_3 = (double) Vector2.Distance(who.getTileLocation(), vector2_1) <= (double) Vector2.Distance(who.getTileLocation(), vector2_2) ? vector2_2 : vector2_1;
            foreach (Vector2 vector2_4 in new List<Vector2>()
            {
              new Vector2(vector2_3.X, vector2_3.Y + 1f),
              new Vector2(vector2_3.X - 1f, vector2_3.Y),
              new Vector2(vector2_3.X + 1f, vector2_3.Y),
              new Vector2(vector2_3.X, vector2_3.Y - 1f)
            })
            {
              Vector2 v = vector2_4;
              if (who.currentLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v))
              {
                for (int index = 0; index < 12; ++index)
                  who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, (float) Game1.random.Next(25, 75), 6, 1, new Vector2((float) Game1.random.Next((int) who.Position.X - 256, (int) who.Position.X + 192), (float) Game1.random.Next((int) who.Position.Y - 256, (int) who.Position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
                who.currentLocation.playSound("wand");
                Game1.displayFarmer = false;
                Game1.player.freezePause = 800;
                Game1.flashAlpha = 1f;
                DelayedAction.fadeAfterDelay((Game1.afterFadeFunction) (() =>
                {
                  who.setTileLocation(v);
                  Game1.displayFarmer = true;
                  Game1.globalFadeToClear();
                }), 800);
                new Microsoft.Xna.Framework.Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
                int num = 0;
                for (int x = who.getTileX() + 8; x >= who.getTileX() - 8; --x)
                {
                  who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2((float) x, (float) who.getTileY()) * 64f, Color.White, animationInterval: 50f)
                  {
                    layerDepth = 1f,
                    delayBeforeAnimationStart = num * 25,
                    motion = new Vector2(-0.25f, 0.0f)
                  });
                  ++num;
                }
                return true;
              }
            }
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MiniObelisk_NeedsSpace"));
            return false;
          case 239:
            this.shakeTimer = 500;
            who.currentLocation.localSound("DwarvishSentry");
            who.freezePause = 500;
            DelayedAction.functionAfterDelay((DelayedAction.delayedBehavior) (() =>
            {
              int totalCrops = Game1.getFarm().getTotalCrops();
              int totalOpenHoeDirt = Game1.getFarm().getTotalOpenHoeDirt();
              int cropsReadyForHarvest1 = Game1.getFarm().getTotalCropsReadyForHarvest();
              int totalUnwateredCrops = Game1.getFarm().getTotalUnwateredCrops();
              int cropsReadyForHarvest2 = Game1.getFarm().getTotalGreenhouseCropsReadyForHarvest();
              int totalForageItems = Game1.getFarm().getTotalForageItems();
              int machinesReadyForHarvest = Game1.getFarm().getNumberOfMachinesReadyForHarvest();
              bool flag = Game1.getFarm().doesFarmCaveNeedHarvesting();
              Game1.multipleDialogues(new string[1]
              {
                Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_Intro", (object) Game1.player.farmName.Value) + "^--------------^" + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_PiecesHay", (object) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay, (object) (Utility.numSilos() * 240)) + "  ^" + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_TotalCrops", (object) totalCrops) + "  ^" + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_CropsReadyForHarvest", (object) cropsReadyForHarvest1) + "  ^" + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_CropsUnwatered", (object) totalUnwateredCrops) + "  ^" + (cropsReadyForHarvest2 != -1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_CropsReadyForHarvest_Greenhouse", (object) cropsReadyForHarvest2) + "  ^" : "") + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_TotalOpenHoeDirt", (object) totalOpenHoeDirt) + "  ^" + (Game1.getFarm().SpawnsForage() ? Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_TotalForage", (object) totalForageItems) + "  ^" : "") + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_MachinesReady", (object) machinesReadyForHarvest) + "  ^" + Game1.content.LoadString("Strings\\StringsFromCSFiles:FarmComputer_FarmCave", flag ? (object) Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes") : (object) Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")) + "  "
              });
            }), 500);
            return true;
          case 247:
            Game1.activeClickableMenu = (IClickableMenu) new TailoringMenu();
            return true;
        }
      }
      if (this.name.Equals("Prairie King Arcade System"))
      {
        if (justCheckingForActivity)
          return true;
        Game1.currentLocation.showPrairieKingMenu();
        return true;
      }
      if (this.name.Equals("Junimo Kart Arcade System"))
      {
        if (justCheckingForActivity)
          return true;
        Response[] answerChoices = new Response[3]
        {
          new Response("Progress", Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12873")),
          new Response("Endless", Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12875")),
          new Response("Exit", Game1.content.LoadString("Strings\\StringsFromCSFiles:TitleMenu.cs.11738"))
        };
        who.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_Arcade_Minecart_Menu"), answerChoices, "MinecartGame");
        return true;
      }
      if (this.name.Equals("Staircase"))
      {
        if (who.currentLocation is MineShaft && (who.currentLocation as MineShaft).shouldCreateLadderOnThisLevel())
        {
          if (justCheckingForActivity)
            return true;
          Game1.enterMine(Game1.CurrentMineLevel + 1);
          Game1.playSound("stairsdown");
        }
      }
      else
      {
        if (this.name.Equals("Slime Ball"))
        {
          if (justCheckingForActivity)
            return true;
          who.currentLocation.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
          DelayedAction.playSoundAfterDelay("slimedead", 40);
          DelayedAction.playSoundAfterDelay("slimeHit", 100);
          who.currentLocation.playSound("slimeHit");
          Random random = new Random((int) Game1.stats.daysPlayed + (int) Game1.uniqueIDForThisGame + (int) this.tileLocation.X * 77 + (int) this.tileLocation.Y * 777 + 2);
          Game1.createMultipleObjectDebris(766, (int) this.tileLocation.X, (int) this.tileLocation.Y, random.Next(10, 21), (float) (1.0 + (who.FacingDirection == 2 ? 0.0 : Game1.random.NextDouble())));
          Utility.makeTemporarySpriteJuicier(new TemporaryAnimatedSprite(44, this.tileLocation.Value * 64f, Color.Lime, 10)
          {
            interval = 70f,
            holdLastFrame = true,
            alphaFade = 0.01f
          }, who.currentLocation);
          Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(44, this.tileLocation.Value * 64f + new Vector2(-16f, 0.0f), Color.Lime, 10)
          {
            interval = 70f,
            delayBeforeAnimationStart = 0,
            holdLastFrame = true,
            alphaFade = 0.01f
          });
          Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(44, this.tileLocation.Value * 64f + new Vector2(0.0f, 16f), Color.Lime, 10)
          {
            interval = 70f,
            delayBeforeAnimationStart = 100,
            holdLastFrame = true,
            alphaFade = 0.01f
          });
          Game1.multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(44, this.tileLocation.Value * 64f + new Vector2(16f, 0.0f), Color.Lime, 10)
          {
            interval = 70f,
            delayBeforeAnimationStart = 200,
            holdLastFrame = true,
            alphaFade = 0.01f
          });
          while (random.NextDouble() < 0.33)
            Game1.createObjectDebris(557, (int) this.tileLocation.X, (int) this.tileLocation.Y, who.UniqueMultiplayerID);
          return true;
        }
        if (this.name.Equals("Furnace") && who.ActiveObject == null && !(bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest)
        {
          if (this.heldObject.Value != null)
          {
            int num = justCheckingForActivity ? 1 : 0;
            return true;
          }
        }
        else
        {
          if (this.name.Contains("Table"))
          {
            if (this.heldObject.Value != null)
            {
              if (justCheckingForActivity)
                return true;
              Object @object = this.heldObject.Value;
              this.heldObject.Value = (Object) null;
              if (who.isMoving())
                Game1.haltAfterCheck = false;
              if (who.addItemToInventoryBool((Item) @object))
                Game1.playSound("coin");
              else
                this.heldObject.Value = @object;
              return true;
            }
            if (!this.name.Equals("Tile Table"))
              return false;
            if (justCheckingForActivity)
              return true;
            ++this.ParentSheetIndex;
            if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 322)
              return true;
            this.ParentSheetIndex -= 9;
            return false;
          }
          if (this.name.Contains("Stool"))
          {
            if (justCheckingForActivity)
              return true;
            ++this.ParentSheetIndex;
            if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 305)
              return true;
            this.ParentSheetIndex -= 9;
            return false;
          }
          if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && (this.name.Contains("Chair") || this.name.Contains("Painting") || this.name.Equals("House Plant")))
          {
            if (justCheckingForActivity)
              return true;
            ++this.ParentSheetIndex;
            int num1 = -1;
            int num2 = -1;
            string name = this.name;
            if (!(name == "Red Chair"))
            {
              if (!(name == "Patio Chair"))
              {
                if (!(name == "Dark Chair"))
                {
                  if (!(name == "Wood Chair"))
                  {
                    if (!(name == "House Plant"))
                    {
                      if (name == "Painting")
                      {
                        num1 = 8;
                        num2 = 32;
                      }
                    }
                    else
                    {
                      num1 = 8;
                      num2 = 0;
                    }
                  }
                  else
                  {
                    num1 = 4;
                    num2 = 24;
                  }
                }
                else
                {
                  num1 = 4;
                  num2 = 60;
                }
              }
              else
              {
                num1 = 4;
                num2 = 52;
              }
            }
            else
            {
              num1 = 4;
              num2 = 44;
            }
            if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != num2 + num1)
              return true;
            this.ParentSheetIndex -= num1;
            return false;
          }
          if (this.name.Equals("Flute Block"))
          {
            if (justCheckingForActivity)
              return true;
            this.preservedParentSheetIndex.Value = (this.preservedParentSheetIndex.Value + 100) % 2400;
            this.shakeTimer = 200;
            if (Game1.soundBank != null)
            {
              if (this.internalSound != null)
              {
                this.internalSound.Stop(AudioStopOptions.Immediate);
                this.internalSound = Game1.soundBank.GetCue("flute");
              }
              else
                this.internalSound = Game1.soundBank.GetCue("flute");
              this.internalSound.SetVariable("Pitch", this.preservedParentSheetIndex.Value);
              this.internalSound.Play();
            }
            this.scale.Y = 1.3f;
            this.shakeTimer = 200;
            return true;
          }
          if (this.name.Equals("Drum Block"))
          {
            if (justCheckingForActivity)
              return true;
            this.preservedParentSheetIndex.Value = (this.preservedParentSheetIndex.Value + 1) % 7;
            this.shakeTimer = 200;
            if (Game1.soundBank != null)
            {
              if (this.internalSound != null)
              {
                this.internalSound.Stop(AudioStopOptions.Immediate);
                this.internalSound = Game1.soundBank.GetCue("drumkit" + this.preservedParentSheetIndex.Value.ToString());
              }
              else
                this.internalSound = Game1.soundBank.GetCue("drumkit" + this.preservedParentSheetIndex.Value.ToString());
              this.internalSound.Play();
            }
            this.scale.Y = 1.3f;
            this.shakeTimer = 200;
            return true;
          }
          if (this.IsSprinkler())
          {
            if (this.heldObject.Value != null && this.heldObject.Value.ParentSheetIndex == 913)
            {
              if (justCheckingForActivity)
                return true;
              if (!Game1.didPlayerJustRightClick(true))
                return false;
              if (this.heldObject.Value.heldObject.Value is Chest)
              {
                Chest chest = this.heldObject.Value.heldObject.Value as Chest;
                chest.GetMutex().RequestLock((Action) (() => chest.ShowMenu()));
              }
            }
          }
          else
          {
            if (this.IsScarecrow())
            {
              if (justCheckingForActivity)
                return true;
              if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 126 && who.CurrentItem != null && who.CurrentItem is Hat)
              {
                this.shakeTimer = 100;
                if ((int) (NetFieldBase<int, NetInt>) this.quality != 0)
                  Game1.createItemDebris((Item) new Hat((int) (NetFieldBase<int, NetInt>) this.quality - 1), this.tileLocation.Value * 64f, (who.FacingDirection + 2) % 4);
                this.quality.Value = (int) (NetFieldBase<int, NetInt>) (who.CurrentItem as Hat).which + 1;
                who.items[who.CurrentToolIndex] = (Item) null;
                who.currentLocation.playSound("dirtyHit");
                return true;
              }
              if (!Game1.didPlayerJustRightClick(true))
                return false;
              this.shakeTimer = 100;
              if (this.SpecialVariable == 0)
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12926"));
              else
                Game1.drawObjectDialogue(this.SpecialVariable == 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12927") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12929", (object) this.SpecialVariable));
              return true;
            }
            if (this.name.Equals("Singing Stone"))
            {
              if (justCheckingForActivity)
                return true;
              if (Game1.soundBank != null)
              {
                ICue cue = Game1.soundBank.GetCue("crystal");
                int num = Game1.random.Next(2400);
                cue.SetVariable("Pitch", num - num % 100);
                this.shakeTimer = 100;
                cue.Play();
                return true;
              }
            }
            else if (this.name.Contains("Feed Hopper") && who.ActiveObject == null)
            {
              if (justCheckingForActivity)
                return true;
              if (who.freeSpotsInInventory() > 0)
              {
                int piecesOfHay = (int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay;
                if (piecesOfHay > 0)
                {
                  bool flag = false;
                  if (who.currentLocation is AnimalHouse)
                  {
                    int val1 = Math.Max(1, Math.Min((who.currentLocation as AnimalHouse).animalsThatLiveHere.Count, piecesOfHay));
                    AnimalHouse currentLocation = who.currentLocation as AnimalHouse;
                    int num3 = currentLocation.numberOfObjectsWithName("Hay");
                    int num4 = Math.Min(val1, (int) (NetFieldBase<int, NetInt>) currentLocation.animalLimit - num3);
                    if (num4 != 0 && Game1.player.couldInventoryAcceptThisObject(178, num4))
                    {
                      (Game1.getLocationFromName("Farm") as Farm).piecesOfHay.Value -= Math.Max(1, num4);
                      who.addItemToInventoryBool((Item) new Object(178, num4));
                      Game1.playSound("shwip");
                      flag = true;
                    }
                  }
                  else if (Game1.player.couldInventoryAcceptThisObject(178, 1))
                  {
                    --(Game1.getLocationFromName("Farm") as Farm).piecesOfHay.Value;
                    who.addItemToInventoryBool((Item) new Object(178, 1));
                    Game1.playSound("shwip");
                  }
                  if ((int) (NetFieldBase<int, NetInt>) (Game1.getLocationFromName("Farm") as Farm).piecesOfHay <= 0)
                    this.showNextIndex.Value = false;
                  if (flag)
                    return true;
                }
                else
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12942"));
              }
              else
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
          }
        }
      }
      Object previous_object = this.heldObject.Value;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest)
        return false;
      if (justCheckingForActivity)
        return true;
      if (who.isMoving())
        Game1.haltAfterCheck = false;
      bool flag1 = false;
      if (this.name.Equals("Bee House"))
      {
        int num5 = -1;
        string str = "Wild";
        int num6 = 0;
        Crop closeFlower = Utility.findCloseFlower(who.currentLocation, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, 5, (Func<Crop, bool>) (crop => !crop.forageCrop.Value));
        if (closeFlower != null)
        {
          str = Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) closeFlower.indexOfHarvest].Split('/')[0];
          num5 = closeFlower.indexOfHarvest.Value;
          num6 = Convert.ToInt32(Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) closeFlower.indexOfHarvest].Split('/')[1]) * 2;
        }
        if (this.heldObject.Value != null)
        {
          this.heldObject.Value.name = str + " Honey";
          this.heldObject.Value.displayName = this.loadDisplayName();
          this.heldObject.Value.Price = Convert.ToInt32(Game1.objectInformation[340].Split('/')[1]) + num6;
          this.heldObject.Value.preservedParentSheetIndex.Value = num5;
          if (Game1.GetSeasonForLocation(Game1.currentLocation).Equals("winter"))
          {
            this.heldObject.Value = (Object) null;
            this.readyForHarvest.Value = false;
            this.showNextIndex.Value = false;
            return false;
          }
          if (who.IsLocalPlayer)
          {
            Object @object = this.heldObject.Value;
            this.heldObject.Value = (Object) null;
            if (!who.addItemToInventoryBool((Item) @object))
            {
              this.heldObject.Value = @object;
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
              return false;
            }
          }
          Game1.playSound("coin");
          flag1 = true;
        }
      }
      else if (who.IsLocalPlayer)
      {
        this.heldObject.Value = (Object) null;
        if (!who.addItemToInventoryBool((Item) previous_object))
        {
          this.heldObject.Value = previous_object;
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          return false;
        }
        Game1.playSound("coin");
        flag1 = true;
        string name = this.name;
        if (!(name == "Keg"))
        {
          if (!(name == "Preserves Jar"))
          {
            if (name == "Cheese Press")
            {
              if (previous_object.ParentSheetIndex == 426)
                ++Game1.stats.GoatCheeseMade;
              else
                ++Game1.stats.CheeseMade;
            }
          }
          else
            ++Game1.stats.PreservesMade;
        }
        else
          ++Game1.stats.BeveragesMade;
      }
      if (this.name.Equals("Crystalarium"))
      {
        this.minutesUntilReady.Value = this.getMinutesForCrystalarium(previous_object.ParentSheetIndex);
        this.heldObject.Value = (Object) previous_object.getOne();
      }
      else if (this.name.Contains("Tapper"))
      {
        if (who.currentLocation.terrainFeatures.ContainsKey((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation) && who.currentLocation.terrainFeatures[(Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation] is Tree)
          (who.currentLocation.terrainFeatures[(Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation] as Tree).UpdateTapperProduct(this, previous_object);
      }
      else
        this.heldObject.Value = (Object) null;
      this.readyForHarvest.Value = false;
      this.showNextIndex.Value = false;
      if (this.name.Equals("Bee House") && !Game1.GetSeasonForLocation(who.currentLocation).Equals("winter"))
      {
        this.heldObject.Value = new Object(Vector2.Zero, 340, (string) null, false, true, false, false);
        this.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 4);
      }
      else if (this.name.Equals("Worm Bin"))
      {
        this.heldObject.Value = new Object(685, Game1.random.Next(2, 6));
        this.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 1);
      }
      if (flag1)
        this.AttemptAutoLoad(who);
      return true;
    }

    public virtual bool IsScarecrow() => this.HasContextTag("crow_scare") || this.Name.Contains("arecrow");

    public virtual int GetRadiusForScarecrow()
    {
      foreach (string contextTag in this.GetContextTags())
      {
        if (contextTag != null && contextTag.StartsWith("crow_scare_radius_"))
        {
          string s = contextTag.Substring("crow_scare_radius".Length + 1);
          int radiusForScarecrow = 0;
          ref int local = ref radiusForScarecrow;
          if (int.TryParse(s, out local))
            return radiusForScarecrow;
        }
      }
      return this.Name.Contains("Deluxe") ? 17 : 9;
    }

    public virtual void AttemptAutoLoad(Farmer who)
    {
      Object object1 = (Object) null;
      if (!who.currentLocation.objects.TryGetValue(new Vector2(this.TileLocation.X, this.TileLocation.Y - 1f), out object1) || object1 == null || !(object1 is Chest))
        return;
      Chest chest = object1 as Chest;
      if (chest.specialChestType.Value != Chest.SpecialChestTypes.AutoLoader)
        return;
      chest.GetMutex().RequestLock((Action) (() =>
      {
        chest.GetMutex().ReleaseLock();
        Object object2 = this.heldObject.Value;
        this.heldObject.Value = (Object) null;
        foreach (Item obj in (NetList<Item, NetRef<Item>>) chest.items)
        {
          Object.autoLoadChest = chest;
          int num = this.performObjectDropInAction(obj, true, who) ? 1 : 0;
          this.heldObject.Value = object2;
          if (num != 0)
          {
            if (this.performObjectDropInAction(obj, false, who))
              this.ConsumeInventoryItem(who, obj, 1);
            Object.autoLoadChest = (Chest) null;
            return;
          }
        }
        Object.autoLoadChest = (Chest) null;
        this.heldObject.Value = object2;
      }));
    }

    public virtual void farmerAdjacentAction(GameLocation location)
    {
      if (this.name == null || this.isTemporarilyInvisible)
        return;
      if (this.name.Equals("Flute Block") && (this.internalSound == null || (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds - this.lastNoteBlockSoundTime >= 1000 && !this.internalSound.IsPlaying) && !Game1.dialogueUp)
      {
        if (Game1.soundBank != null)
        {
          this.internalSound = Game1.soundBank.GetCue("flute");
          this.internalSound.SetVariable("Pitch", this.preservedParentSheetIndex.Value);
          this.internalSound.Play();
        }
        this.scale.Y = 1.3f;
        this.shakeTimer = 200;
        this.lastNoteBlockSoundTime = (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
        if (!(location is IslandSouthEast))
          return;
        (location as IslandSouthEast).OnFlutePlayed(this.preservedParentSheetIndex.Value);
      }
      else if (this.name.Equals("Drum Block") && (this.internalSound == null || Game1.currentGameTime.TotalGameTime.TotalMilliseconds - (double) this.lastNoteBlockSoundTime >= 1000.0 && !this.internalSound.IsPlaying) && !Game1.dialogueUp)
      {
        if (Game1.soundBank != null)
        {
          this.internalSound = Game1.soundBank.GetCue("drumkit" + this.preservedParentSheetIndex.Value.ToString());
          this.internalSound.Play();
        }
        this.scale.Y = 1.3f;
        this.shakeTimer = 200;
        this.lastNoteBlockSoundTime = (int) Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
      }
      else
      {
        if (!this.name.Equals("Obelisk"))
          return;
        ++this.scale.X;
        if ((double) this.scale.X > 30.0)
        {
          this.ParentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 29 ? 30 : 29;
          this.scale.X = 0.0f;
          this.scale.Y += 2f;
        }
        if ((double) this.scale.Y < 20.0 || Game1.random.NextDouble() >= 0.0001 || location.characters.Count >= 4)
          return;
        Vector2 tileLocation1 = Game1.player.getTileLocation();
        foreach (Vector2 adjacentTilesOffset in Character.AdjacentTilesOffsets)
        {
          Vector2 tileLocation2 = tileLocation1 + adjacentTilesOffset;
          if (!location.isTileOccupied(tileLocation2) && location.isTilePassable(new Location((int) tileLocation2.X, (int) tileLocation2.Y), Game1.viewport) && location.isCharacterAtTile(tileLocation2) == null)
          {
            if (Game1.random.NextDouble() < 0.1)
              location.characters.Add((NPC) new GreenSlime(tileLocation2 * new Vector2(64f, 64f)));
            else if (Game1.random.NextDouble() < 0.5)
              location.characters.Add((NPC) new ShadowGuy(tileLocation2 * new Vector2(64f, 64f)));
            else
              location.characters.Add((NPC) new ShadowGirl(tileLocation2 * new Vector2(64f, 64f)));
            location.characters[location.characters.Count - 1].moveTowardPlayerThreshold.Value = 4;
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(352, 400f, 2, 1, tileLocation2 * new Vector2(64f, 64f), false, false));
            location.playSound("shadowpeep");
            break;
          }
        }
      }
    }

    public virtual void addWorkingAnimation(GameLocation environment)
    {
      if (environment == null || !environment.farmers.Any())
        return;
      string name = this.name;
      if (!(name == "Keg"))
      {
        if (!(name == "Preserves Jar"))
        {
          if (!(name == "Oil Maker"))
          {
            if (!(name == "Furnace"))
            {
              if (!(name == "Slime Egg-Press"))
                return;
              Game1.multiplayer.broadcastSprites(environment, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, -160f), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Lime, 1f, 0.0f, 0.0f, 0.0f)
              {
                alphaFade = 0.005f
              });
            }
            else
            {
              if (Game1.random.NextDouble() >= 0.5)
                return;
              Game1.multiplayer.broadcastSprites(environment, new TemporaryAnimatedSprite(30, this.tileLocation.Value * 64f + new Vector2(0.0f, -16f), Color.White, 4, animationInterval: 50f, numberOfLoops: 10, sourceRectWidth: 64, layerDepth: ((float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05)))
              {
                alphaFade = 0.005f,
                light = true,
                lightcolor = Color.Black
              });
              environment.playSound("fireball");
            }
          }
          else
            Game1.multiplayer.broadcastSprites(environment, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow, 1f, 0.0f, 0.0f, 0.0f)
            {
              alphaFade = 0.005f
            });
        }
        else
        {
          Color color = Color.White;
          if (this.heldObject.Value.Name.Contains("Pickled"))
            color = Color.White;
          else if (this.heldObject.Value.Name.Contains("Jelly"))
            color = Color.LightBlue;
          Game1.multiplayer.broadcastSprites(environment, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, color * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
          {
            alphaFade = 0.005f
          });
        }
      }
      else
      {
        Color color = Color.DarkGray;
        if (this.heldObject.Value.Name.Contains("Wine"))
          color = Color.Lavender;
        else if (this.heldObject.Value.Name.Contains("Juice"))
          color = Color.White;
        else if (this.heldObject.Value.name.Equals("Beer"))
          color = Color.Yellow;
        Game1.multiplayer.broadcastSprites(environment, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) this.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, color * 0.75f, 1f, 0.0f, 0.0f, 0.0f)
        {
          alphaFade = 0.005f
        });
        environment.playSound("bubbles");
      }
    }

    public virtual void onReadyForHarvest(GameLocation environment)
    {
    }

    public virtual bool minutesElapsed(int minutes, GameLocation environment)
    {
      if (this.heldObject.Value != null && !this.name.Contains("Table") && (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 165))
      {
        if (this.name.Equals("Bee House") && !environment.IsOutdoors || this.IsSprinkler() || (bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 231)
          return false;
        if (Game1.IsMasterGame)
          this.minutesUntilReady.Value -= minutes;
        if ((int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady <= 0 && !this.name.Contains("Incubator"))
        {
          if (!(bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest)
            environment.playSound("dwop");
          this.readyForHarvest.Value = true;
          this.minutesUntilReady.Value = 0;
          this.onReadyForHarvest(environment);
          this.showNextIndex.Value = false;
          if (this.name.Equals("Bee House") || this.name.Equals("Loom") || this.name.Equals("Mushroom Box"))
            this.showNextIndex.Value = true;
          if (this.lightSource != null)
          {
            environment.removeLightSource((int) (NetFieldBase<int, NetInt>) this.lightSource.identifier);
            this.lightSource = (LightSource) null;
          }
        }
        if (!(bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest && Game1.random.NextDouble() < 0.33)
          this.addWorkingAnimation(environment);
      }
      else if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
        {
          case 29:
          case 30:
            this.showNextIndex.Value = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 29;
            this.scale.Y = Math.Max(0.0f, this.scale.Y -= (float) (minutes / 2 + 1));
            break;
          case 83:
            this.showNextIndex.Value = false;
            environment.removeLightSource((int) ((double) this.tileLocation.X * 797.0 + (double) this.tileLocation.Y * 13.0 + 666.0));
            break;
          case 96:
          case 97:
            this.minutesUntilReady.Value -= minutes;
            this.showNextIndex.Value = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 96;
            if ((int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady <= 0)
            {
              this.performRemoveAction((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, environment);
              environment.objects.Remove((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
              environment.objects.Add((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, new Object((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, 98));
              if (!Game1.MasterPlayer.mailReceived.Contains("Capsule_Broken"))
              {
                Game1.MasterPlayer.mailReceived.Add("Capsule_Broken");
                break;
              }
              break;
            }
            break;
          case 141:
          case 142:
            this.showNextIndex.Value = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 141;
            break;
        }
      }
      return false;
    }

    public override string checkForSpecialItemHoldUpMeessage()
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        if ((NetFieldBase<string, NetString>) this.type != (NetString) null && this.type.Equals((object) "Arch"))
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12993");
        switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
        {
          case 102:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12994");
          case 535:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12995");
        }
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 160)
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12996");
      return base.checkForSpecialItemHoldUpMeessage();
    }

    public virtual bool countsForShippedCollection()
    {
      if ((NetFieldBase<string, NetString>) this.type == (NetString) null || this.type.Contains("Arch") || (bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
        return false;
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 433)
        return true;
      int category = this.Category;
      if (category <= -14)
      {
        if (category != -74)
        {
          switch (category - -29)
          {
            case 0:
            case 5:
            case 7:
            case 8:
            case 9:
            case 10:
              break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 6:
              goto label_12;
            default:
              if (category == -14)
                break;
              goto label_12;
          }
        }
      }
      else if (category <= -7)
      {
        if (category != -12 && (uint) (category - -8) > 1U)
          goto label_12;
      }
      else if (category != -2 && category != 0)
        goto label_12;
      return false;
label_12:
      return Object.isIndexOkForBasicShippedCategory((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);
    }

    public static bool isIndexOkForBasicShippedCategory(int index) => index != 434 && index != 889 && index != 928;

    public static bool isPotentialBasicShippedCategory(int index, string category)
    {
      int result = 0;
      int.TryParse(category, out result);
      if (index == 433)
        return true;
      switch (result)
      {
        case -74:
        case -29:
        case -24:
        case -22:
        case -21:
        case -20:
        case -19:
        case -14:
        case -12:
        case -8:
        case -7:
        case -2:
          return false;
        case 0:
          return false;
        default:
          return Object.isIndexOkForBasicShippedCategory(index);
      }
    }

    public virtual Vector2 getScale()
    {
      if (this.Category == -22)
        return Vector2.Zero;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        this.scale.Y = Math.Max(4f, this.scale.Y - 0.04f);
        return this.scale;
      }
      if (this.heldObject.Value == null && (int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady <= 0 || (bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 10 || this.name.Contains("Table") || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 264 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 165 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 231)
        return Vector2.Zero;
      if (this.name.Equals("Loom"))
      {
        this.scale.X = (float) (((double) this.scale.X + 0.0399999991059303) % (2.0 * Math.PI));
        return Vector2.Zero;
      }
      this.scale.X -= 0.1f;
      this.scale.Y += 0.1f;
      if ((double) this.scale.X <= 0.0)
        this.scale.X = 10f;
      if ((double) this.scale.Y >= 10.0)
        this.scale.Y = 0.0f;
      return new Vector2(Math.Abs(this.scale.X - 5f), Math.Abs(this.scale.Y - 5f));
    }

    public virtual void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) f.ActiveObject.bigCraftable)
      {
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(Object.getSourceRectForBigCraftable(f.ActiveObject.ParentSheetIndex)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 3) / 10000f));
      }
      else
      {
        spriteBatch.Draw(Game1.objectSpriteSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 3) / 10000f));
        if (f.ActiveObject == null || !f.ActiveObject.Name.Contains("="))
          return;
        spriteBatch.Draw(Game1.objectSpriteSheet, objectPosition + new Vector2(32f, 32f), new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0.0f, new Vector2(32f, 32f), (float) (4.0 + (double) Math.Abs(Game1.starCropShimmerPause) / 8.0), SpriteEffects.None, Math.Max(0.0f, (float) (f.getStandingY() + 3) / 10000f));
        if ((double) Math.Abs(Game1.starCropShimmerPause) <= 0.0500000007450581 && Game1.random.NextDouble() < 0.97)
          return;
        Game1.starCropShimmerPause += 0.04f;
        if ((double) Game1.starCropShimmerPause < 0.800000011920929)
          return;
        Game1.starCropShimmerPause = -0.8f;
      }
    }

    public virtual void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
    {
      if (!this.isPlaceable() || this is Wallpaper)
        return;
      int x = (int) Game1.GetPlacementGrabTile().X * 64;
      int y = (int) Game1.GetPlacementGrabTile().Y * 64;
      Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
      if (Game1.isCheckingNonMousePlacement)
      {
        Vector2 placementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, location, (Item) this, x, y);
        x = (int) placementPosition.X;
        y = (int) placementPosition.Y;
      }
      if (Utility.isThereAnObjectHereWhichAcceptsThisItem(location, (Item) this, x, y))
        return;
      bool flag = Utility.playerCanPlaceItemHere(location, (Item) this, x, y, Game1.player) || Utility.isThereAnObjectHereWhichAcceptsThisItem(location, (Item) this, x, y) && Utility.withinRadiusOfPlayer(x, y, 1, Game1.player);
      Game1.isCheckingNonMousePlacement = false;
      int num1 = 1;
      int num2 = 1;
      if (this is Furniture)
      {
        Furniture furniture = this as Furniture;
        num1 = furniture.getTilesWide();
        num2 = furniture.getTilesHigh();
      }
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < num2; ++index2)
          spriteBatch.Draw(Game1.mouseCursors, new Vector2((float) ((x / 64 + index1) * 64 - Game1.viewport.X), (float) ((y / 64 + index2) * 64 - Game1.viewport.Y)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(flag ? 194 : 210, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && !(this is Furniture) && ((int) (NetFieldBase<int, NetInt>) this.category == -74 || (int) (NetFieldBase<int, NetInt>) this.category == -19))
        return;
      this.draw(spriteBatch, x / 64, y / 64, 0.5f);
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.isRecipe)
      {
        transparency = 0.5f;
        scaleSize *= 0.75f;
      }
      bool flag = (drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1 || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double) scaleSize > 0.3 && this.Stack != int.MaxValue;
      if (this.IsRecipe)
        flag = false;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        Microsoft.Xna.Framework.Rectangle rectForBigCraftable = Object.getSourceRectForBigCraftable((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, location + new Vector2(32f, 32f), new Microsoft.Xna.Framework.Rectangle?(rectForBigCraftable), color * transparency, 0.0f, new Vector2(8f, 16f), (float) (4.0 * ((double) scaleSize < 0.2 ? (double) scaleSize : (double) scaleSize / 2.0)), SpriteEffects.None, layerDepth);
        if (flag)
          Utility.drawTinyDigits((int) (NetFieldBase<int, NetInt>) this.stack, spriteBatch, location + new Vector2((float) (64 - Utility.getWidthOfTinyDigitString((int) (NetFieldBase<int, NetInt>) this.stack, 3f * scaleSize)) + 3f * scaleSize, (float) (64.0 - 18.0 * (double) scaleSize + 2.0)), 3f * scaleSize, 1f, color);
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 590 & drawShadow)
          spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(32f, 48f), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), color * 0.5f, 0.0f, new Vector2((float) Game1.shadowTexture.Bounds.Center.X, (float) Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
        spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2((float) (int) (32.0 * (double) scaleSize), (float) (int) (32.0 * (double) scaleSize)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 16, 16)), color * transparency, 0.0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
        if (flag)
          Utility.drawTinyDigits((int) (NetFieldBase<int, NetInt>) this.stack, spriteBatch, location + new Vector2((float) (64 - Utility.getWidthOfTinyDigitString((int) (NetFieldBase<int, NetInt>) this.stack, 3f * scaleSize)) + 3f * scaleSize, (float) (64.0 - 18.0 * (double) scaleSize + 1.0)), 3f * scaleSize, 1f, color);
        if (drawStackNumber != StackDrawType.Hide && (int) (NetFieldBase<int, NetInt>) this.quality > 0)
        {
          Microsoft.Xna.Framework.Rectangle rectangle = (int) (NetFieldBase<int, NetInt>) this.quality < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + ((int) (NetFieldBase<int, NetInt>) this.quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8);
          Texture2D mouseCursors = Game1.mouseCursors;
          float num = (int) (NetFieldBase<int, NetInt>) this.quality < 4 ? 0.0f : (float) ((Math.Cos((double) Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
          spriteBatch.Draw(mouseCursors, location + new Vector2(12f, 52f + num), new Microsoft.Xna.Framework.Rectangle?(rectangle), color * transparency, 0.0f, new Vector2(4f, 4f), (float) (3.0 * (double) scaleSize * (1.0 + (double) num)), SpriteEffects.None, layerDepth);
        }
        if (this.Category == -22 && this.uses.Value > 0)
        {
          float power = ((float) (FishingRod.maxTackleUses - this.uses.Value) + 0.0f) / (float) FishingRod.maxTackleUses;
          spriteBatch.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int) location.X, (int) ((double) location.Y + 56.0 * (double) scaleSize), (int) (64.0 * (double) scaleSize * (double) power), (int) (8.0 * (double) scaleSize)), Utility.getRedToGreenLerpColor(power));
        }
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.isRecipe)
        return;
      spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(16f, 16f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16)), color, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, layerDepth + 0.0001f);
    }

    public virtual void drawAsProp(SpriteBatch b)
    {
      if (this.isTemporarilyInvisible)
        return;
      int x1 = (int) this.tileLocation.X;
      int y1 = (int) this.tileLocation.Y;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        Vector2 vector2 = this.getScale() * 4f;
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x1 * 64), (float) (y1 * 64 - 64)));
        Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) local.X - (double) vector2.X / 2.0), (int) ((double) local.Y - (double) vector2.Y / 2.0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
        b.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(Object.getSourceRectForBigCraftable((bool) (NetFieldBase<bool, NetBool>) this.showNextIndex ? this.ParentSheetIndex + 1 : this.ParentSheetIndex)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, Math.Max(0.0f, (float) ((y1 + 1) * 64 - 1) / 10000f) + ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 264 ? 0.0015f : 0.0f));
        if (!this.Name.Equals("Loom") || (int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady <= 0)
          return;
        b.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435)), Color.White, this.scale.X, new Vector2(32f, 32f), 1f, SpriteEffects.None, Math.Max(0.0f, (float) ((double) ((y1 + 1) * 64 - 1) / 10000.0 + 9.99999974737875E-05)));
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle rectangle;
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 590 && (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 742)
        {
          SpriteBatch spriteBatch = b;
          Texture2D shadowTexture = Game1.shadowTexture;
          Vector2 position = this.getLocalPosition(Game1.viewport) + new Vector2(32f, 53f);
          Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
          Color white = Color.White;
          rectangle = Game1.shadowTexture.Bounds;
          double x2 = (double) rectangle.Center.X;
          rectangle = Game1.shadowTexture.Bounds;
          double y2 = (double) rectangle.Center.Y;
          Vector2 origin = new Vector2((float) x2, (float) y2);
          rectangle = this.getBoundingBox(new Vector2((float) x1, (float) y1));
          double layerDepth = (double) rectangle.Bottom / 15000.0;
          spriteBatch.Draw(shadowTexture, position, sourceRectangle, white, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
        }
        SpriteBatch spriteBatch1 = b;
        Texture2D objectSpriteSheet = Game1.objectSpriteSheet;
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x1 * 64 + 32), (float) (y1 * 64 + 32)));
        Microsoft.Xna.Framework.Rectangle? sourceRectangle1 = new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(this.ParentSheetIndex));
        Color white1 = Color.White;
        Vector2 origin1 = new Vector2(8f, 8f);
        Vector2 scale1 = this.scale;
        double scale2 = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
        int effects = (bool) (NetFieldBase<bool, NetBool>) this.flipped ? 1 : 0;
        rectangle = this.getBoundingBox(new Vector2((float) x1, (float) y1));
        double layerDepth1 = (double) rectangle.Bottom / 10000.0;
        spriteBatch1.Draw(objectSpriteSheet, local, sourceRectangle1, white1, 0.0f, origin1, (float) scale2, (SpriteEffects) effects, (float) layerDepth1);
      }
    }

    public virtual void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      if (this.isTemporarilyInvisible)
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        Vector2 vector2 = this.getScale() * 4f;
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64 - 64)));
        Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) local.X - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
        float layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 - 24) / 10000f) + (float) x * 1E-05f;
        if (this.ParentSheetIndex == 105 || this.ParentSheetIndex == 264)
          layerDepth = Math.Max(0.0f, (float) ((y + 1) * 64 + 2) / 10000f) + (float) x / 1000000f;
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 272)
        {
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(Object.getSourceRectForBigCraftable(this.ParentSheetIndex + 1)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
          spriteBatch.Draw(Game1.bigCraftableSpriteSheet, local + new Vector2(8.5f, 12f) * 4f, new Microsoft.Xna.Framework.Rectangle?(Object.getSourceRectForBigCraftable(this.ParentSheetIndex + 2)), Color.White * alpha, (float) (Game1.currentGameTime.TotalGameTime.TotalSeconds * -1.5), new Vector2(7.5f, 15.5f), 4f, SpriteEffects.None, layerDepth + 1E-05f);
          return;
        }
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(Object.getSourceRectForBigCraftable((bool) (NetFieldBase<bool, NetBool>) this.showNextIndex ? this.ParentSheetIndex + 1 : this.ParentSheetIndex)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        if (this.Name.Equals("Loom") && (int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady > 0)
          spriteBatch.Draw(Game1.objectSpriteSheet, this.getLocalPosition(Game1.viewport) + new Vector2(32f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, 16, 16)), Color.White * alpha, this.scale.X, new Vector2(8f, 8f), 4f, SpriteEffects.None, Math.Max(0.0f, (float) ((double) ((y + 1) * 64) / 10000.0 + 9.99999974737875E-05 + (double) x * 9.99999974737875E-06)));
        if ((bool) (NetFieldBase<bool, NetBool>) this.isLamp && Game1.isDarkOut())
          spriteBatch.Draw(Game1.mouseCursors, local + new Vector2(-32f, -32f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(88, 1779, 32, 32)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 - 20) / 10000f) + (float) x / 1000000f);
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 126 && (int) (NetFieldBase<int, NetInt>) this.quality != 0)
          spriteBatch.Draw(FarmerRenderer.hatsTexture, local + new Vector2(-3f, -6f) * 4f, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(((int) (NetFieldBase<int, NetInt>) this.quality - 1) * 20 % FarmerRenderer.hatsTexture.Width, ((int) (NetFieldBase<int, NetInt>) this.quality - 1) * 20 / FarmerRenderer.hatsTexture.Width * 20 * 4, 20, 20)), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float) ((y + 1) * 64 - 20) / 10000f) + (float) x * 1E-05f);
      }
      else if (!Game1.eventUp || Game1.CurrentEvent != null && !Game1.CurrentEvent.isTileWalkedOn(x, y))
      {
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 590)
        {
          SpriteBatch spriteBatch1 = spriteBatch;
          Texture2D mouseCursors = Game1.mouseCursors;
          Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
          Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(368 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1200.0 <= 400.0 ? (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 400.0 / 100.0) * 16 : 0), 32, 16, 16));
          Color color = Color.White * alpha;
          Vector2 origin = new Vector2(8f, 8f);
          Vector2 scale1 = this.scale;
          double scale2 = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
          int effects = (bool) (NetFieldBase<bool, NetBool>) this.flipped ? 1 : 0;
          double layerDepth = (this.isPassable() ? (double) this.getBoundingBox(new Vector2((float) x, (float) y)).Top : (double) this.getBoundingBox(new Vector2((float) x, (float) y)).Bottom) / 10000.0;
          spriteBatch1.Draw(mouseCursors, local, sourceRectangle, color, 0.0f, origin, (float) scale2, (SpriteEffects) effects, (float) layerDepth);
          return;
        }
        Microsoft.Xna.Framework.Rectangle rectangle;
        if ((int) (NetFieldBase<int, NetInt>) this.fragility != 2)
        {
          SpriteBatch spriteBatch2 = spriteBatch;
          Texture2D shadowTexture = Game1.shadowTexture;
          Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32), (float) (y * 64 + 51 + 4)));
          Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
          Color color = Color.White * alpha;
          rectangle = Game1.shadowTexture.Bounds;
          double x1 = (double) rectangle.Center.X;
          rectangle = Game1.shadowTexture.Bounds;
          double y1 = (double) rectangle.Center.Y;
          Vector2 origin = new Vector2((float) x1, (float) y1);
          rectangle = this.getBoundingBox(new Vector2((float) x, (float) y));
          double layerDepth = (double) rectangle.Bottom / 15000.0;
          spriteBatch2.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth);
        }
        SpriteBatch spriteBatch3 = spriteBatch;
        Texture2D objectSpriteSheet1 = Game1.objectSpriteSheet;
        Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
        Microsoft.Xna.Framework.Rectangle? sourceRectangle1 = new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(this.ParentSheetIndex));
        Color color1 = Color.White * alpha;
        Vector2 origin1 = new Vector2(8f, 8f);
        Vector2 scale3 = this.scale;
        double scale4 = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
        int effects1 = (bool) (NetFieldBase<bool, NetBool>) this.flipped ? 1 : 0;
        int num1;
        if (!this.isPassable())
        {
          rectangle = this.getBoundingBox(new Vector2((float) x, (float) y));
          num1 = rectangle.Bottom;
        }
        else
        {
          rectangle = this.getBoundingBox(new Vector2((float) x, (float) y));
          num1 = rectangle.Top;
        }
        double layerDepth1 = (double) num1 / 10000.0;
        spriteBatch3.Draw(objectSpriteSheet1, local1, sourceRectangle1, color1, 0.0f, origin1, (float) scale4, (SpriteEffects) effects1, (float) layerDepth1);
        if (this.heldObject.Value != null && this.IsSprinkler())
        {
          Vector2 vector2 = Vector2.Zero;
          if (this.heldObject.Value.ParentSheetIndex == 913)
            vector2 = new Vector2(0.0f, -20f);
          SpriteBatch spriteBatch4 = spriteBatch;
          Texture2D objectSpriteSheet2 = Game1.objectSpriteSheet;
          Vector2 local2 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (y * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))) + vector2);
          Microsoft.Xna.Framework.Rectangle? sourceRectangle2 = new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex + 1));
          Color color2 = Color.White * alpha;
          Vector2 origin2 = new Vector2(8f, 8f);
          Vector2 scale5 = this.scale;
          double scale6 = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
          int effects2 = (bool) (NetFieldBase<bool, NetBool>) this.flipped ? 1 : 0;
          int num2;
          if (!this.isPassable())
          {
            rectangle = this.getBoundingBox(new Vector2((float) x, (float) y));
            num2 = rectangle.Bottom;
          }
          else
          {
            rectangle = this.getBoundingBox(new Vector2((float) x, (float) y));
            num2 = rectangle.Top;
          }
          double layerDepth2 = (double) num2 / 10000.0 + 9.99999974737875E-06;
          spriteBatch4.Draw(objectSpriteSheet2, local2, sourceRectangle2, color2, 0.0f, origin2, (float) scale6, (SpriteEffects) effects2, (float) layerDepth2);
        }
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.readyForHarvest)
        return;
      float num3 = (float) ((double) ((y + 1) * 64) / 10000.0 + (double) this.tileLocation.X / 50000.0);
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 264)
        num3 += 0.02f;
      float num4 = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
      spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 - 8), (float) (y * 64 - 96 - 16) + num4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num3 + 1E-06f);
      if (this.heldObject.Value == null)
        return;
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32), (float) (y * 64 - 64 - 8) + num4)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.heldObject.Value.parentSheetIndex, 16, 16)), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, num3 + 1E-05f);
      if (!(this.heldObject.Value is ColoredObject))
        return;
      spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64 + 32), (float) (y * 64 - 64 - 8) + num4)), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.heldObject.Value.parentSheetIndex + 1, 16, 16)), (this.heldObject.Value as ColoredObject).color.Value * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, num3 + 1.1E-05f);
    }

    public virtual void draw(
      SpriteBatch spriteBatch,
      int xNonTile,
      int yNonTile,
      float layerDepth,
      float alpha = 1f)
    {
      if (this.isTemporarilyInvisible)
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        Vector2 vector2 = this.getScale() * 4f;
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) xNonTile, (float) yNonTile));
        Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle((int) ((double) local.X - (double) vector2.X / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) ((double) local.Y - (double) vector2.Y / 2.0) + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int) (64.0 + (double) vector2.X), (int) (128.0 + (double) vector2.Y / 2.0));
        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(Object.getSourceRectForBigCraftable((bool) (NetFieldBase<bool, NetBool>) this.showNextIndex ? this.ParentSheetIndex + 1 : this.ParentSheetIndex)), Color.White * alpha, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        if (this.Name.Equals("Loom") && (int) (NetFieldBase<int, NetIntDelta>) this.minutesUntilReady > 0)
          spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(local) + new Vector2(32f, 0.0f), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 435, 16, 16)), Color.White * alpha, this.scale.X, new Vector2(8f, 8f), 4f, SpriteEffects.None, layerDepth);
        if (!(bool) (NetFieldBase<bool, NetBool>) this.isLamp || !Game1.isDarkOut())
          return;
        spriteBatch.Draw(Game1.mouseCursors, local + new Vector2(-32f, -32f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(88, 1779, 32, 32)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth);
      }
      else
      {
        if (Game1.eventUp && Game1.CurrentEvent.isTileWalkedOn(xNonTile / 64, yNonTile / 64))
          return;
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 590 && (int) (NetFieldBase<int, NetInt>) this.fragility != 2)
        {
          SpriteBatch spriteBatch1 = spriteBatch;
          Texture2D shadowTexture = Game1.shadowTexture;
          Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (xNonTile + 32), (float) (yNonTile + 51 + 4)));
          Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
          Color color = Color.White * alpha;
          Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
          double x = (double) bounds.Center.X;
          bounds = Game1.shadowTexture.Bounds;
          double y = (double) bounds.Center.Y;
          Vector2 origin = new Vector2((float) x, (float) y);
          double layerDepth1 = (double) layerDepth - 9.99999997475243E-07;
          spriteBatch1.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, (float) layerDepth1);
        }
        SpriteBatch spriteBatch2 = spriteBatch;
        Texture2D objectSpriteSheet = Game1.objectSpriteSheet;
        Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (xNonTile + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float) (yNonTile + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
        Microsoft.Xna.Framework.Rectangle? sourceRectangle1 = new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(this.ParentSheetIndex));
        Color color1 = Color.White * alpha;
        Vector2 origin1 = new Vector2(8f, 8f);
        Vector2 scale1 = this.scale;
        double scale2 = (double) this.scale.Y > 1.0 ? (double) this.getScale().Y : 4.0;
        int effects = (bool) (NetFieldBase<bool, NetBool>) this.flipped ? 1 : 0;
        double layerDepth2 = (double) layerDepth;
        spriteBatch2.Draw(objectSpriteSheet, local1, sourceRectangle1, color1, 0.0f, origin1, (float) scale2, (SpriteEffects) effects, (float) layerDepth2);
      }
    }

    private int getMinutesForCrystalarium(int whichGem)
    {
      switch (whichGem)
      {
        case 60:
          return 3000;
        case 62:
          return 2240;
        case 64:
          return 3000;
        case 66:
          return 1360;
        case 68:
          return 1120;
        case 70:
          return 2400;
        case 72:
          return 7200;
        case 80:
          return 420;
        case 82:
          return 1300;
        case 84:
          return 1120;
        case 86:
          return 800;
        default:
          return 5000;
      }
    }

    public override int maximumStackSize() => this.ParentSheetIndex == 911 || this.Category == -22 ? 1 : 999;

    public override int addToStack(Item otherStack)
    {
      int num = this.maximumStackSize();
      if (num == 1)
        return otherStack.Stack;
      this.stack.Value += otherStack.Stack;
      if (otherStack is Object)
      {
        Object @object = otherStack as Object;
        if (this.IsSpawnedObject && !@object.IsSpawnedObject)
          this.IsSpawnedObject = false;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.stack <= num)
        return 0;
      int stack = (int) (NetFieldBase<int, NetInt>) this.stack - num;
      this.stack.Value = num;
      return stack;
    }

    public virtual void hoverAction()
    {
    }

    public virtual bool clicked(Farmer who) => false;

    public override Item getOne()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable)
      {
        int parentSheetIndex = this.ParentSheetIndex;
        if (this.name.Contains("Seasonal"))
          parentSheetIndex = this.ParentSheetIndex - this.ParentSheetIndex % 4;
        Object one = new Object((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, parentSheetIndex);
        one.IsRecipe = (bool) (NetFieldBase<bool, NetBool>) this.isRecipe;
        one.name = this.name;
        one.DisplayName = this.DisplayName;
        one.SpecialVariable = this.SpecialVariable;
        one._GetOneFrom((Item) this);
        return (Item) one;
      }
      Object one1 = new Object((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 1);
      one1.Scale = this.scale;
      one1.Quality = (int) (NetFieldBase<int, NetInt>) this.quality;
      one1.IsSpawnedObject = (bool) (NetFieldBase<bool, NetBool>) this.isSpawnedObject;
      one1.IsRecipe = (bool) (NetFieldBase<bool, NetBool>) this.isRecipe;
      one1.Stack = 1;
      one1.SpecialVariable = this.SpecialVariable;
      one1.Price = (int) (NetFieldBase<int, NetInt>) this.price;
      one1.name = this.name;
      one1.DisplayName = this.DisplayName;
      one1.HasBeenInInventory = this.HasBeenInInventory;
      one1.HasBeenPickedUpByFarmer = this.HasBeenPickedUpByFarmer;
      one1.uses.Value = this.uses.Value;
      one1.questItem.Value = (bool) (NetFieldBase<bool, NetBool>) this.questItem;
      one1.questId.Value = (int) (NetFieldBase<int, NetInt>) this.questId;
      one1.preserve.Value = this.preserve.Value;
      one1.preservedParentSheetIndex.Value = this.preservedParentSheetIndex.Value;
      one1._GetOneFrom((Item) this);
      return (Item) one1;
    }

    public override void _GetOneFrom(Item source)
    {
      this.orderData.Value = (source as Object).orderData.Value;
      this.owner.Value = (source as Object).owner.Value;
      base._GetOneFrom(source);
    }

    public override bool canBePlacedHere(GameLocation l, Vector2 tile)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 710)
        return CrabPot.IsValidCrabPotLocationTile(l, (int) tile.X, (int) tile.Y);
      if (((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 105 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 264) && (bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && l.terrainFeatures.ContainsKey(tile) && l.terrainFeatures[tile] is Tree && !l.objects.ContainsKey(tile) || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 805 && l.terrainFeatures.ContainsKey(tile) && l.terrainFeatures[tile] is Tree || this.name != null && this.name.Contains("Bomb") && (!l.isTileOccupiedForPlacement(tile, this) || l.isTileOccupiedByFarmer(tile) != null))
        return true;
      if (Object.isWildTreeSeed((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex))
        return !l.isTileOccupiedForPlacement(tile, this) && this.canPlaceWildTreeSeed(l, tile);
      if (((int) (NetFieldBase<int, NetInt>) this.category == -74 || (int) (NetFieldBase<int, NetInt>) this.category == -19) && !l.isTileHoeDirt(tile) && !this.bigCraftable.Value)
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
        {
          case 69:
          case 292:
          case 309:
          case 310:
          case 311:
          case 628:
          case 629:
          case 630:
          case 631:
          case 632:
          case 633:
          case 835:
          case 891:
            if (l.isTileOccupiedForPlacement(tile, this))
              return false;
            return l.CanPlantTreesHere((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (int) tile.X, (int) tile.Y) || (bool) (NetFieldBase<bool, NetBool>) l.isOutdoors;
          case 251:
            if (l.isTileOccupiedForPlacement(tile, this))
              return false;
            if ((bool) (NetFieldBase<bool, NetBool>) l.isOutdoors)
              return true;
            return l.IsGreenhouse && l.doesTileHaveProperty((int) tile.X, (int) tile.Y, "Diggable", "Back") != null;
          default:
            return false;
        }
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.category == -19 && l.isTileHoeDirt(tile) && ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 805 || l.terrainFeatures.ContainsKey(tile) && l.terrainFeatures[tile] is HoeDirt && (int) (NetFieldBase<int, NetInt>) (l.terrainFeatures[tile] as HoeDirt).fertilizer != 0 || l.objects.ContainsKey(tile) && l.objects[tile] is IndoorPot && (int) (NetFieldBase<int, NetInt>) (l.objects[tile] as IndoorPot).hoeDirt.Value.fertilizer != 0))
          return false;
        if (l != null)
        {
          Vector2 vector2 = tile * 64f * 64f;
          vector2.X += 32f;
          vector2.Y += 32f;
          foreach (Furniture furniture in l.furniture)
          {
            if ((int) (NetFieldBase<int, NetInt>) furniture.furniture_type == 11 && furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Contains((int) vector2.X, (int) vector2.Y) && furniture.heldObject.Value == null)
              return true;
            if (furniture.getBoundingBox(furniture.TileLocation).Intersects(new Microsoft.Xna.Framework.Rectangle((int) tile.X * 64, (int) tile.Y * 64, 64, 64)) && !furniture.isPassable() && !furniture.AllowPlacementOnThisTile((int) tile.X, (int) tile.Y))
              return false;
          }
        }
        return !l.isTileOccupiedForPlacement(tile, this);
      }
    }

    public override bool isPlaceable()
    {
      if (Utility.IsNormalObjectAtParentSheetIndex((Item) this, 681) || Utility.IsNormalObjectAtParentSheetIndex((Item) this, 688) || Utility.IsNormalObjectAtParentSheetIndex((Item) this, 689) || Utility.IsNormalObjectAtParentSheetIndex((Item) this, 690) || Utility.IsNormalObjectAtParentSheetIndex((Item) this, 261) || Utility.IsNormalObjectAtParentSheetIndex((Item) this, 886) || Utility.IsNormalObjectAtParentSheetIndex((Item) this, 896) || Utility.IsNormalObjectAtParentSheetIndex((Item) this, 911) || Utility.IsNormalObjectAtParentSheetIndex((Item) this, 879))
        return false;
      int category = this.Category;
      return this.type.Value != null && (this.Category == -8 || this.Category == -9 || this.type.Value.Equals("Crafting") || this.isSapling() || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 710 || this.Category == -74 || this.Category == -19) && ((int) (NetFieldBase<int, NetInt>) this.edibility < 0 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 292 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 891);
    }

    public bool IsConsideredReadyMachineForComputer()
    {
      if (this.bigCraftable.Value && this.heldObject.Value != null)
      {
        if (!(this.heldObject.Value is Chest))
          return this.minutesUntilReady.Value <= 0;
        if (!(this.heldObject.Value as Chest).isEmpty())
          return true;
      }
      return false;
    }

    public virtual bool isSapling() => !this.bigCraftable.Value && this.GetType() == typeof (Object) && this.name.Contains("Sapling");

    public static bool isWildTreeSeed(int index) => index == 309 || index == 310 || index == 311 || index == 292 || index == 891;

    private bool canPlaceWildTreeSeed(GameLocation location, Vector2 tile)
    {
      bool flag1 = location.doesTileHaveProperty((int) tile.X, (int) tile.Y, "Diggable", "Back") != null;
      string str = location.doesTileHaveProperty((int) tile.X, (int) tile.Y, "NoSpawn", "Back");
      bool flag2 = str != null && (str.Equals("Tree") || str.Equals("All") || str.Equals("True"));
      int num1 = location is Farm ? 1 : (location.CanPlantTreesHere((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (int) tile.X, (int) tile.Y) ? 1 : 0);
      bool flag3 = location.objects.ContainsKey(tile) || location.terrainFeatures.ContainsKey(tile) && !(location.terrainFeatures[tile] is HoeDirt);
      int num2 = flag1 ? 1 : 0;
      return (num1 | num2) != 0 && !flag2 && !flag3;
    }

    public virtual bool IsSprinkler() => this.GetBaseRadiusForSprinkler() >= 0;

    public virtual int GetModifiedRadiusForSprinkler()
    {
      int radiusForSprinkler = this.GetBaseRadiusForSprinkler();
      if (radiusForSprinkler < 0)
        return -1;
      if (this.heldObject.Value != null && Utility.IsNormalObjectAtParentSheetIndex((Item) (Object) (NetFieldBase<Object, NetRef<Object>>) this.heldObject, 915))
        ++radiusForSprinkler;
      return radiusForSprinkler;
    }

    public virtual int GetBaseRadiusForSprinkler()
    {
      if (Utility.IsNormalObjectAtParentSheetIndex((Item) this, 599))
        return 0;
      if (Utility.IsNormalObjectAtParentSheetIndex((Item) this, 621))
        return 1;
      return Utility.IsNormalObjectAtParentSheetIndex((Item) this, 645) ? 2 : -1;
    }

    public virtual bool placementAction(GameLocation location, int x, int y, Farmer who = null)
    {
      Vector2 placementTile = new Vector2((float) (x / 64), (float) (y / 64));
      this.health = 10;
      if (who != null)
        this.owner.Value = who.UniqueMultiplayerID;
      else
        this.owner.Value = Game1.player.UniqueMultiplayerID;
      if (!(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && !(this is Furniture))
      {
        if (this.IsSprinkler() && location.doesTileHavePropertyNoNull((int) placementTile.X, (int) placementTile.Y, "NoSprinklers", "Back") == "T")
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:NoSprinklers"));
          return false;
        }
        switch (this.ParentSheetIndex)
        {
          case 93:
            if (location.objects.ContainsKey(placementTile))
              return false;
            location.removeLightSource((int) ((double) this.tileLocation.X * 2000.0 + (double) this.tileLocation.Y));
            location.removeLightSource((int) (long) Game1.player.uniqueMultiplayerID);
            new Torch(placementTile, 1).placementAction(location, x, y, who == null ? Game1.player : who);
            return true;
          case 94:
            if (location.objects.ContainsKey(placementTile))
              return false;
            new Torch(placementTile, 1, 94).placementAction(location, x, y, who);
            return true;
          case 286:
            foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
            {
              if (temporarySprite.position.Equals(placementTile * 64f))
                return false;
            }
            int num1 = Game1.random.Next();
            location.playSound("thudStep");
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 100f, 1, 24, placementTile * 64f, true, false, location, who)
            {
              shakeIntensity = 0.5f,
              shakeIntensityChange = 1f / 500f,
              extraInfoForEndBehavior = num1,
              endFunction = new TemporaryAnimatedSprite.endBehavior(location.removeTemporarySpritesWithID)
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 3f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = (float) num1
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 3f) * 4f, true, true, (float) (y + 7) / 10000f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 100,
              id = (float) num1
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 3f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 200,
              id = (float) num1
            });
            location.netAudio.StartPlaying("fuse");
            return true;
          case 287:
            foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
            {
              if (temporarySprite.position.Equals(placementTile * 64f))
                return false;
            }
            int num2 = Game1.random.Next();
            location.playSound("thudStep");
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 100f, 1, 24, placementTile * 64f, true, false, location, who)
            {
              shakeIntensity = 0.5f,
              shakeIntensityChange = 1f / 500f,
              extraInfoForEndBehavior = num2,
              endFunction = new TemporaryAnimatedSprite.endBehavior(location.removeTemporarySpritesWithID)
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = (float) num2
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 100,
              id = (float) num2
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 200,
              id = (float) num2
            });
            location.netAudio.StartPlaying("fuse");
            return true;
          case 288:
            foreach (TemporaryAnimatedSprite temporarySprite in location.temporarySprites)
            {
              if (temporarySprite.position.Equals(placementTile * 64f))
                return false;
            }
            int num3 = Game1.random.Next();
            location.playSound("thudStep");
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, 100f, 1, 24, placementTile * 64f, true, false, location, who)
            {
              shakeIntensity = 0.5f,
              shakeIntensityChange = 1f / 500f,
              extraInfoForEndBehavior = num3,
              endFunction = new TemporaryAnimatedSprite.endBehavior(location.removeTemporarySpritesWithID)
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 0.0f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.Yellow, 4f, 0.0f, 0.0f, 0.0f)
            {
              id = (float) num3
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 0.0f) * 4f, true, true, (float) (y + 7) / 10000f, 0.0f, Color.Orange, 4f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 100,
              id = (float) num3
            });
            Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(598, 1279, 3, 4), 53f, 5, 9, placementTile * 64f + new Vector2(5f, 0.0f) * 4f, true, false, (float) (y + 7) / 10000f, 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f)
            {
              delayBeforeAnimationStart = 200,
              id = (float) num3
            });
            location.netAudio.StartPlaying("fuse");
            return true;
          case 292:
          case 309:
          case 310:
          case 311:
          case 891:
            if (!this.canPlaceWildTreeSeed(location, placementTile))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13021"));
              return false;
            }
            Vector2 key1 = new Vector2();
            for (int index1 = x / 64 - 2; index1 <= x / 64 + 2; ++index1)
            {
              for (int index2 = y / 64 - 2; index2 <= y / 64 + 2; ++index2)
              {
                key1.X = (float) index1;
                key1.Y = (float) index2;
                if (location.terrainFeatures.ContainsKey(key1) && location.terrainFeatures[key1] is FruitTree)
                {
                  Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060_Fruit"));
                  return false;
                }
              }
            }
            int which = 1;
            switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
            {
              case 292:
                which = 8;
                break;
              case 310:
                which = 2;
                break;
              case 311:
                which = 3;
                break;
              case 891:
                which = 7;
                break;
            }
            location.terrainFeatures.Remove(placementTile);
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Tree(which, 0));
            location.playSound("dirtyHit");
            return true;
          case 293:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(10));
            location.playSound("thudStep");
            return true;
          case 297:
            if (location.objects.ContainsKey(placementTile) || location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Grass(1, 4));
            location.playSound("dirtyHit");
            return true;
          case 298:
            if (location.objects.ContainsKey(placementTile))
              return false;
            location.objects.Add(placementTile, (Object) new Fence(placementTile, 5, false));
            location.playSound("axe");
            return true;
          case 322:
            if (location.objects.ContainsKey(placementTile))
              return false;
            location.objects.Add(placementTile, (Object) new Fence(placementTile, 1, false));
            location.playSound("axe");
            return true;
          case 323:
            if (location.objects.ContainsKey(placementTile))
              return false;
            location.objects.Add(placementTile, (Object) new Fence(placementTile, 2, false));
            location.playSound("stoneStep");
            return true;
          case 324:
            if (location.objects.ContainsKey(placementTile))
              return false;
            location.objects.Add(placementTile, (Object) new Fence(placementTile, 3, false));
            location.playSound("hammer");
            return true;
          case 325:
            if (location.objects.ContainsKey(placementTile))
              return false;
            location.objects.Add(placementTile, (Object) new Fence(placementTile, 4, true));
            location.playSound("axe");
            return true;
          case 328:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(0));
            location.playSound("axchop");
            return true;
          case 329:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(1));
            location.playSound("thudStep");
            return true;
          case 331:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(2));
            location.playSound("axchop");
            return true;
          case 333:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(3));
            location.playSound("thudStep");
            return true;
          case 401:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(4));
            location.playSound("thudStep");
            return true;
          case 405:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(6));
            location.playSound("woodyStep");
            return true;
          case 407:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(5));
            location.playSound("dirtyHit");
            return true;
          case 409:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(7));
            location.playSound("stoneStep");
            return true;
          case 411:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(8));
            location.playSound("stoneStep");
            return true;
          case 415:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(9));
            location.playSound("stoneStep");
            return true;
          case 710:
            if (!CrabPot.IsValidCrabPotLocationTile(location, (int) placementTile.X, (int) placementTile.Y))
              return false;
            new CrabPot(placementTile).placementAction(location, x, y, who);
            return true;
          case 805:
            return location.terrainFeatures.ContainsKey(placementTile) && location.terrainFeatures[placementTile] is Tree && (location.terrainFeatures[placementTile] as Tree).fertilize(location);
          case 840:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(11));
            location.playSound("stoneStep");
            return true;
          case 841:
            if (location.terrainFeatures.ContainsKey(placementTile))
              return false;
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Flooring(12));
            location.playSound("stoneStep");
            return true;
          case 926:
            if (location.objects.ContainsKey(placementTile) || location.terrainFeatures.ContainsKey(placementTile))
              return false;
            OverlaidDictionary objects = location.objects;
            Vector2 key2 = placementTile;
            Torch torch = new Torch(placementTile, 278, true);
            torch.Fragility = 1;
            torch.destroyOvernight = true;
            objects.Add(key2, (Object) torch);
            Utility.addSmokePuff(location, new Vector2((float) x, (float) y));
            Utility.addSmokePuff(location, new Vector2((float) (x + 16), (float) (y + 16)));
            Utility.addSmokePuff(location, new Vector2((float) (x + 32), (float) y));
            Utility.addSmokePuff(location, new Vector2((float) (x + 48), (float) (y + 16)));
            Utility.addSmokePuff(location, new Vector2((float) (x + 32), (float) (y + 32)));
            Game1.playSound("fireball");
            return true;
        }
      }
      else
      {
        switch (this.ParentSheetIndex)
        {
          case 37:
          case 38:
          case 39:
            if (location.objects.ContainsKey(placementTile))
              return false;
            location.objects.Add(placementTile, (Object) new Sign(placementTile, this.ParentSheetIndex));
            location.playSound("axe");
            return true;
          case 62:
            location.objects.Add(placementTile, (Object) new IndoorPot(placementTile));
            break;
          case 71:
            if (location is MineShaft)
            {
              if ((location as MineShaft).shouldCreateLadderOnThisLevel() && (location as MineShaft).recursiveTryToCreateLadderDown(placementTile))
              {
                ++MineShaft.numberOfCraftedStairsUsedThisRun;
                return true;
              }
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
            }
            return false;
          case 105:
          case 264:
            if (location.terrainFeatures.ContainsKey(placementTile) && location.terrainFeatures[placementTile] is Tree)
            {
              Tree terrainFeature = location.terrainFeatures[placementTile] as Tree;
              if ((int) (NetFieldBase<int, NetInt>) terrainFeature.growthStage >= 5 && !(bool) (NetFieldBase<bool, NetBool>) terrainFeature.stump && !location.objects.ContainsKey(placementTile))
              {
                Object one = (Object) this.getOne();
                one.heldObject.Value = (Object) null;
                one.tileLocation.Value = placementTile;
                location.objects.Add(placementTile, one);
                terrainFeature.tapped.Value = true;
                terrainFeature.UpdateTapperProduct(one);
                location.playSound("axe");
                return true;
              }
            }
            return false;
          case 130:
          case 232:
            if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
              return false;
            }
            OverlaidDictionary objects1 = location.objects;
            Vector2 key3 = placementTile;
            Chest chest1 = new Chest(true, placementTile, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);
            chest1.shakeTimer = 50;
            objects1.Add(key3, (Object) chest1);
            location.playSound((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 130 ? "axe" : "hammer");
            return true;
          case 143:
          case 144:
          case 145:
          case 146:
          case 147:
          case 148:
          case 149:
          case 150:
          case 151:
            if (location.objects.ContainsKey(placementTile))
              return false;
            Torch torch1 = new Torch(placementTile, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, true);
            torch1.shakeTimer = 25;
            torch1.placementAction(location, x, y, who);
            return true;
          case 163:
            location.objects.Add(placementTile, (Object) new Cask(placementTile));
            location.playSound("hammer");
            break;
          case 165:
            Object object1 = new Object(placementTile, 165);
            object1.heldObject.Value = (Object) new Chest();
            location.objects.Add(placementTile, object1);
            location.playSound("axe");
            return true;
          case 208:
            location.objects.Add(placementTile, (Object) new Workbench(placementTile));
            location.playSound("axe");
            return true;
          case 209:
            if (!(this is MiniJukebox miniJukebox))
              miniJukebox = new MiniJukebox(placementTile);
            location.objects.Add(placementTile, (Object) miniJukebox);
            miniJukebox.RegisterToLocation(location);
            location.playSound("hammer");
            return true;
          case 211:
            if (!(this is WoodChipper woodChipper))
              woodChipper = new WoodChipper(placementTile);
            woodChipper.placementAction(location, x, y, (Farmer) null);
            location.objects.Add(placementTile, (Object) woodChipper);
            location.playSound("hammer");
            return true;
          case 214:
            if (!(this is Phone phone))
              phone = new Phone(placementTile);
            location.objects.Add(placementTile, (Object) phone);
            location.playSound("hammer");
            return true;
          case 216:
            if (location.objects.ContainsKey(placementTile))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
              return false;
            }
            if (!(location is FarmHouse) && !(location is IslandFarmHouse))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
              return false;
            }
            if (location is FarmHouse && (location as FarmHouse).upgradeLevel < 1)
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:MiniFridge_NoKitchen"));
              return false;
            }
            Chest chest2 = new Chest(216, placementTile, 217, 2);
            chest2.shakeTimer = 50;
            Chest chest3 = chest2;
            chest3.fridge.Value = true;
            location.objects.Add(placementTile, (Object) chest3);
            location.playSound("hammer");
            return true;
          case 238:
            if (!(location is Farm))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceOnFarm"));
              return false;
            }
            Vector2 vector2_1 = Vector2.Zero;
            Vector2 vector2_2 = Vector2.Zero;
            foreach (KeyValuePair<Vector2, Object> pair in location.objects.Pairs)
            {
              if ((bool) (NetFieldBase<bool, NetBool>) pair.Value.bigCraftable && pair.Value.ParentSheetIndex == 238)
              {
                if (vector2_1.Equals(Vector2.Zero))
                  vector2_1 = pair.Key;
                else if (vector2_2.Equals(Vector2.Zero))
                {
                  vector2_2 = pair.Key;
                  break;
                }
              }
            }
            if (!vector2_1.Equals(Vector2.Zero) && !vector2_2.Equals(Vector2.Zero))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:OnlyPlaceTwo"));
              return false;
            }
            break;
          case 248:
            if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
              return false;
            }
            OverlaidDictionary objects2 = location.objects;
            Vector2 key4 = placementTile;
            Chest chest4 = new Chest(true, placementTile, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);
            chest4.shakeTimer = 50;
            chest4.SpecialChestType = Chest.SpecialChestTypes.MiniShippingBin;
            objects2.Add(key4, (Object) chest4);
            location.playSound("axe");
            return true;
          case 254:
            if (!(location is AnimalHouse) || !(location as AnimalHouse).name.Contains("Barn"))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MustBePlacedInBarn"));
              return false;
            }
            break;
          case 256:
            if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
              return false;
            }
            OverlaidDictionary objects3 = location.objects;
            Vector2 key5 = placementTile;
            Chest chest5 = new Chest(true, placementTile, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);
            chest5.shakeTimer = 50;
            chest5.SpecialChestType = Chest.SpecialChestTypes.JunimoChest;
            objects3.Add(key5, (Object) chest5);
            location.playSound("axe");
            return true;
          case 275:
            if (location.objects.ContainsKey(placementTile) || location is MineShaft || location is VolcanoDungeon)
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
              return false;
            }
            Chest chest6 = new Chest(true, placementTile, (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);
            chest6.shakeTimer = 50;
            chest6.SpecialChestType = Chest.SpecialChestTypes.AutoLoader;
            Chest chest7 = chest6;
            chest7.lidFrameCount.Value = 2;
            location.objects.Add(placementTile, (Object) chest7);
            location.playSound("axe");
            return true;
        }
      }
      if (this.Category == -19 && location.terrainFeatures.ContainsKey(placementTile) && location.terrainFeatures[placementTile] is HoeDirt && (location.terrainFeatures[placementTile] as HoeDirt).crop != null && (this.ParentSheetIndex == 369 || this.ParentSheetIndex == 368) && (int) (NetFieldBase<int, NetInt>) (location.terrainFeatures[placementTile] as HoeDirt).crop.currentPhase != 0)
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13916"));
        return false;
      }
      if (this.isSapling())
      {
        if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 251)
        {
          Vector2 key6 = new Vector2();
          for (int index3 = x / 64 - 2; index3 <= x / 64 + 2; ++index3)
          {
            for (int index4 = y / 64 - 2; index4 <= y / 64 + 2; ++index4)
            {
              key6.X = (float) index3;
              key6.Y = (float) index4;
              if (location.terrainFeatures.ContainsKey(key6) && (location.terrainFeatures[key6] is Tree || location.terrainFeatures[key6] is FruitTree))
              {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13060"));
                return false;
              }
            }
          }
          if (FruitTree.IsGrowthBlocked(new Vector2((float) (x / 64), (float) (y / 64)), location))
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:FruitTree_PlacementWarning", (object) this.DisplayName));
            return false;
          }
        }
        if (location.terrainFeatures.ContainsKey(placementTile))
        {
          if (!(location.terrainFeatures[placementTile] is HoeDirt) || (location.terrainFeatures[placementTile] as HoeDirt).crop != null)
            return false;
          location.terrainFeatures.Remove(placementTile);
        }
        if (location is Farm && (location.doesTileHaveProperty((int) placementTile.X, (int) placementTile.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int) placementTile.X, (int) placementTile.Y, "Type", "Back").Equals("Grass") || location.doesTileHavePropertyNoNull((int) placementTile.X, (int) placementTile.Y, "Type", "Back").Equals("Dirt")) && !location.doesTileHavePropertyNoNull((int) placementTile.X, (int) placementTile.Y, "NoSpawn", "Back").Equals("Tree") || location.CanPlantTreesHere((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (int) placementTile.X, (int) placementTile.Y) && (location.doesTileHaveProperty((int) placementTile.X, (int) placementTile.Y, "Diggable", "Back") != null || location.doesTileHavePropertyNoNull((int) placementTile.X, (int) placementTile.Y, "Type", "Back").Equals("Stone")))
        {
          location.playSound("dirtyHit");
          DelayedAction.playSoundAfterDelay("coin", 100);
          if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 251)
          {
            location.terrainFeatures.Add(placementTile, (TerrainFeature) new Bush(placementTile, 3, location));
            return true;
          }
          bool flag = location.IsGreenhouse || ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 69 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 835) && location is IslandWest;
          location.terrainFeatures.Add(placementTile, (TerrainFeature) new FruitTree((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
          {
            GreenHouseTree = flag,
            GreenHouseTileTree = location.doesTileHavePropertyNoNull((int) placementTile.X, (int) placementTile.Y, "Type", "Back").Equals("Stone")
          });
          return true;
        }
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13068"));
        return false;
      }
      if (this.Category == -74 || this.Category == -19)
      {
        if (!location.terrainFeatures.ContainsKey(placementTile) || !(location.terrainFeatures[placementTile] is HoeDirt) || !((HoeDirt) location.terrainFeatures[placementTile]).canPlantThisSeedHere(who.ActiveObject.ParentSheetIndex, (int) placementTile.X, (int) placementTile.Y, who.ActiveObject.Category == -19) || !((HoeDirt) location.terrainFeatures[placementTile]).plant(who.ActiveObject.ParentSheetIndex, (int) placementTile.X, (int) placementTile.Y, who, who.ActiveObject.Category == -19, location) || !who.IsLocalPlayer)
          return false;
        if (this.Category == -74)
        {
          foreach (Object object2 in location.Objects.Values)
          {
            if (object2.IsSprinkler() && object2.heldObject.Value != null && object2.heldObject.Value.ParentSheetIndex == 913 && object2.IsInSprinklerRangeBroadphase(placementTile) && object2.GetSprinklerTiles().Contains(placementTile))
            {
              Chest chest = object2.heldObject.Value.heldObject.Value as Chest;
              if (chest != null && chest.items.Count > 0 && chest.items[0] != null && !chest.GetMutex().IsLocked())
              {
                chest.GetMutex().RequestLock((Action) (() =>
                {
                  if (chest.items.Count > 0 && chest.items[0] != null)
                  {
                    Item obj = chest.items[0];
                    if (obj.Category == -19 && ((HoeDirt) location.terrainFeatures[placementTile]).plant(obj.ParentSheetIndex, (int) placementTile.X, (int) placementTile.Y, who, true, location))
                    {
                      --obj.Stack;
                      if (obj.Stack <= 0)
                        chest.items[0] = (Item) null;
                    }
                  }
                  chest.GetMutex().ReleaseLock();
                }));
                break;
              }
            }
          }
        }
        Game1.haltAfterCheck = false;
        return true;
      }
      if (!this.performDropDownAction(who))
      {
        Object object3 = (Object) this.getOne();
        bool flag = false;
        if (object3.GetType() == typeof (Furniture) && Furniture.GetFurnitureInstance((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, new Vector2?(new Vector2((float) (x / 64), (float) (y / 64)))).GetType() != object3.GetType())
        {
          object3 = (Object) new StorageFurniture((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, new Vector2((float) (x / 64), (float) (y / 64)));
          (object3 as Furniture).currentRotation.Value = (this as Furniture).currentRotation.Value;
          (object3 as Furniture).updateRotation();
          flag = true;
        }
        object3.shakeTimer = 50;
        object3.tileLocation.Value = placementTile;
        object3.performDropDownAction(who);
        if (object3.name.Contains("Seasonal"))
        {
          int num = object3.ParentSheetIndex - object3.ParentSheetIndex % 4;
          object3.ParentSheetIndex = num + Utility.getSeasonNumber(Game1.currentSeason);
        }
        if (location.objects.ContainsKey(placementTile))
        {
          if (location.objects[placementTile].ParentSheetIndex != (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
          {
            Game1.createItemDebris((Item) location.objects[placementTile], placementTile * 64f, Game1.random.Next(4));
            location.objects[placementTile] = object3;
          }
        }
        else if (object3 is Furniture)
        {
          if (flag)
            location.furniture.Add(object3 as Furniture);
          else
            location.furniture.Add(this as Furniture);
        }
        else
          location.objects.Add(placementTile, object3);
        object3.initializeLightSource(placementTile);
      }
      location.playSound("woodyStep");
      return true;
    }

    public override bool actionWhenPurchased()
    {
      if (this.type.Value != null && this.type.Contains("Blueprint"))
      {
        string str = this.name.Substring(this.name.IndexOf(' ') + 1);
        if (!Game1.player.blueprints.Contains(this.name))
          Game1.player.blueprints.Add(str);
        return true;
      }
      if (Utility.IsNormalObjectAtParentSheetIndex((Item) this, 434))
      {
        if (!Game1.isFestival())
          Game1.player.mailReceived.Add("CF_Sewer");
        else
          Game1.player.mailReceived.Add("CF_Fair");
        Game1.exitActiveMenu();
        Game1.player.eatObject(this, true);
      }
      return base.actionWhenPurchased() || (bool) (NetFieldBase<bool, NetBool>) this.isRecipe;
    }

    public override bool canBePlacedInWater() => (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 710;

    public virtual bool needsToBeDonated() => !(bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && (NetFieldBase<string, NetString>) this.type != (NetString) null && (this.type.Equals((object) "Minerals") || this.type.Equals((object) "Arch")) && !(Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum).museumAlreadyHasArtifact((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex);

    public override string getDescription()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.isRecipe)
        return this.Category == -7 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13073", (object) this.loadDisplayName()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13074", (object) this.loadDisplayName());
      if (this.needsToBeDonated())
        return Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13078"), Game1.smallFont, this.getDescriptionWidth());
      return (bool) (NetFieldBase<bool, NetBool>) this.bigCraftable && !Game1.bigCraftablesInformation.ContainsKey((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex) ? "" : Game1.parseText((bool) (NetFieldBase<bool, NetBool>) this.bigCraftable ? Game1.bigCraftablesInformation[(int) (NetFieldBase<int, NetInt>) this.parentSheetIndex].Split('/')[4] : (Game1.objectInformation.ContainsKey((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex) ? Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.parentSheetIndex].Split('/')[5] : "???"), Game1.smallFont, this.getDescriptionWidth());
    }

    public virtual int sellToStorePrice(long specificPlayerID = -1)
    {
      if (this is Fence)
        return (int) (NetFieldBase<int, NetInt>) this.price;
      if (this.Category == -22)
        return (int) ((double) (int) (NetFieldBase<int, NetInt>) this.price * (1.0 + (double) (int) (NetFieldBase<int, NetInt>) this.quality * 0.25) * (((double) (FishingRod.maxTackleUses - this.uses.Value) + 0.0) / (double) FishingRod.maxTackleUses));
      float storePrice = this.getPriceAfterMultipliers((float) (int) ((double) (int) (NetFieldBase<int, NetInt>) this.price * (1.0 + (double) this.Quality * 0.25)), specificPlayerID);
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 493)
        storePrice /= 2f;
      if ((double) storePrice > 0.0)
        storePrice = Math.Max(1f, storePrice * Game1.MasterPlayer.difficultyModifier);
      return (int) storePrice;
    }

    public override int salePrice()
    {
      if (this is Fence)
        return (int) (NetFieldBase<int, NetInt>) this.price;
      if ((bool) (NetFieldBase<bool, NetBool>) this.isRecipe)
        return (int) (NetFieldBase<int, NetInt>) this.price * 10;
      switch ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex)
      {
        case 378:
          return Game1.year <= 1 ? 80 : 160;
        case 380:
          return Game1.year <= 1 ? 150 : 250;
        case 382:
          return Game1.year <= 1 ? 120 : 250;
        case 384:
          return Game1.year <= 1 ? 350 : 750;
        case 388:
          return Game1.year <= 1 ? 10 : 50;
        case 390:
          return Game1.year <= 1 ? 20 : 100;
        default:
          float num = (float) (int) ((double) ((int) (NetFieldBase<int, NetInt>) this.price * 2) * (1.0 + (double) (int) (NetFieldBase<int, NetInt>) this.quality * 0.25));
          if ((int) (NetFieldBase<int, NetInt>) this.category == -74 || this.isSapling())
            num = (float) (int) Math.Max(1f, num * Game1.MasterPlayer.difficultyModifier);
          return (int) num;
      }
    }

    protected virtual float getPriceAfterMultipliers(float startPrice, long specificPlayerID = -1)
    {
      bool flag = false;
      if (this.name != null && (this.name.ToLower().Contains("mayonnaise") || this.name.ToLower().Contains("cheese") || this.name.ToLower().Contains("cloth") || this.name.ToLower().Contains("wool")))
        flag = true;
      float val1 = 1f;
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (Game1.player.useSeparateWallets)
        {
          if (specificPlayerID == -1L)
          {
            if (allFarmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID || !allFarmer.isActive())
              continue;
          }
          else if (allFarmer.UniqueMultiplayerID != specificPlayerID)
            continue;
        }
        else if (!allFarmer.isActive())
          continue;
        float val2 = 1f;
        if (allFarmer.professions.Contains(0) && (flag || this.Category == -5 || this.Category == -6 || this.Category == -18))
          val2 *= 1.2f;
        if (allFarmer.professions.Contains(1) && (this.Category == -75 || this.Category == -80 || this.Category == -79 && !(bool) (NetFieldBase<bool, NetBool>) this.isSpawnedObject))
          val2 *= 1.1f;
        if (allFarmer.professions.Contains(4) && this.Category == -26)
          val2 *= 1.4f;
        if (allFarmer.professions.Contains(6) && this.Category == -4)
          val2 *= allFarmer.professions.Contains(8) ? 1.5f : 1.25f;
        if (allFarmer.professions.Contains(12) && (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 388)
        {
          int parentSheetIndex = (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex;
        }
        if (allFarmer.professions.Contains(15) && this.Category == -27)
          val2 *= 1.25f;
        if (allFarmer.professions.Contains(20) && ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex >= 334 && (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex <= 337 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 910))
          val2 *= 1.5f;
        if (allFarmer.professions.Contains(23) && (this.Category == -2 || this.Category == -12))
          val2 *= 1.3f;
        if (allFarmer.eventsSeen.Contains(2120303) && ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 296 || (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 410))
          val2 *= 3f;
        if (allFarmer.eventsSeen.Contains(3910979) && (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 399)
          val2 *= 5f;
        val1 = Math.Max(val1, val2);
      }
      return startPrice * val1;
    }

    public enum PreserveType
    {
      Wine,
      Jelly,
      Pickle,
      Juice,
      Roe,
      AgedRoe,
    }

    public enum HoneyType
    {
      Wild = -1, // 0xFFFFFFFF
      Poppy = 376, // 0x00000178
      Tulip = 591, // 0x0000024F
      SummerSpangle = 593, // 0x00000251
      FairyRose = 595, // 0x00000253
      BlueJazz = 597, // 0x00000255
    }
  }
}
