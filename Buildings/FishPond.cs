// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.FishPond
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Buildings
{
  public class FishPond : Building
  {
    public const int MAXIMUM_OCCUPANCY = 10;
    public static readonly float FISHING_MILLISECONDS = 1000f;
    public static readonly int HARVEST_BASE_EXP = 10;
    public static readonly float HARVEST_OUTPUT_EXP_MULTIPLIER = 0.04f;
    public static readonly int QUEST_BASE_EXP = 20;
    public static readonly float QUEST_SPAWNRATE_EXP_MULTIPIER = 5f;
    public const int NUMBER_OF_NETTING_STYLE_TYPES = 4;
    public readonly NetInt fishType = new NetInt(-1);
    public readonly NetInt lastUnlockedPopulationGate = new NetInt(0);
    public readonly NetBool hasCompletedRequest = new NetBool(false);
    public readonly NetRef<StardewValley.Object> sign = new NetRef<StardewValley.Object>();
    public readonly NetColor overrideWaterColor = new NetColor(Color.White);
    public readonly NetRef<Item> output = new NetRef<Item>();
    public readonly NetRef<StardewValley.Object> neededItem = new NetRef<StardewValley.Object>();
    public readonly NetIntDelta neededItemCount = new NetIntDelta(0);
    public readonly NetInt daysSinceSpawn = new NetInt(0);
    public readonly NetInt nettingStyle = new NetInt(0);
    public readonly NetInt seedOffset = new NetInt(0);
    public readonly NetBool hasSpawnedFish = new NetBool(false);
    [XmlIgnore]
    public readonly NetMutex needsMutex = new NetMutex();
    [XmlIgnore]
    protected bool _hasAnimatedSpawnedFish;
    [XmlIgnore]
    protected float _delayUntilFishSilhouetteAdded;
    [XmlIgnore]
    protected int _numberOfFishToJump;
    [XmlIgnore]
    protected float _timeUntilFishHop;
    [XmlIgnore]
    protected StardewValley.Object _fishObject;
    [XmlIgnore]
    public List<PondFishSilhouette> _fishSilhouettes = new List<PondFishSilhouette>();
    [XmlIgnore]
    public List<JumpingFish> _jumpingFish = new List<JumpingFish>();
    [XmlIgnore]
    private readonly NetEvent0 animateHappyFishEvent = new NetEvent0();
    [XmlIgnore]
    public List<TemporaryAnimatedSprite> animations = new List<TemporaryAnimatedSprite>();
    [XmlIgnore]
    protected FishPondData _fishPondData;

    public FishPond(BluePrint b, Vector2 tileLocation)
      : base(b, tileLocation)
    {
      this.UpdateMaximumOccupancy();
      this.fadeWhenPlayerIsBehind.Value = false;
      this.Reseed();
      this._fishSilhouettes = new List<PondFishSilhouette>();
      this._jumpingFish = new List<JumpingFish>();
    }

    public FishPond()
    {
      this._fishSilhouettes = new List<PondFishSilhouette>();
      this._jumpingFish = new List<JumpingFish>();
    }

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.fishType, (INetSerializable) this.output, (INetSerializable) this.daysSinceSpawn, (INetSerializable) this.lastUnlockedPopulationGate, (INetSerializable) this.animateHappyFishEvent, (INetSerializable) this.hasCompletedRequest, (INetSerializable) this.neededItem, (INetSerializable) this.seedOffset, (INetSerializable) this.hasSpawnedFish, (INetSerializable) this.needsMutex.NetFields, (INetSerializable) this.neededItemCount, (INetSerializable) this.overrideWaterColor, (INetSerializable) this.sign, (INetSerializable) this.nettingStyle);
      this.animateHappyFishEvent.onEvent += new NetEvent0.Event(this.AnimateHappyFish);
      this.fishType.fieldChangeVisibleEvent += new NetFieldBase<int, NetInt>.FieldChange(this.OnFishTypeChanged);
    }

    public virtual void OnFishTypeChanged(NetInt field, int old_value, int new_value)
    {
      this._fishSilhouettes.Clear();
      this._jumpingFish.Clear();
      this._fishObject = (StardewValley.Object) null;
    }

    public virtual void Reseed() => this.seedOffset.Value = DateTime.UtcNow.Millisecond;

    public List<PondFishSilhouette> GetFishSilhouettes() => this._fishSilhouettes;

    public void UpdateMaximumOccupancy()
    {
      this.GetFishPondData();
      if (this._fishPondData == null)
        return;
      for (int index = 1; index <= 10; ++index)
      {
        if (index <= this.lastUnlockedPopulationGate.Value)
        {
          this.maxOccupants.Set(index);
        }
        else
        {
          if (this._fishPondData.PopulationGates != null && this._fishPondData.PopulationGates.ContainsKey(index))
            break;
          this.maxOccupants.Set(index);
        }
      }
    }

    public FishPondData GetFishPondData()
    {
      if (this.fishType.Value <= 0)
        return (FishPondData) null;
      List<FishPondData> fishPondDataList = Game1.content.Load<List<FishPondData>>("Data\\FishPondData");
      StardewValley.Object fishObject = this.GetFishObject();
      foreach (FishPondData fishPondData in fishPondDataList)
      {
        bool flag = false;
        foreach (string requiredTag in fishPondData.RequiredTags)
        {
          if (!fishObject.HasContextTag(requiredTag))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          this._fishPondData = fishPondData;
          if (this._fishPondData.SpawnTime == -1)
          {
            int price = fishObject.Price;
            this._fishPondData.SpawnTime = price > 30 ? (price > 80 ? (price > 120 ? (price > 250 ? 5 : 4) : 3) : 2) : 1;
          }
          return this._fishPondData;
        }
      }
      return (FishPondData) null;
    }

    public StardewValley.Object GetFishProduce(Random random = null)
    {
      if (random == null)
        random = Game1.random;
      this.GetFishPondData();
      if (this._fishPondData != null)
      {
        foreach (FishPondReward producedItem in this._fishPondData.ProducedItems)
        {
          if (this.currentOccupants.Value >= producedItem.RequiredPopulation && random.NextDouble() <= (double) producedItem.Chance)
          {
            StardewValley.Object fishProduce = new StardewValley.Object(producedItem.ItemID, random.Next(producedItem.MinQuantity, producedItem.MaxQuantity + 1));
            if ((int) (NetFieldBase<int, NetInt>) fishProduce.parentSheetIndex == 812)
            {
              Color? nullable = TailoringMenu.GetDyeColor((Item) this.GetFishObject());
              if (!nullable.HasValue)
                nullable = new Color?(Color.Orange);
              if (this.fishType.Value == 698)
                nullable = new Color?(new Color(61, 55, 42));
              fishProduce = (StardewValley.Object) new ColoredObject(producedItem.ItemID, random.Next(producedItem.MinQuantity, producedItem.MaxQuantity + 1), nullable.Value);
              fishProduce.name = Game1.objectInformation[this.fishType.Value].Split('/')[0] + " Roe";
              fishProduce.preserve.Value = new StardewValley.Object.PreserveType?(StardewValley.Object.PreserveType.Roe);
              fishProduce.preservedParentSheetIndex.Value = this.fishType.Value;
              fishProduce.Price += Convert.ToInt32(Game1.objectInformation[this.fishType.Value].Split('/')[1]) / 2;
            }
            return fishProduce;
          }
        }
      }
      return (StardewValley.Object) null;
    }

    public int FishCount => this.currentOccupants.Value;

    public Item ItemWanted => (Item) null;

    private Item CreateFishInstance() => (Item) new StardewValley.Object((int) (NetFieldBase<int, NetInt>) this.fishType, 1);

    public override bool doAction(Vector2 tileLocation, Farmer who)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft <= 0 && (double) tileLocation.X >= (double) (int) (NetFieldBase<int, NetInt>) this.tileX && (double) tileLocation.X < (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide) && (double) tileLocation.Y >= (double) (int) (NetFieldBase<int, NetInt>) this.tileY && (double) tileLocation.Y < (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh))
      {
        if (who.isMoving())
          Game1.haltAfterCheck = false;
        if (who.ActiveObject != null && this.performActiveObjectDropInAction(who, false))
          return true;
        if (this.output.Value != null)
        {
          Item obj = this.output.Value;
          this.output.Value = (Item) null;
          if (who.addItemToInventoryBool(obj))
          {
            Game1.playSound("coin");
            int num = 0;
            if (obj != null && obj is StardewValley.Object)
              num = (int) ((double) (obj as StardewValley.Object).sellToStorePrice() * (double) StardewValley.Buildings.FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER);
            who.gainExperience(1, num + StardewValley.Buildings.FishPond.HARVEST_BASE_EXP);
          }
          else
          {
            this.output.Value = obj;
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
          }
          return true;
        }
        if (who.ActiveObject != null && this.HasUnresolvedNeeds() && !who.ActiveObject.bigCraftable.Value && who.ActiveObject.ParentSheetIndex == this.neededItem.Value.ParentSheetIndex)
        {
          if (this.neededItemCount.Value == 1)
            this.showObjectThrownIntoPondAnimation(who, who.ActiveObject, (DelayedAction.delayedBehavior) (() =>
            {
              if (this.neededItemCount.Value > 0)
                return;
              Game1.playSound("jingle1");
            }));
          else
            this.showObjectThrownIntoPondAnimation(who, who.ActiveObject);
          who.reduceActiveItemByOne();
          if (who == Game1.player)
          {
            --this.neededItemCount.Value;
            if (this.neededItemCount.Value <= 0)
            {
              this.needsMutex.RequestLock((Action) (() =>
              {
                this.needsMutex.ReleaseLock();
                this.ResolveNeeds(who);
              }));
              this.neededItemCount.Value = -1;
            }
          }
          if (this.neededItemCount.Value <= 0)
            this.animateHappyFishEvent.Fire();
          return true;
        }
        if (who.ActiveObject != null && (who.ActiveObject.Category == -4 || who.ActiveObject.ParentSheetIndex == 393 || who.ActiveObject.ParentSheetIndex == 397))
        {
          if (this.fishType.Value >= 0)
          {
            if (!this.isLegalFishForPonds((int) (NetFieldBase<int, NetInt>) this.fishType))
            {
              string displayName = who.ActiveObject.DisplayName;
              Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:CantPutInPonds", (object) displayName.ToLower()));
              return true;
            }
            if (who.ActiveObject.ParentSheetIndex != (int) (NetFieldBase<int, NetInt>) this.fishType)
            {
              string displayName = who.ActiveObject.DisplayName;
              if (who.ActiveObject.ParentSheetIndex == 393 || who.ActiveObject.ParentSheetIndex == 397)
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:WrongFishTypeCoral", (object) displayName));
              else if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.de)
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:WrongFishType", (object) displayName, (object) Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.fishType].Split('/')[4]));
              else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:WrongFishType", (object) displayName.ToLower(), (object) Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.fishType].Split('/')[4].ToLower()));
              return true;
            }
            if ((int) (NetFieldBase<int, NetInt>) this.currentOccupants < (int) (NetFieldBase<int, NetInt>) this.maxOccupants)
              return this.addFishToPond(who, who.ActiveObject);
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:PondFull"));
            return true;
          }
          if (this.isLegalFishForPonds(who.ActiveObject.ParentSheetIndex))
            return this.addFishToPond(who, who.ActiveObject);
          string displayName1 = who.ActiveObject.DisplayName;
          if (who.ActiveObject.HasContextTag("fish_legendary"))
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:CantPutInPonds", (object) displayName1));
          else
            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:CantPutInPonds", (object) displayName1));
          return true;
        }
        if ((int) (NetFieldBase<int, NetInt>) this.fishType >= 0)
        {
          if (Game1.didPlayerJustRightClick(true))
          {
            Game1.playSound("bigSelect");
            Game1.activeClickableMenu = (IClickableMenu) new PondQueryMenu(this);
            return true;
          }
        }
        else if (Game1.didPlayerJustRightClick(true))
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:NoFish"));
          return true;
        }
      }
      return base.doAction(tileLocation, who);
    }

    public void AnimateHappyFish()
    {
      this._numberOfFishToJump = this.currentOccupants.Value;
      this._timeUntilFishHop = 1f;
    }

    public Vector2 GetItemBucketTile() => new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 4), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 4));

    public Vector2 GetRequestTile() => new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 2), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 2));

    public Vector2 GetCenterTile() => new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX + 2), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + 2));

    public void ResolveNeeds(Farmer who)
    {
      this.Reseed();
      this.hasCompletedRequest.Value = true;
      this.lastUnlockedPopulationGate.Value = this.maxOccupants.Value + 1;
      this.UpdateMaximumOccupancy();
      this.daysSinceSpawn.Value = 0;
      int num = 0;
      FishPondData fishPondData = this.GetFishPondData();
      if (fishPondData != null)
        num = (int) ((double) fishPondData.SpawnTime * (double) StardewValley.Buildings.FishPond.QUEST_SPAWNRATE_EXP_MULTIPIER);
      who.gainExperience(1, num + StardewValley.Buildings.FishPond.QUEST_BASE_EXP);
      Random r = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + (int) (NetFieldBase<int, NetInt>) this.seedOffset);
      Game1.showGlobalMessage(PondQueryMenu.getCompletedRequestString(this, this.GetFishObject(), r));
    }

    public override void resetLocalState()
    {
      base.resetLocalState();
      this._jumpingFish.Clear();
      while (this._fishSilhouettes.Count < this.currentOccupants.Value)
      {
        PondFishSilhouette pondFishSilhouette = new PondFishSilhouette(this);
        this._fishSilhouettes.Add(pondFishSilhouette);
        pondFishSilhouette.position = (this.GetCenterTile() + new Vector2(Utility.Lerp(-0.5f, 0.5f, (float) Game1.random.NextDouble()) * (float) ((int) (NetFieldBase<int, NetInt>) this.tilesWide - 2), Utility.Lerp(-0.5f, 0.5f, (float) Game1.random.NextDouble()) * (float) ((int) (NetFieldBase<int, NetInt>) this.tilesHigh - 2))) * 64f;
      }
    }

    private bool isLegalFishForPonds(int type)
    {
      List<FishPondData> fishPondDataList = Game1.content.Load<List<FishPondData>>("Data\\FishPondData");
      StardewValley.Object @object = new StardewValley.Object(type, 1);
      if (@object.HasContextTag("fish_legendary"))
        return false;
      foreach (FishPondData fishPondData in fishPondDataList)
      {
        bool flag = false;
        foreach (string requiredTag in fishPondData.RequiredTags)
        {
          if (!@object.HasContextTag(requiredTag))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return true;
      }
      return false;
    }

    private void showObjectThrownIntoPondAnimation(
      Farmer who,
      StardewValley.Object whichObject,
      DelayedAction.delayedBehavior callback = null)
    {
      who.faceGeneralDirection(this.GetCenterTile() * 64f + new Vector2(32f, 32f));
      if (who.FacingDirection == 1 || who.FacingDirection == 3)
      {
        float num1 = Vector2.Distance((Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) who.position, this.GetCenterTile() * 64f);
        float num2 = (float) ((double) this.GetCenterTile().Y * 64.0 + 32.0) - who.position.Y;
        float num3 = num1 - 8f;
        float y = 1f / 400f;
        float num4 = num3 * (float) Math.Sqrt((double) y / (2.0 * ((double) num3 + 96.0)));
        float timer = (float) (2.0 * ((double) num4 / (double) y)) + ((float) Math.Sqrt((double) num4 * (double) num4 + 2.0 * (double) y * 96.0) - num4) / y + num2;
        float num5 = 0.0f;
        if ((double) num2 > 0.0)
        {
          num5 = num2 / 832f;
          timer += num5 * 200f;
        }
        Game1.playSound("throwDownITem");
        List<TemporaryAnimatedSprite> sprites = new List<TemporaryAnimatedSprite>();
        sprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) whichObject.parentSheetIndex, 16, 16), (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) who.position + new Vector2(0.0f, -64f), false, 0.0f, Color.White)
        {
          scale = 4f,
          layerDepth = 1f,
          totalNumberOfLoops = 1,
          interval = timer,
          motion = new Vector2((who.FacingDirection == 3 ? -1f : 1f) * (num4 - num5), (float) (-(double) num4 * 3.0 / 2.0)),
          acceleration = new Vector2(0.0f, y),
          timeBasedMotion = true
        });
        sprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, this.GetCenterTile() * 64f, false, false)
        {
          delayBeforeAnimationStart = (int) timer,
          layerDepth = (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 2.0) / 10000.0)
        });
        sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 55f, 8, 0, this.GetCenterTile() * 64f, false, Game1.random.NextDouble() < 0.5, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
        {
          delayBeforeAnimationStart = (int) timer
        });
        sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 65f, 8, 0, this.GetCenterTile() * 64f + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-16, 32)), false, Game1.random.NextDouble() < 0.5, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
        {
          delayBeforeAnimationStart = (int) timer
        });
        sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 75f, 8, 0, this.GetCenterTile() * 64f + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-16, 32)), false, Game1.random.NextDouble() < 0.5, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
        {
          delayBeforeAnimationStart = (int) timer
        });
        if (who.IsLocalPlayer)
        {
          DelayedAction.playSoundAfterDelay("waterSlosh", (int) timer, who.currentLocation);
          if (callback != null)
            DelayedAction.functionAfterDelay(callback, (int) timer);
        }
        if (this.fishType.Value >= 0 && whichObject.ParentSheetIndex == this.fishType.Value)
          this._delayUntilFishSilhouetteAdded = timer / 1000f;
        Game1.multiplayer.broadcastSprites(who.currentLocation, sprites);
      }
      else
      {
        float num6 = Vector2.Distance((Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) who.position, this.GetCenterTile() * 64f);
        float num7 = Math.Abs(num6);
        if (who.FacingDirection == 0)
        {
          num6 = -num6;
          num7 += 64f;
        }
        float num8 = this.GetCenterTile().X * 64f - who.position.X;
        float y = 1f / 400f;
        float num9 = (float) Math.Sqrt(2.0 * (double) y * (double) num7);
        float num10 = (float) (Math.Sqrt(2.0 * ((double) num7 - (double) num6) / (double) y) + (double) num9 / (double) y) * 1.05f;
        float timer = (float) ((who.FacingDirection != 0 ? (double) (num10 * 2.5f) : (double) (num10 * 0.7f)) - (double) Math.Abs(num8) / (who.FacingDirection == 0 ? 100.0 : 2.0));
        Game1.playSound("throwDownITem");
        List<TemporaryAnimatedSprite> sprites = new List<TemporaryAnimatedSprite>();
        sprites.Add(new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) whichObject.parentSheetIndex, 16, 16), (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) who.position + new Vector2(0.0f, -64f), false, 0.0f, Color.White)
        {
          scale = 4f,
          layerDepth = 1f,
          totalNumberOfLoops = 1,
          interval = timer,
          motion = new Vector2(num8 / (who.FacingDirection == 0 ? 900f : 1000f), -num9),
          acceleration = new Vector2(0.0f, y),
          timeBasedMotion = true
        });
        sprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, this.GetCenterTile() * 64f, false, false)
        {
          delayBeforeAnimationStart = (int) timer,
          layerDepth = (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 2.0) / 10000.0)
        });
        sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 55f, 8, 0, this.GetCenterTile() * 64f, false, Game1.random.NextDouble() < 0.5, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
        {
          delayBeforeAnimationStart = (int) timer
        });
        sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 65f, 8, 0, this.GetCenterTile() * 64f + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-16, 32)), false, Game1.random.NextDouble() < 0.5, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
        {
          delayBeforeAnimationStart = (int) timer
        });
        sprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 75f, 8, 0, this.GetCenterTile() * 64f + new Vector2((float) Game1.random.Next(-32, 32), (float) Game1.random.Next(-16, 32)), false, Game1.random.NextDouble() < 0.5, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 1.0) / 10000.0), 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f)
        {
          delayBeforeAnimationStart = (int) timer
        });
        if (who.IsLocalPlayer)
        {
          DelayedAction.playSoundAfterDelay("waterSlosh", (int) timer, who.currentLocation);
          if (callback != null)
            DelayedAction.functionAfterDelay(callback, (int) timer);
        }
        if (this.fishType.Value >= 0 && whichObject.ParentSheetIndex == this.fishType.Value)
          this._delayUntilFishSilhouetteAdded = timer / 1000f;
        Game1.multiplayer.broadcastSprites(who.currentLocation, sprites);
      }
    }

    private bool addFishToPond(Farmer who, StardewValley.Object fish)
    {
      who.reduceActiveItemByOne();
      if ((int) (NetFieldBase<int, NetInt>) this.currentOccupants == 0)
      {
        this.fishType.Value = fish.ParentSheetIndex;
        this._fishPondData = (FishPondData) null;
        this.UpdateMaximumOccupancy();
      }
      ++this.currentOccupants.Value;
      this.showObjectThrownIntoPondAnimation(who, fish);
      return true;
    }

    public override void dayUpdate(int dayOfMonth)
    {
      this.hasSpawnedFish.Value = false;
      this._hasAnimatedSpawnedFish = false;
      if (this.hasCompletedRequest.Value)
      {
        this.neededItem.Value = (StardewValley.Object) null;
        this.neededItemCount.Set(-1);
        this.hasCompletedRequest.Value = false;
      }
      this.GetFishPondData();
      if ((int) (NetFieldBase<int, NetInt>) this.currentOccupants > 0)
      {
        Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + (int) (NetFieldBase<int, NetInt>) this.tileX * 1000 + (int) (NetFieldBase<int, NetInt>) this.tileY * 2000);
        if (random.NextDouble() < (double) Utility.Lerp(0.15f, 0.95f, (float) (int) (NetFieldBase<int, NetInt>) this.currentOccupants / 10f))
          this.output.Value = (Item) this.GetFishProduce(random);
        ++this.daysSinceSpawn.Value;
        if (this.daysSinceSpawn.Value > this.GetFishPondData().SpawnTime)
          this.daysSinceSpawn.Value = this.GetFishPondData().SpawnTime;
        if (this.daysSinceSpawn.Value >= this.GetFishPondData().SpawnTime)
        {
          KeyValuePair<int, int> neededItemData = this._GetNeededItemData();
          if (neededItemData.Key != -1)
          {
            if (this.currentOccupants.Value >= this.maxOccupants.Value && this.neededItem.Value == null)
            {
              this.neededItem.Value = new StardewValley.Object(neededItemData.Key, 1);
              this.neededItemCount.Set(neededItemData.Value);
            }
          }
          else
            this.SpawnFish();
        }
        if (this.currentOccupants.Value == 10 && (int) (NetFieldBase<int, NetInt>) this.fishType == 717)
        {
          foreach (Farmer allFarmer in Game1.getAllFarmers())
          {
            if (!allFarmer.mailReceived.Contains("FullCrabPond"))
            {
              allFarmer.mailReceived.Add("FullCrabPond");
              allFarmer.activeDialogueEvents.Add("FullCrabPond", 14);
            }
          }
        }
        this.doFishSpecificWaterColoring();
      }
      base.dayUpdate(dayOfMonth);
    }

    private void doFishSpecificWaterColoring()
    {
      this.overrideWaterColor.Value = Color.White;
      if ((int) (NetFieldBase<int, NetInt>) this.fishType == 162 && this.lastUnlockedPopulationGate.Value >= 2)
        this.overrideWaterColor.Value = new Color(250, 30, 30);
      else if ((int) (NetFieldBase<int, NetInt>) this.fishType == 796 && (int) (NetFieldBase<int, NetInt>) this.currentOccupants > 2)
        this.overrideWaterColor.Value = new Color(60, (int) byte.MaxValue, 60);
      else if ((int) (NetFieldBase<int, NetInt>) this.fishType == 795 && (int) (NetFieldBase<int, NetInt>) this.currentOccupants > 2)
      {
        this.overrideWaterColor.Value = new Color(120, 20, 110);
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.fishType != 155 || (int) (NetFieldBase<int, NetInt>) this.currentOccupants <= 2)
          return;
        this.overrideWaterColor.Value = new Color(150, 100, 200);
      }
    }

    public bool JumpFish()
    {
      if (this._fishSilhouettes.Count == 0)
        return false;
      PondFishSilhouette random = Utility.GetRandom<PondFishSilhouette>(this._fishSilhouettes);
      this._fishSilhouettes.Remove(random);
      this._jumpingFish.Add(new JumpingFish(this, random.position, (this.GetCenterTile() + new Vector2(0.5f, 0.5f)) * 64f));
      return true;
    }

    public void SpawnFish()
    {
      if (this.currentOccupants.Value >= this.maxOccupants.Value || this.currentOccupants.Value <= 0)
        return;
      this.hasSpawnedFish.Value = true;
      this.daysSinceSpawn.Value = 0;
      ++this.currentOccupants.Value;
      if (this.currentOccupants.Value <= this.maxOccupants.Value)
        return;
      this.currentOccupants.Value = this.maxOccupants.Value;
    }

    public override bool performActiveObjectDropInAction(Farmer who, bool probe)
    {
      if (who.ActiveObject == null || !(bool) (NetFieldBase<bool, NetBool>) who.ActiveObject.bigCraftable || !who.ActiveObject.Name.Contains("Sign") || this.sign.Value != null && !((NetFieldBase<int, NetInt>) who.ActiveObject.parentSheetIndex != this.sign.Value.parentSheetIndex))
        return base.performActiveObjectDropInAction(who, probe);
      if (probe)
        return true;
      StardewValley.Object @object = this.sign.Value;
      this.sign.Value = (StardewValley.Object) who.ActiveObject.getOne();
      who.reduceActiveItemByOne();
      if (@object != null)
        Game1.createItemDebris((Item) @object, new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX + 0.5f, (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh)) * 64f, 3, who.currentLocation);
      who.currentLocation.playSound("axe");
      return true;
    }

    public override void performToolAction(Tool t, int tileX, int tileY)
    {
      switch (t)
      {
        case Axe _:
        case Pickaxe _:
          if (this.sign.Value != null)
          {
            if (t.getLastFarmerToUse() != null)
              Game1.createItemDebris((Item) (StardewValley.Object) (NetFieldBase<StardewValley.Object, NetRef<StardewValley.Object>>) this.sign, new Vector2((float) (int) (NetFieldBase<int, NetInt>) this.tileX + 0.5f, (float) ((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh)) * 64f, 3, t.getLastFarmerToUse().currentLocation);
            this.sign.Value = (StardewValley.Object) null;
            t.getLastFarmerToUse().currentLocation.playSound("hammer");
            break;
          }
          break;
      }
      base.performToolAction(t, tileX, tileY);
    }

    public override void performActionOnConstruction(GameLocation location)
    {
      base.performActionOnConstruction(location);
      this.nettingStyle.Value = ((int) (NetFieldBase<int, NetInt>) this.tileX / 3 + (int) (NetFieldBase<int, NetInt>) this.tileY / 3) % 3;
    }

    public override void performActionOnBuildingPlacement()
    {
      base.performActionOnBuildingPlacement();
      this.nettingStyle.Value = ((int) (NetFieldBase<int, NetInt>) this.tileX / 3 + (int) (NetFieldBase<int, NetInt>) this.tileY / 3) % 3;
    }

    public bool HasUnresolvedNeeds() => this.neededItem.Value != null && this._GetNeededItemData().Key != -1 && !this.hasCompletedRequest.Value;

    private KeyValuePair<int, int> _GetNeededItemData()
    {
      if (this.currentOccupants.Value < (int) (NetFieldBase<int, NetInt>) this.maxOccupants)
        return new KeyValuePair<int, int>(-1, 0);
      this.GetFishPondData();
      if (this._fishPondData.PopulationGates != null)
      {
        if (this.maxOccupants.Value + 1 <= this.lastUnlockedPopulationGate.Value)
          return new KeyValuePair<int, int>(-1, 0);
        if (this._fishPondData.PopulationGates.ContainsKey(this.maxOccupants.Value + 1))
        {
          Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + (int) (NetFieldBase<int, NetInt>) this.tileX * 1000 + (int) (NetFieldBase<int, NetInt>) this.tileY * 2000);
          int key = -1;
          int num = 1;
          string[] strArray = Utility.GetRandom<string>(this._fishPondData.PopulationGates[this.maxOccupants.Value + 1], random).Split(' ');
          if (strArray.Length >= 1)
            key = Convert.ToInt32(strArray[0]);
          if (strArray.Length >= 3)
            num = random.Next(Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]) + 1);
          else if (strArray.Length >= 2)
            num = Convert.ToInt32(strArray[1]);
          return new KeyValuePair<int, int>(key, num);
        }
      }
      return new KeyValuePair<int, int>(-1, 0);
    }

    public void ClearPond()
    {
      this._hasAnimatedSpawnedFish = false;
      this.hasSpawnedFish.Value = false;
      this._fishSilhouettes.Clear();
      this._jumpingFish.Clear();
      this._fishObject = (StardewValley.Object) null;
      this.currentOccupants.Value = 0;
      this.daysSinceSpawn.Value = 0;
      this.neededItem.Value = (StardewValley.Object) null;
      this.neededItemCount.Value = -1;
      this.lastUnlockedPopulationGate.Value = 0;
      this.fishType.Value = -1;
      this.Reseed();
      this.overrideWaterColor.Value = Color.White;
    }

    public StardewValley.Object CatchFish()
    {
      if (this.currentOccupants.Value == 0)
        return (StardewValley.Object) null;
      --this.currentOccupants.Value;
      return (StardewValley.Object) this.CreateFishInstance();
    }

    public StardewValley.Object GetFishObject()
    {
      if (this._fishObject == null)
        this._fishObject = new StardewValley.Object(this.fishType.Value, 1);
      return this._fishObject;
    }

    public override void Update(GameTime time)
    {
      this.needsMutex.Update((GameLocation) Game1.getFarm());
      this.animateHappyFishEvent.Poll();
      if (!this._hasAnimatedSpawnedFish && this.hasSpawnedFish.Value && this._numberOfFishToJump <= 0 && Utility.isOnScreen((this.GetCenterTile() + new Vector2(0.5f, 0.5f)) * 64f, 64))
      {
        this._hasAnimatedSpawnedFish = true;
        if (this.fishType.Value != 393 && this.fishType.Value != 397)
        {
          this._numberOfFishToJump = 1;
          this._timeUntilFishHop = Utility.RandomFloat(2f, 5f);
        }
      }
      TimeSpan elapsedGameTime;
      if ((double) this._delayUntilFishSilhouetteAdded > 0.0)
      {
        double fishSilhouetteAdded = (double) this._delayUntilFishSilhouetteAdded;
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds = elapsedGameTime.TotalSeconds;
        this._delayUntilFishSilhouetteAdded = (float) (fishSilhouetteAdded - totalSeconds);
        if ((double) this._delayUntilFishSilhouetteAdded < 0.0)
          this._delayUntilFishSilhouetteAdded = 0.0f;
      }
      if (this._numberOfFishToJump > 0 && (double) this._timeUntilFishHop > 0.0)
      {
        double timeUntilFishHop = (double) this._timeUntilFishHop;
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds = elapsedGameTime.TotalSeconds;
        this._timeUntilFishHop = (float) (timeUntilFishHop - totalSeconds);
        if ((double) this._timeUntilFishHop <= 0.0 && this.JumpFish())
        {
          --this._numberOfFishToJump;
          this._timeUntilFishHop = Utility.RandomFloat(0.15f, 0.25f);
        }
      }
      while (this._fishSilhouettes.Count > this.currentOccupants.Value - this._jumpingFish.Count)
        this._fishSilhouettes.RemoveAt(0);
      if ((double) this._delayUntilFishSilhouetteAdded <= 0.0)
      {
        while (this._fishSilhouettes.Count < this.currentOccupants.Value - this._jumpingFish.Count)
          this._fishSilhouettes.Add(new PondFishSilhouette(this));
      }
      for (int index = 0; index < this._fishSilhouettes.Count; ++index)
      {
        PondFishSilhouette fishSilhouette = this._fishSilhouettes[index];
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds = elapsedGameTime.TotalSeconds;
        fishSilhouette.Update((float) totalSeconds);
      }
      for (int index = 0; index < this._jumpingFish.Count; ++index)
      {
        JumpingFish jumpingFish = this._jumpingFish[index];
        elapsedGameTime = time.ElapsedGameTime;
        double totalSeconds = elapsedGameTime.TotalSeconds;
        if (jumpingFish.Update((float) totalSeconds))
        {
          this._fishSilhouettes.Add(new PondFishSilhouette(this)
          {
            position = this._jumpingFish[index].position
          });
          this._jumpingFish.RemoveAt(index);
          --index;
        }
      }
      base.Update(time);
    }

    public override bool isTileFishable(Vector2 tile) => (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft <= 0 && (double) tile.X > (double) (int) (NetFieldBase<int, NetInt>) this.tileX && (double) tile.X < (double) ((int) (NetFieldBase<int, NetInt>) this.tileX + (int) (NetFieldBase<int, NetInt>) this.tilesWide - 1) && (double) tile.Y > (double) (int) (NetFieldBase<int, NetInt>) this.tileY && (double) tile.Y < (double) ((int) (NetFieldBase<int, NetInt>) this.tileY + (int) (NetFieldBase<int, NetInt>) this.tilesHigh - 1);

    public override bool CanRefillWateringCan() => (int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft <= 0;

    public override Microsoft.Xna.Framework.Rectangle getSourceRectForMenu() => new Microsoft.Xna.Framework.Rectangle(0, 0, 80, 80);

    public override void drawInMenu(SpriteBatch b, int x, int y)
    {
      y += 32;
      this.drawShadow(b, x, y);
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 80, 80, 80)), new Color(60, 126, 150) * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 1f);
      for (int tileY = (int) (NetFieldBase<int, NetInt>) this.tileY; tileY < (int) (NetFieldBase<int, NetInt>) this.tileY + 5; ++tileY)
      {
        for (int tileX = (int) (NetFieldBase<int, NetInt>) this.tileX; tileX < (int) (NetFieldBase<int, NetInt>) this.tileX + 4; ++tileX)
        {
          int num = tileY == (int) (NetFieldBase<int, NetInt>) this.tileY + 4 ? 1 : 0;
          bool flag = tileY == (int) (NetFieldBase<int, NetInt>) this.tileY;
          if (num != 0)
            b.Draw(Game1.mouseCursors, new Vector2((float) (x + tileX * 64 + 32), (float) (y + (tileY + 1) * 64 - (int) Game1.currentLocation.waterPosition - 32)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + ((tileX + tileY) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128)), 64, 32 + (int) Game1.currentLocation.waterPosition - 5)), (Color) (NetFieldBase<Color, NetColor>) Game1.currentLocation.waterColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
          else
            b.Draw(Game1.mouseCursors, new Vector2((float) (x + tileX * 64 + 32), (float) (y + tileY * 64 + 32 - (!flag ? (int) Game1.currentLocation.waterPosition : 0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + ((tileX + tileY) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128)) + (flag ? (int) Game1.currentLocation.waterPosition : 0), 64, 64 + (flag ? (int) -(double) Game1.currentLocation.waterPosition : 0))), (Color) (NetFieldBase<Color, NetColor>) Game1.currentLocation.waterColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
      }
      b.Draw(this.texture.Value, new Vector2((float) x, (float) y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 80, 80)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 1f);
      b.Draw(this.texture.Value, new Vector2((float) (x + 64), (float) (y + 44 + (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2500.0 < 1250.0 ? 4 : 0))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16, 160, 48, 7)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(this.texture.Value, new Vector2((float) x, (float) (y - 128)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(80, 0, 80, 48)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 0.0f), 4f, SpriteEffects.None, 1f);
    }

    public override void OnEndMove()
    {
      foreach (PondFishSilhouette fishSilhouette in this._fishSilhouettes)
        fishSilhouette.position = (this.GetCenterTile() + new Vector2(Utility.Lerp(-0.5f, 0.5f, (float) Game1.random.NextDouble()) * (float) ((int) (NetFieldBase<int, NetInt>) this.tilesWide - 2), Utility.Lerp(-0.5f, 0.5f, (float) Game1.random.NextDouble()) * (float) ((int) (NetFieldBase<int, NetInt>) this.tilesHigh - 2))) * 64f;
    }

    public override void draw(SpriteBatch b)
    {
      if (this.isMoving)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.daysOfConstructionLeft > 0)
      {
        this.drawInConstruction(b);
      }
      else
      {
        for (int index = this.animations.Count - 1; index >= 0; --index)
          this.animations[index].draw(b);
        this.drawShadow(b);
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 80, 80, 80)), (this.overrideWaterColor.Equals(Color.White) ? new Color(60, 126, 150) : (Color) (NetFieldBase<Color, NetColor>) this.overrideWaterColor) * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 80f), 4f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 - 3.0) / 10000.0));
        for (int tileY = (int) (NetFieldBase<int, NetInt>) this.tileY; tileY < (int) (NetFieldBase<int, NetInt>) this.tileY + 5; ++tileY)
        {
          for (int tileX = (int) (NetFieldBase<int, NetInt>) this.tileX; tileX < (int) (NetFieldBase<int, NetInt>) this.tileX + 4; ++tileX)
          {
            int num = tileY == (int) (NetFieldBase<int, NetInt>) this.tileY + 4 ? 1 : 0;
            bool flag = tileY == (int) (NetFieldBase<int, NetInt>) this.tileY;
            if (num != 0)
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (tileX * 64 + 32), (float) ((tileY + 1) * 64 - (int) Game1.currentLocation.waterPosition - 32))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + ((tileX + tileY) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128)), 64, 32 + (int) Game1.currentLocation.waterPosition - 5)), this.overrideWaterColor.Equals(Color.White) ? (Color) (NetFieldBase<Color, NetColor>) Game1.currentLocation.waterColor : this.overrideWaterColor.Value * 0.5f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 - 2.0) / 10000.0));
            else
              b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (tileX * 64 + 32), (float) (tileY * 64 + 32 - (!flag ? (int) Game1.currentLocation.waterPosition : 0)))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + ((tileX + tileY) % 2 == 0 ? (Game1.currentLocation.waterTileFlip ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 0 : 128)) + (flag ? (int) Game1.currentLocation.waterPosition : 0), 64, 64 + (flag ? (int) -(double) Game1.currentLocation.waterPosition : 0))), this.overrideWaterColor.Equals(Color.White) ? (Color) (NetFieldBase<Color, NetColor>) Game1.currentLocation.waterColor : this.overrideWaterColor.Value * 0.5f, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 - 2.0) / 10000.0));
          }
        }
        TimeSpan totalGameTime;
        if (this.overrideWaterColor.Value.Equals(Color.White))
        {
          SpriteBatch spriteBatch = b;
          Texture2D texture = this.texture.Value;
          xTile.Dimensions.Rectangle viewport = Game1.viewport;
          double x = (double) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 64);
          int num1 = (int) (NetFieldBase<int, NetInt>) this.tileY * 64 + 44;
          totalGameTime = Game1.currentGameTime.TotalGameTime;
          int num2 = totalGameTime.TotalMilliseconds % 2500.0 < 1250.0 ? 4 : 0;
          double y = (double) (num1 + num2);
          Vector2 globalPosition = new Vector2((float) x, (float) y);
          Vector2 local = Game1.GlobalToLocal(viewport, globalPosition);
          Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16, 160, 48, 7));
          Color color = this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha;
          Vector2 zero = Vector2.Zero;
          double layerDepth = (((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 1.0) / 10000.0;
          spriteBatch.Draw(texture, local, sourceRectangle, color, 0.0f, zero, 4f, SpriteEffects.None, (float) layerDepth);
        }
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 80, 80)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 80f), 4f, SpriteEffects.None, (float) (((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 / 10000.0));
        if (this.nettingStyle.Value < 3)
          b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 - 128))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(80, (int) (NetFieldBase<int, NetInt>) this.nettingStyle * 48, 80, 48)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, new Vector2(0.0f, 80f), 4f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 2.0) / 10000.0));
        if (this.sign.Value != null)
        {
          b.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 8), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 - 128 - 32))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.sign.Value.parentSheetIndex, 16, 32)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 2.0) / 10000.0));
          if (this.fishType.Value != -1)
          {
            b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 8 + 8 - 4), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 - 128 - 8 + 4))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.fishType.Value, 16, 16)), Color.Black * 0.4f * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 3.0) / 10000.0));
            b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 8 + 8 - 1), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 - 128 - 8 + 1))), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.fishType.Value, 16, 16)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 4.0) / 10000.0));
            Utility.drawTinyDigits(this.currentOccupants.Value, b, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 32 + 8 + (this.currentOccupants.Value < 10 ? 8 : 0)), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + (int) (NetFieldBase<int, NetInt>) this.tilesHigh * 64 - 96))), 3f, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 5.0) / 10000.0), Color.LightYellow * (float) (NetFieldBase<float, NetFloat>) this.alpha);
          }
        }
        if (this._fishObject != null && ((int) (NetFieldBase<int, NetInt>) this._fishObject.parentSheetIndex == 393 || (int) (NetFieldBase<int, NetInt>) this._fishObject.parentSheetIndex == 397))
        {
          for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.currentOccupants; ++index)
          {
            Vector2 vector2 = Vector2.Zero;
            int num = (index + this.seedOffset.Value) % 10;
            switch (num)
            {
              case 0:
                vector2 = new Vector2(0.0f, 0.0f);
                break;
              case 1:
                vector2 = new Vector2(48f, 32f);
                break;
              case 2:
                vector2 = new Vector2(80f, 72f);
                break;
              case 3:
                vector2 = new Vector2(140f, 28f);
                break;
              case 4:
                vector2 = new Vector2(96f, 0.0f);
                break;
              case 5:
                vector2 = new Vector2(0.0f, 96f);
                break;
              case 6:
                vector2 = new Vector2(140f, 80f);
                break;
              case 7:
                vector2 = new Vector2(64f, 120f);
                break;
              case 8:
                vector2 = new Vector2(140f, 140f);
                break;
              case 9:
                vector2 = new Vector2(0.0f, 150f);
                break;
            }
            b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 64 + 7), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + 64 + 32)) + vector2), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 3f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 - 2.0) / 10000.0 - 1.10000000859145E-05));
            b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64 + 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64 + 64)) + vector2), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.fishType.Value, 16, 16)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha * 0.75f, 0.0f, Vector2.Zero, 3f, num % 3 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 - 2.0) / 10000.0 - 9.99999974737875E-06));
          }
        }
        else
        {
          for (int index = 0; index < this._fishSilhouettes.Count; ++index)
            this._fishSilhouettes[index].Draw(b);
        }
        for (int index = 0; index < this._jumpingFish.Count; ++index)
          this._jumpingFish[index].Draw(b);
        if (this.HasUnresolvedNeeds())
        {
          Vector2 globalPosition = this.GetRequestTile() * 64f + 64f * new Vector2(0.5f, 0.5f);
          totalGameTime = Game1.currentGameTime.TotalGameTime;
          float num = (float) (3.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2));
          float layerDepth = (float) (((double) globalPosition.Y + 160.0) / 10000.0 + 9.99999997475243E-07);
          globalPosition.Y += num - 32f;
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(403, 496, 5, 14)), Color.White * 0.75f, 0.0f, new Vector2(2f, 14f), 4f, SpriteEffects.None, layerDepth);
        }
        if (this.output.Value == null)
          return;
        b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((int) (NetFieldBase<int, NetInt>) this.tileX * 64), (float) ((int) (NetFieldBase<int, NetInt>) this.tileY * 64)) + new Vector2(65f, 59f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 160, 15, 16)), this.color.Value * (float) (NetFieldBase<float, NetFloat>) this.alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((((double) (int) (NetFieldBase<int, NetInt>) this.tileY + 0.5) * 64.0 + 1.0) / 10000.0));
        Vector2 vector2_1 = this.GetItemBucketTile() * 64f;
        totalGameTime = Game1.currentGameTime.TotalGameTime;
        Vector2 globalPosition1 = vector2_1 + new Vector2(0.0f, -2f) * 64f + new Vector2(0.0f, (float) (4.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2)));
        Vector2 vector2_2 = new Vector2(40f, 36f);
        float layerDepth1 = (float) (((double) vector2_1.Y + 64.0) / 10000.0 + 9.99999997475243E-07);
        float layerDepth2 = (float) (((double) vector2_1.Y + 64.0) / 10000.0 + 9.99999974737875E-06);
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, globalPosition1), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth1);
        b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, globalPosition1 + vector2_2), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.output.Value.parentSheetIndex, 16, 16)), Color.White * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, layerDepth2);
        if (!(this.output.Value is ColoredObject))
          return;
        b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, globalPosition1 + vector2_2), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.output.Value.parentSheetIndex + 1, 16, 16)), (this.output.Value as ColoredObject).color.Value * 0.75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, layerDepth2 + 1E-05f);
      }
    }
  }
}
