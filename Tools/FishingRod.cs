// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.FishingRod
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Tools
{
  public class FishingRod : Tool
  {
    public const int sizeOfLandCheckRectangle = 11;
    [XmlElement("bobber")]
    public readonly NetPosition bobber = new NetPosition();
    public static int minFishingBiteTime = 600;
    public static int maxFishingBiteTime = 30000;
    public static int minTimeToNibble = 340;
    public static int maxTimeToNibble = 800;
    public static int maxTackleUses = 20;
    protected Vector2 _lastAppliedMotion = Vector2.Zero;
    protected Vector2[] _totalMotionBuffer = new Vector2[4];
    protected int _totalMotionBufferIndex;
    protected NetVector2 _totalMotion = new NetVector2(Vector2.Zero);
    public static double baseChanceForTreasure = 0.15;
    private int bobberBob;
    [XmlIgnore]
    public float bobberTimeAccumulator;
    [XmlIgnore]
    public float timePerBobberBob = 2000f;
    [XmlIgnore]
    public float timeUntilFishingBite = -1f;
    [XmlIgnore]
    public float fishingBiteAccumulator;
    [XmlIgnore]
    public float fishingNibbleAccumulator;
    [XmlIgnore]
    public float timeUntilFishingNibbleDone = -1f;
    [XmlIgnore]
    public float castingPower;
    [XmlIgnore]
    public float castingChosenCountdown;
    [XmlIgnore]
    public float castingTimerSpeed = 1f / 1000f;
    [XmlIgnore]
    public float fishWiggle;
    [XmlIgnore]
    public float fishWiggleIntensity;
    [XmlIgnore]
    public bool isFishing;
    [XmlIgnore]
    public bool hit;
    [XmlIgnore]
    public bool isNibbling;
    [XmlIgnore]
    public bool favBait;
    [XmlIgnore]
    public bool isTimingCast;
    [XmlIgnore]
    public bool isCasting;
    [XmlIgnore]
    public bool castedButBobberStillInAir;
    [XmlIgnore]
    protected bool _hasPlayerAdjustedBobber;
    private bool lastCatchWasJunk;
    [XmlIgnore]
    public bool doneWithAnimation;
    [XmlIgnore]
    public bool pullingOutOfWater;
    [XmlIgnore]
    public bool isReeling;
    [XmlIgnore]
    public bool hasDoneFucntionYet;
    [XmlIgnore]
    public bool fishCaught;
    [XmlIgnore]
    public bool recordSize;
    [XmlIgnore]
    public bool treasureCaught;
    [XmlIgnore]
    public bool showingTreasure;
    [XmlIgnore]
    public bool hadBobber;
    [XmlIgnore]
    public bool bossFish;
    [XmlIgnore]
    public bool fromFishPond;
    [XmlIgnore]
    public bool caughtDoubleFish;
    [XmlIgnore]
    public List<TemporaryAnimatedSprite> animations = new List<TemporaryAnimatedSprite>();
    [XmlIgnore]
    public SparklingText sparklingText;
    [XmlIgnore]
    private int fishSize;
    [XmlIgnore]
    private int whichFish;
    [XmlIgnore]
    private int fishQuality;
    [XmlIgnore]
    private int clearWaterDistance;
    [XmlIgnore]
    private int originalFacingDirection;
    [XmlIgnore]
    private string itemCategory;
    [XmlIgnore]
    private int recastTimerMs;
    protected const int RECAST_DELAY_MS = 200;
    [XmlIgnore]
    private readonly NetEventBinary pullFishFromWaterEvent = new NetEventBinary();
    [XmlIgnore]
    private readonly NetEvent1Field<bool, NetBool> doneFishingEvent = new NetEvent1Field<bool, NetBool>();
    [XmlIgnore]
    private readonly NetEvent0 startCastingEvent = new NetEvent0();
    [XmlIgnore]
    private readonly NetEvent0 castingEndEnableMovementEvent = new NetEvent0();
    [XmlIgnore]
    private readonly NetEvent0 putAwayEvent = new NetEvent0();
    [XmlIgnore]
    private readonly NetEvent0 beginReelingEvent = new NetEvent0();
    public static ICue chargeSound;
    public static ICue reelSound;
    private bool usedGamePadToCast;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.bobber.NetFields, (INetSerializable) this.pullFishFromWaterEvent, (INetSerializable) this.doneFishingEvent, (INetSerializable) this.startCastingEvent, (INetSerializable) this.castingEndEnableMovementEvent, (INetSerializable) this.putAwayEvent, (INetSerializable) this._totalMotion, (INetSerializable) this.beginReelingEvent);
      this._totalMotion.InterpolationEnabled = false;
      this._totalMotion.InterpolationWait = false;
      this.pullFishFromWaterEvent.AddReaderHandler(new Action<BinaryReader>(this.doPullFishFromWater));
      this.doneFishingEvent.onEvent += new AbstractNetEvent1<bool>.Event(this.doDoneFishing);
      this.startCastingEvent.onEvent += new NetEvent0.Event(this.doStartCasting);
      this.castingEndEnableMovementEvent.onEvent += new NetEvent0.Event(this.doCastingEndEnableMovement);
      this.beginReelingEvent.onEvent += new NetEvent0.Event(this.beginReeling);
      this.putAwayEvent.onEvent += new NetEvent0.Event(((Item) this).resetState);
    }

    public override void actionWhenStopBeingHeld(Farmer who)
    {
      this.putAwayEvent.Fire();
      base.actionWhenStopBeingHeld(who);
    }

    public FishingRod()
      : base("Fishing Rod", 0, 189, 8, false)
    {
      this.numAttachmentSlots.Value = 2;
      this.attachments.SetCount((int) (NetFieldBase<int, NetInt>) this.numAttachmentSlots);
      this.IndexOfMenuItemView = 8 + (int) (NetFieldBase<int, NetInt>) this.upgradeLevel;
    }

    public override void resetState()
    {
      this.isNibbling = false;
      this.fishCaught = false;
      this.isFishing = false;
      this.isReeling = false;
      this.isCasting = false;
      this.isTimingCast = false;
      this.doneWithAnimation = false;
      this.pullingOutOfWater = false;
      this.fromFishPond = false;
      this.caughtDoubleFish = false;
      this.fishingBiteAccumulator = 0.0f;
      this.showingTreasure = false;
      this.fishingNibbleAccumulator = 0.0f;
      this.timeUntilFishingBite = -1f;
      this.timeUntilFishingNibbleDone = -1f;
      this.bobberTimeAccumulator = 0.0f;
      this.castingChosenCountdown = 0.0f;
      this._totalMotionBufferIndex = 0;
      for (int index = 0; index < this._totalMotionBuffer.Length; ++index)
        this._totalMotionBuffer[index] = Vector2.Zero;
      if (this.lastUser != null && Game1.player == this.lastUser)
      {
        for (int index = Game1.screenOverlayTempSprites.Count - 1; index >= 0; --index)
        {
          if ((double) Game1.screenOverlayTempSprites[index].id == 987654336.0)
            Game1.screenOverlayTempSprites.RemoveAt(index);
        }
      }
      this._totalMotion.Value = Vector2.Zero;
      this._lastAppliedMotion = Vector2.Zero;
      this.pullFishFromWaterEvent.Clear();
      this.doneFishingEvent.Clear();
      this.startCastingEvent.Clear();
      this.castingEndEnableMovementEvent.Clear();
      this.beginReelingEvent.Clear();
      this.bobber.Set(Vector2.Zero);
    }

    public override Item getOne()
    {
      FishingRod destination = new FishingRod();
      destination.UpgradeLevel = this.UpgradeLevel;
      destination.numAttachmentSlots.Value = this.numAttachmentSlots.Value;
      destination.IndexOfMenuItemView = this.IndexOfMenuItemView;
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14041");

    protected override string loadDescription() => (int) (NetFieldBase<int, NetInt>) this.upgradeLevel != 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14042") : Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.trainingRodDescription");

    public override int salePrice()
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel)
      {
        case 0:
          return 500;
        case 1:
          return 2000;
        case 2:
          return 5000;
        case 3:
          return 15000;
        default:
          return 500;
      }
    }

    public override int attachmentSlots()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel > 2)
        return 2;
      return (int) (NetFieldBase<int, NetInt>) this.upgradeLevel <= 1 ? 0 : 1;
    }

    public FishingRod(int upgradeLevel)
      : base("Fishing Rod", upgradeLevel, 189, 8, false)
    {
      this.numAttachmentSlots.Value = 2;
      this.attachments.SetCount((int) (NetFieldBase<int, NetInt>) this.numAttachmentSlots);
      this.IndexOfMenuItemView = 8 + upgradeLevel;
      this.UpgradeLevel = upgradeLevel;
    }

    public override string DisplayName
    {
      get
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel)
        {
          case 0:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14045");
          case 1:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14046");
          case 2:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14047");
          case 3:
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14048");
          default:
            return this.displayName;
        }
      }
    }

    public override string Name
    {
      get
      {
        switch ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel)
        {
          case 0:
            return "Bamboo Pole";
          case 1:
            return "Training Rod";
          case 2:
            return "Fiberglass Rod";
          case 3:
            return "Iridium Rod";
          default:
            return this.BaseName;
        }
      }
      set => this.BaseName = value;
    }

    private int getAddedDistance(Farmer who)
    {
      if (who.FishingLevel >= 15)
        return 4;
      if (who.FishingLevel >= 8)
        return 3;
      if (who.FishingLevel >= 4)
        return 2;
      return who.FishingLevel >= 1 ? 1 : 0;
    }

    private Vector2 calculateBobberTile() => (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber with
    {
      X = this.bobber.X / 64f,
      Y = this.bobber.Y / 64f
    };

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      if (this.fishCaught || !who.IsLocalPlayer && (this.isReeling || this.isFishing || this.pullingOutOfWater))
        return;
      this.hasDoneFucntionYet = true;
      Vector2 bobberTile = this.calculateBobberTile();
      int x1 = (int) bobberTile.X;
      int y1 = (int) bobberTile.Y;
      base.DoFunction(location, x, y, power, who);
      if (this.doneWithAnimation)
        who.canReleaseTool = true;
      if (Game1.isAnyGamePadButtonBeingPressed())
        Game1.lastCursorMotionWasMouse = false;
      if (!this.isFishing && !this.castedButBobberStillInAir && !this.pullingOutOfWater && !this.isNibbling && !this.hit && !this.showingTreasure)
      {
        if (!Game1.eventUp && who.IsLocalPlayer && !this.hasEnchantmentOfType<EfficientToolEnchantment>())
        {
          float stamina = who.Stamina;
          who.Stamina -= (float) (8.0 - (double) who.FishingLevel * 0.100000001490116);
          who.checkForExhaustion(stamina);
        }
        if (location.canFishHere() && location.isTileFishable(x1, y1))
        {
          this.clearWaterDistance = FishingRod.distanceToLand((int) ((double) this.bobber.X / 64.0), (int) ((double) this.bobber.Y / 64.0), this.lastUser.currentLocation);
          this.isFishing = true;
          location.temporarySprites.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, new Vector2(this.bobber.X - 32f, this.bobber.Y - 32f), false, false));
          if (who.IsLocalPlayer)
            location.playSound("dropItemInWater");
          this.timeUntilFishingBite = this.calculateTimeUntilFishingBite(bobberTile, true, who);
          ++Game1.stats.TimesFished;
          double num1 = ((double) this.bobber.X - 32.0) / 64.0;
          double num2 = ((double) this.bobber.Y - 32.0) / 64.0;
          if ((NetFieldBase<Point, NetPoint>) location.fishSplashPoint != (NetPoint) null && new Rectangle((int) this.bobber.X - 32, (int) this.bobber.Y - 32, 64, 64).Intersects(new Rectangle(location.fishSplashPoint.X * 64, location.fishSplashPoint.Y * 64, 64, 64)))
          {
            this.timeUntilFishingBite /= 4f;
            location.temporarySprites.Add(new TemporaryAnimatedSprite(10, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber - new Vector2(32f, 32f), Color.Cyan));
          }
          who.UsingTool = true;
          who.canMove = false;
        }
        else
        {
          if (this.doneWithAnimation)
            who.UsingTool = false;
          if (!this.doneWithAnimation)
            return;
          who.canMove = true;
        }
      }
      else
      {
        if (this.isCasting || this.pullingOutOfWater)
          return;
        bool fromFishPond = location.isTileBuildingFishable((int) bobberTile.X, (int) bobberTile.Y);
        who.FarmerSprite.PauseForSingleAnimation = false;
        switch (who.FacingDirection)
        {
          case 0:
            who.FarmerSprite.animateBackwardsOnce(299, 35f);
            break;
          case 1:
            who.FarmerSprite.animateBackwardsOnce(300, 35f);
            break;
          case 2:
            who.FarmerSprite.animateBackwardsOnce(301, 35f);
            break;
          case 3:
            who.FarmerSprite.animateBackwardsOnce(302, 35f);
            break;
        }
        if (this.isNibbling)
        {
          double num = this.attachments[0] != null ? (double) this.attachments[0].Price / 10.0 : 0.0;
          bool flag1 = false;
          if ((NetFieldBase<Point, NetPoint>) location.fishSplashPoint != (NetPoint) null)
            flag1 = new Rectangle(location.fishSplashPoint.X * 64, location.fishSplashPoint.Y * 64, 64, 64).Intersects(new Rectangle((int) this.bobber.X - 80, (int) this.bobber.Y - 80, 64, 64));
          StardewValley.Object @object = location.getFish(this.fishingNibbleAccumulator, this.attachments[0] != null ? this.attachments[0].ParentSheetIndex : -1, this.clearWaterDistance + (flag1 ? 1 : 0), this.lastUser, num + (flag1 ? 0.4 : 0.0), bobberTile);
          if (@object == null || @object.ParentSheetIndex <= 0)
            @object = new StardewValley.Object(Game1.random.Next(167, 173), 1);
          if ((double) @object.scale.X == 1.0)
            this.favBait = true;
          Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
          bool flag2 = false;
          if (@object is Furniture)
            flag2 = true;
          else if (Utility.IsNormalObjectAtParentSheetIndex((Item) @object, @object.ParentSheetIndex) && dictionary.ContainsKey(@object.ParentSheetIndex))
          {
            string[] strArray = dictionary[@object.ParentSheetIndex].Split('/');
            int result = -1;
            if (!int.TryParse(strArray[1], out result))
              flag2 = true;
          }
          else
            flag2 = true;
          this.lastCatchWasJunk = false;
          if (((@object.Category == -20 || @object.ParentSheetIndex == 152 || @object.ParentSheetIndex == 153 || (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex == 157 || (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex == 797 || (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex == 79 || (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex == 73 || @object.ParentSheetIndex == 842 || @object.ParentSheetIndex >= 820 && @object.ParentSheetIndex <= 828 || (int) (NetFieldBase<int, NetInt>) @object.parentSheetIndex == GameLocation.CAROLINES_NECKLACE_ITEM ? 1 : (@object.ParentSheetIndex == 890 ? 1 : 0)) | (fromFishPond ? 1 : 0) | (flag2 ? 1 : 0)) != 0)
          {
            this.lastCatchWasJunk = true;
            string itemCategory = "Object";
            if (@object is Furniture)
              itemCategory = "Furniture";
            this.pullFishFromWater(@object.ParentSheetIndex, -1, 0, 0, false, false, fromFishPond, itemCategory: itemCategory);
          }
          else
          {
            if (this.hit || !who.IsLocalPlayer)
              return;
            this.hit = true;
            Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber + new Vector2(-140f, -160f)), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true)
            {
              scaleChangeChange = -0.005f,
              motion = new Vector2(0.0f, -0.1f),
              endFunction = new TemporaryAnimatedSprite.endBehavior(this.startMinigameEndFunction),
              extraInfoForEndBehavior = @object.ParentSheetIndex,
              id = 9.876543E+08f
            });
            location.localSound("FishHit");
          }
        }
        else
        {
          if (fromFishPond)
          {
            StardewValley.Object fish = location.getFish(-1f, -1, -1, this.lastUser, -1.0, bobberTile);
            if (fish != null)
            {
              this.pullFishFromWater(fish.ParentSheetIndex, -1, 0, 0, false, false, fromFishPond);
              return;
            }
          }
          if (who.IsLocalPlayer)
            location.playSound("pullItemFromWater");
          this.isFishing = false;
          this.pullingOutOfWater = true;
          if (this.lastUser.FacingDirection == 1 || this.lastUser.FacingDirection == 3)
          {
            double num3 = (double) Math.Abs(this.bobber.X - (float) this.lastUser.getStandingX());
            float y2 = 0.005f;
            double num4 = (double) y2;
            float num5 = -(float) Math.Sqrt(num3 * num4 / 2.0);
            this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(170, 1903, 7, 8), (float) (2.0 * ((double) Math.Abs(num5 - 0.5f) / (double) y2)) * 1.2f, 1, 0, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber + new Vector2(-32f, -48f), false, false, (float) who.getStandingY() / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-20, 20) / 100f)
            {
              motion = new Vector2((who.FacingDirection == 3 ? -1f : 1f) * (num5 + 0.2f), num5 - 0.8f),
              acceleration = new Vector2(0.0f, y2),
              endFunction = new TemporaryAnimatedSprite.endBehavior(this.donefishingEndFunction),
              timeBasedMotion = true,
              alphaFade = 1f / 1000f
            });
          }
          else
          {
            float num6 = this.bobber.Y - (float) this.lastUser.getStandingY();
            float num7 = Math.Abs(num6 + 256f);
            float y3 = 0.005f;
            float num8 = (float) Math.Sqrt(2.0 * (double) y3 * (double) num7);
            this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(170, 1903, 7, 8), (float) (Math.Sqrt(2.0 * ((double) num7 - (double) num6) / (double) y3) + (double) num8 / (double) y3), 1, 0, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber + new Vector2(-32f, -48f), false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-20, 20) / 100f)
            {
              motion = new Vector2(0.0f, -num8),
              acceleration = new Vector2(0.0f, y3),
              endFunction = new TemporaryAnimatedSprite.endBehavior(this.donefishingEndFunction),
              timeBasedMotion = true,
              alphaFade = 1f / 1000f
            });
          }
          who.UsingTool = true;
          who.canReleaseTool = false;
        }
      }
    }

    private float calculateTimeUntilFishingBite(Vector2 bobberTile, bool isFirstCast, Farmer who)
    {
      if (Game1.currentLocation.isTileBuildingFishable((int) bobberTile.X, (int) bobberTile.Y) && Game1.currentLocation is BuildableGameLocation)
      {
        Building buildingAt = (Game1.currentLocation as BuildableGameLocation).getBuildingAt(bobberTile);
        if (buildingAt != null && buildingAt is FishPond && (int) (NetFieldBase<int, NetInt>) (buildingAt as FishPond).currentOccupants > 0)
          return FishPond.FISHING_MILLISECONDS;
      }
      float val2 = (float) Game1.random.Next(FishingRod.minFishingBiteTime, FishingRod.maxFishingBiteTime - 250 * who.FishingLevel - (this.attachments[1] == null || this.attachments[1].ParentSheetIndex != 686 ? (this.attachments[1] == null || this.attachments[1].ParentSheetIndex != 687 ? 0 : 10000) : 5000));
      if (isFirstCast)
        val2 *= 0.75f;
      if (this.attachments[0] != null)
      {
        val2 *= 0.5f;
        if ((int) (NetFieldBase<int, NetInt>) this.attachments[0].parentSheetIndex == 774)
          val2 *= 0.75f;
      }
      return Math.Max(500f, val2);
    }

    public Color getColor()
    {
      switch ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel)
      {
        case 0:
          return Color.Goldenrod;
        case 1:
          return Color.OliveDrab;
        case 2:
          return Color.White;
        case 3:
          return Color.Violet;
        default:
          return Color.White;
      }
    }

    public static int distanceToLand(int tileX, int tileY, GameLocation location)
    {
      Rectangle r = new Rectangle(tileX - 1, tileY - 1, 3, 3);
      bool flag = false;
      int num = 1;
      while (!flag && r.Width <= 11)
      {
        foreach (Vector2 position in Utility.getBorderOfThisRectangle(r))
        {
          if (location.isTileOnMap(position) && location.doesTileHaveProperty((int) position.X, (int) position.Y, "Water", "Back") == null)
          {
            flag = true;
            num = r.Width / 2;
            break;
          }
        }
        r.Inflate(1, 1);
      }
      if (r.Width > 11)
        num = 6;
      return num - 1;
    }

    public void startMinigameEndFunction(int extra)
    {
      this.beginReelingEvent.Fire();
      this.isReeling = true;
      this.hit = false;
      switch (this.lastUser.FacingDirection)
      {
        case 1:
          this.lastUser.FarmerSprite.setCurrentSingleFrame(48);
          break;
        case 3:
          this.lastUser.FarmerSprite.setCurrentSingleFrame(48, flip: true);
          break;
      }
      float num1 = 1f * ((float) this.clearWaterDistance / 5f);
      int num2 = 1 + this.lastUser.FishingLevel / 2;
      float num3 = num1 * ((float) Game1.random.Next(num2, Math.Max(6, num2)) / 5f);
      if (this.favBait)
        num3 *= 1.2f;
      float fishSize = Math.Max(0.0f, Math.Min(1f, num3 * (float) (1.0 + (double) Game1.random.Next(-10, 11) / 100.0)));
      bool treasure = !Game1.isFestival() && this.lastUser.fishCaught != null && this.lastUser.fishCaught.Count() > 1 && Game1.random.NextDouble() < FishingRod.baseChanceForTreasure + (double) this.lastUser.LuckLevel * 0.005 + (this.getBaitAttachmentIndex() == 703 ? FishingRod.baseChanceForTreasure : 0.0) + (this.getBobberAttachmentIndex() == 693 ? FishingRod.baseChanceForTreasure / 3.0 : 0.0) + this.lastUser.DailyLuck / 2.0 + (this.lastUser.professions.Contains(9) ? FishingRod.baseChanceForTreasure : 0.0);
      Game1.activeClickableMenu = (IClickableMenu) new BobberBar(extra, fishSize, treasure, this.attachments[1] != null ? this.attachments[1].ParentSheetIndex : -1);
    }

    public int getBobberAttachmentIndex() => this.attachments[1] == null ? -1 : this.attachments[1].ParentSheetIndex;

    public int getBaitAttachmentIndex() => this.attachments[0] == null ? -1 : this.attachments[0].ParentSheetIndex;

    public bool inUse() => this.isFishing || this.isCasting || this.isTimingCast || this.isNibbling || this.isReeling || this.fishCaught;

    public void donefishingEndFunction(int extra)
    {
      this.isFishing = false;
      this.isReeling = false;
      this.lastUser.canReleaseTool = true;
      this.lastUser.canMove = true;
      this.lastUser.UsingTool = false;
      this.lastUser.FarmerSprite.PauseForSingleAnimation = false;
      this.pullingOutOfWater = false;
      this.doneFishing(this.lastUser);
    }

    public static void endOfAnimationBehavior(Farmer f)
    {
    }

    public override StardewValley.Object attach(StardewValley.Object o)
    {
      if (o != null && o.Category == -21 && (int) (NetFieldBase<int, NetInt>) this.upgradeLevel > 1)
      {
        StardewValley.Object stack = this.attachments[0];
        if (stack != null && stack.canStackWith((ISalable) o))
        {
          stack.Stack = o.addToStack((Item) stack);
          if (stack.Stack <= 0)
            stack = (StardewValley.Object) null;
        }
        this.attachments[0] = o;
        Game1.playSound("button1");
        return stack;
      }
      if (o != null && o.Category == -22 && (int) (NetFieldBase<int, NetInt>) this.upgradeLevel > 2)
      {
        StardewValley.Object attachment = this.attachments[1];
        this.attachments[1] = o;
        Game1.playSound("button1");
        return attachment;
      }
      if (o == null)
      {
        if (this.attachments[0] != null)
        {
          StardewValley.Object attachment = this.attachments[0];
          this.attachments[0] = (StardewValley.Object) null;
          Game1.playSound("dwop");
          return attachment;
        }
        if (this.attachments[1] != null)
        {
          StardewValley.Object attachment = this.attachments[1];
          this.attachments[1] = (StardewValley.Object) null;
          Game1.playSound("dwop");
          return attachment;
        }
      }
      return (StardewValley.Object) null;
    }

    public override void drawAttachments(SpriteBatch b, int x, int y)
    {
      y += this.enchantments.Count<BaseEnchantment>() > 0 ? 8 : 4;
      if ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel > 1)
      {
        if (this.attachments[0] == null)
        {
          b.Draw(Game1.menuTexture, new Vector2((float) x, (float) y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 36)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
        }
        else
        {
          b.Draw(Game1.menuTexture, new Vector2((float) x, (float) y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
          this.attachments[0].drawInMenu(b, new Vector2((float) x, (float) y), 1f);
        }
      }
      if ((int) (NetFieldBase<int, NetInt>) this.upgradeLevel <= 2)
        return;
      if (this.attachments[1] == null)
      {
        b.Draw(Game1.menuTexture, new Vector2((float) x, (float) (y + 64 + 4)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 37)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
      }
      else
      {
        b.Draw(Game1.menuTexture, new Vector2((float) x, (float) (y + 64 + 4)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
        this.attachments[1].drawInMenu(b, new Vector2((float) x, (float) (y + 64 + 4)), 1f);
      }
    }

    public override bool canThisBeAttached(StardewValley.Object o)
    {
      if (o == null || o.Category == -21 && (int) (NetFieldBase<int, NetInt>) this.upgradeLevel > 1)
        return true;
      return o.Category == -22 && (int) (NetFieldBase<int, NetInt>) this.upgradeLevel > 2;
    }

    public void playerCaughtFishEndFunction(int extraData)
    {
      this.lastUser.Halt();
      this.lastUser.armOffset = Vector2.Zero;
      this.castedButBobberStillInAir = false;
      this.fishCaught = true;
      this.isReeling = false;
      this.isFishing = false;
      this.pullingOutOfWater = false;
      this.lastUser.canReleaseTool = false;
      if (!this.lastUser.IsLocalPlayer)
        return;
      if (!Game1.isFestival())
      {
        this.recordSize = this.lastUser.caughtFish(this.whichFish, this.fishSize, this.fromFishPond, this.caughtDoubleFish ? 2 : 1);
        this.lastUser.faceDirection(2);
      }
      else
      {
        Game1.currentLocation.currentEvent.caughtFish(this.whichFish, this.fishSize, this.lastUser);
        this.fishCaught = false;
        this.doneFishing(this.lastUser);
      }
      if (FishingRod.isFishBossFish(this.whichFish))
      {
        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14068"));
        string str = Game1.objectInformation[this.whichFish].Split('/')[4];
        Game1.multiplayer.globalChatInfoMessage("CaughtLegendaryFish", Game1.player.Name, str);
      }
      else if (this.recordSize)
      {
        this.sparklingText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14069"), Color.LimeGreen, Color.Azure);
        this.lastUser.currentLocation.localSound("newRecord");
      }
      else
        this.lastUser.currentLocation.localSound("fishSlap");
    }

    public static bool isFishBossFish(int index)
    {
      switch (index)
      {
        case 159:
        case 160:
        case 163:
        case 682:
        case 775:
          return true;
        default:
          return false;
      }
    }

    public void pullFishFromWater(
      int whichFish,
      int fishSize,
      int fishQuality,
      int fishDifficulty,
      bool treasureCaught,
      bool wasPerfect,
      bool fromFishPond,
      bool caughtDouble = false,
      string itemCategory = "Object")
    {
      this.pullFishFromWaterEvent.Fire((NetEventBinary.ArgWriter) (writer =>
      {
        writer.Write(whichFish);
        writer.Write(fishSize);
        writer.Write(fishQuality);
        writer.Write(fishDifficulty);
        writer.Write(treasureCaught);
        writer.Write(wasPerfect);
        writer.Write(fromFishPond);
        writer.Write(caughtDouble);
        writer.Write(itemCategory);
      }));
    }

    private void doPullFishFromWater(BinaryReader argReader)
    {
      int num1 = argReader.ReadInt32();
      int num2 = argReader.ReadInt32();
      int num3 = argReader.ReadInt32();
      int num4 = argReader.ReadInt32();
      bool flag1 = argReader.ReadBoolean();
      bool flag2 = argReader.ReadBoolean();
      bool flag3 = argReader.ReadBoolean();
      bool flag4 = argReader.ReadBoolean();
      string str = argReader.ReadString();
      this.treasureCaught = flag1;
      this.fishSize = num2;
      this.fishQuality = num3;
      this.whichFish = num1;
      this.fromFishPond = flag3;
      this.caughtDoubleFish = flag4;
      this.itemCategory = str;
      if (num3 >= 2 & flag2)
        this.fishQuality = 4;
      else if (num3 >= 1 & flag2)
        this.fishQuality = 2;
      if (this.lastUser == null)
        return;
      if (!Game1.isFestival() && this.lastUser.IsLocalPlayer && !flag3 && str == "Object")
      {
        this.bossFish = FishingRod.isFishBossFish(num1);
        int howMuch = Math.Max(1, (num3 + 1) * 3 + num4 / 3);
        if (flag1)
          howMuch += (int) ((double) howMuch * 1.20000004768372);
        if (flag2)
          howMuch += (int) ((double) howMuch * 1.39999997615814);
        if (this.bossFish)
          howMuch *= 5;
        this.lastUser.gainExperience(1, howMuch);
      }
      if (this.fishQuality < 0)
        this.fishQuality = 0;
      Rectangle sourceRect = new Rectangle();
      string textureName;
      if (str == "Object")
      {
        textureName = "Maps\\springobjects";
        sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, num1, 16, 16);
      }
      else
      {
        textureName = "LooseSprites\\Cursors";
        sourceRect = new Rectangle(228, 408, 16, 16);
      }
      float animationInterval;
      if (this.lastUser.FacingDirection == 1 || this.lastUser.FacingDirection == 3)
      {
        float num5 = Vector2.Distance((Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber, this.lastUser.Position);
        float y1 = 1f / 1000f;
        float num6 = (float) (128.0 - ((double) this.lastUser.Position.Y - (double) this.bobber.Y + 10.0));
        double a1 = 4.0 * Math.PI / 11.0;
        float f1 = (float) ((double) num5 * (double) y1 * Math.Tan(a1) / Math.Sqrt(2.0 * (double) num5 * (double) y1 * Math.Tan(a1) - 2.0 * (double) y1 * (double) num6));
        if (float.IsNaN(f1))
          f1 = 0.6f;
        float num7 = f1 * (float) (1.0 / Math.Tan(a1));
        animationInterval = num5 / num7;
        this.animations.Add(new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, 1, 0, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber, false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2((this.lastUser.FacingDirection == 3 ? -1f : 1f) * -num7, -f1),
          acceleration = new Vector2(0.0f, y1),
          timeBasedMotion = true,
          endFunction = new TemporaryAnimatedSprite.endBehavior(this.playerCaughtFishEndFunction),
          extraInfoForEndBehavior = num1,
          endSound = "tinyWhip"
        });
        if (this.caughtDoubleFish)
        {
          float num8 = Vector2.Distance((Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber, this.lastUser.Position);
          float y2 = 0.0008f;
          float num9 = (float) (128.0 - ((double) this.lastUser.Position.Y - (double) this.bobber.Y + 10.0));
          double a2 = 4.0 * Math.PI / 11.0;
          float f2 = (float) ((double) num8 * (double) y2 * Math.Tan(a2) / Math.Sqrt(2.0 * (double) num8 * (double) y2 * Math.Tan(a2) - 2.0 * (double) y2 * (double) num9));
          if (float.IsNaN(f2))
            f2 = 0.6f;
          float num10 = f2 * (float) (1.0 / Math.Tan(a2));
          animationInterval = num8 / num10;
          this.animations.Add(new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, 1, 0, (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber, false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2((this.lastUser.FacingDirection == 3 ? -1f : 1f) * -num10, -f2),
            acceleration = new Vector2(0.0f, y2),
            timeBasedMotion = true,
            endSound = "fishSlap",
            Parent = this.lastUser.currentLocation
          });
        }
      }
      else
      {
        float num11 = this.bobber.Y - (float) (this.lastUser.getStandingY() - 64);
        float num12 = Math.Abs((float) ((double) num11 + 256.0 + 32.0));
        if (this.lastUser.FacingDirection == 0)
          num12 += 96f;
        float y3 = 3f / 1000f;
        float num13 = (float) Math.Sqrt(2.0 * (double) y3 * (double) num12);
        animationInterval = (float) (Math.Sqrt(2.0 * ((double) num12 - (double) num11) / (double) y3) + (double) num13 / (double) y3);
        float x1 = 0.0f;
        if ((double) animationInterval != 0.0)
          x1 = (this.lastUser.Position.X - this.bobber.X) / animationInterval;
        this.animations.Add(new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, 1, 0, new Vector2(this.bobber.X, this.bobber.Y), false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(x1, -num13),
          acceleration = new Vector2(0.0f, y3),
          timeBasedMotion = true,
          endFunction = new TemporaryAnimatedSprite.endBehavior(this.playerCaughtFishEndFunction),
          extraInfoForEndBehavior = num1,
          endSound = "tinyWhip"
        });
        if (this.caughtDoubleFish)
        {
          float num14 = this.bobber.Y - (float) (this.lastUser.getStandingY() - 64);
          float num15 = Math.Abs((float) ((double) num14 + 256.0 + 32.0));
          if (this.lastUser.FacingDirection == 0)
            num15 += 96f;
          float y4 = 0.004f;
          float num16 = (float) Math.Sqrt(2.0 * (double) y4 * (double) num15);
          animationInterval = (float) (Math.Sqrt(2.0 * ((double) num15 - (double) num14) / (double) y4) + (double) num16 / (double) y4);
          float x2 = 0.0f;
          if ((double) animationInterval != 0.0)
            x2 = (this.lastUser.Position.X - this.bobber.X) / animationInterval;
          this.animations.Add(new TemporaryAnimatedSprite(textureName, sourceRect, animationInterval, 1, 0, new Vector2(this.bobber.X, this.bobber.Y), false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(x2, -num16),
            acceleration = new Vector2(0.0f, y4),
            timeBasedMotion = true,
            endSound = "fishSlap",
            Parent = this.lastUser.currentLocation
          });
        }
      }
      if (this.lastUser.IsLocalPlayer)
      {
        this.lastUser.currentLocation.playSound("pullItemFromWater");
        this.lastUser.currentLocation.playSound("dwop");
      }
      this.castedButBobberStillInAir = false;
      this.pullingOutOfWater = true;
      this.isFishing = false;
      this.isReeling = false;
      this.lastUser.FarmerSprite.PauseForSingleAnimation = false;
      switch (this.lastUser.FacingDirection)
      {
        case 0:
          this.lastUser.FarmerSprite.animateBackwardsOnce(299, animationInterval);
          break;
        case 1:
          this.lastUser.FarmerSprite.animateBackwardsOnce(300, animationInterval);
          break;
        case 2:
          this.lastUser.FarmerSprite.animateBackwardsOnce(301, animationInterval);
          break;
        case 3:
          this.lastUser.FarmerSprite.animateBackwardsOnce(302, animationInterval);
          break;
      }
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      float scale = 4f;
      if (!this.bobber.Equals((object) Vector2.Zero) && this.isFishing)
      {
        Vector2 bobber = (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber;
        if ((double) this.bobberTimeAccumulator > (double) this.timePerBobberBob)
        {
          if (!this.isNibbling && !this.isReeling || Game1.random.NextDouble() < 0.05)
          {
            this.lastUser.currentLocation.localSound("waterSlosh");
            this.lastUser.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64, 64), 150f, 8, 0, new Vector2(this.bobber.X - 32f, this.bobber.Y - 32f), false, Game1.random.NextDouble() < 0.5, 1f / 1000f, 0.01f, Color.White, 0.75f, 3f / 1000f, 0.0f, 0.0f));
          }
          this.timePerBobberBob = this.bobberBob == 0 ? (float) Game1.random.Next(1500, 3500) : (float) Game1.random.Next(350, 750);
          this.bobberTimeAccumulator = 0.0f;
          if (this.isNibbling || this.isReeling)
          {
            this.timePerBobberBob = (float) Game1.random.Next(25, 75);
            bobber.X += (float) Game1.random.Next(-5, 5);
            bobber.Y += (float) Game1.random.Next(-5, 5);
            if (!this.isReeling)
              scale += (float) Game1.random.Next(-20, 20) / 100f;
          }
          else if (Game1.random.NextDouble() < 0.1)
            this.lastUser.currentLocation.localSound("bob");
        }
        float layerDepth = bobber.Y / 10000f;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, bobber), new Rectangle?(new Rectangle(179 + this.bobberBob * 9, 1903, 9, 9)), Color.White, 0.0f, new Vector2(4f, 4f), scale, SpriteEffects.None, layerDepth);
      }
      else if ((this.isTimingCast || (double) this.castingChosenCountdown > 0.0) && this.lastUser.IsLocalPlayer)
      {
        int num1 = (int) (-(double) Math.Abs(this.castingChosenCountdown / 2f - this.castingChosenCountdown) / 50.0);
        float num2 = (double) this.castingChosenCountdown <= 0.0 || (double) this.castingChosenCountdown >= 100.0 ? 1f : this.castingChosenCountdown / 100f;
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position + new Vector2(-48f, (float) (num1 - 160))), new Rectangle?(new Rectangle(193, 1868, 47, 12)), Color.White * num2, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.885f);
        b.Draw(Game1.staminaRect, new Rectangle((int) Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position).X - 32 - 4, (int) Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position).Y + num1 - 128 - 32 + 12, (int) (164.0 * (double) this.castingPower), 25), new Rectangle?(Game1.staminaRect.Bounds), Utility.getRedToGreenLerpColor(this.castingPower) * num2, 0.0f, Vector2.Zero, SpriteEffects.None, 0.887f);
      }
      for (int index = this.animations.Count - 1; index >= 0; --index)
        this.animations[index].draw(b);
      if (this.sparklingText != null && !this.fishCaught)
        this.sparklingText.draw(b, Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position + new Vector2(-24f, -192f)));
      else if (this.sparklingText != null && this.fishCaught)
        this.sparklingText.draw(b, Game1.GlobalToLocal(Game1.viewport, this.getLastFarmerToUse().Position + new Vector2(-64f, -352f)));
      if (!this.bobber.Value.Equals(Vector2.Zero) && (this.isFishing || this.pullingOutOfWater || this.castedButBobberStillInAir) && this.lastUser.FarmerSprite.CurrentFrame != 57 && (this.lastUser.FacingDirection != 0 || !this.pullingOutOfWater || this.whichFish == -1))
      {
        Vector2 vector2_1 = this.isFishing ? (Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber : (this.animations.Count > 0 ? this.animations[0].position + new Vector2(0.0f, 4f * scale) : Vector2.Zero);
        if (this.whichFish != -1)
          vector2_1 += new Vector2(32f, 32f);
        Vector2 vector2_2 = Vector2.Zero;
        if (this.castedButBobberStillInAir)
        {
          switch (this.lastUser.FacingDirection)
          {
            case 0:
              switch (this.lastUser.FarmerSprite.currentAnimationIndex)
              {
                case 0:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(22f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 4.0)));
                  break;
                case 1:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(32f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 4.0)));
                  break;
                case 2:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(36f, (float) ((double) this.lastUser.armOffset.Y - 64.0 + 40.0)));
                  break;
                case 3:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(36f, this.lastUser.armOffset.Y - 16f));
                  break;
                case 4:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(36f, this.lastUser.armOffset.Y - 32f));
                  break;
                case 5:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(36f, this.lastUser.armOffset.Y - 32f));
                  break;
                default:
                  vector2_2 = Vector2.Zero;
                  break;
              }
              break;
            case 1:
              switch (this.lastUser.FarmerSprite.currentAnimationIndex)
              {
                case 0:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-48f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 8.0)));
                  break;
                case 1:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-16f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 20.0)));
                  break;
                case 2:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(84f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 20.0)));
                  break;
                case 3:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(112f, (float) ((double) this.lastUser.armOffset.Y - 32.0 - 20.0)));
                  break;
                case 4:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(120f, (float) ((double) this.lastUser.armOffset.Y - 32.0 + 8.0)));
                  break;
                case 5:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(120f, (float) ((double) this.lastUser.armOffset.Y - 32.0 + 8.0)));
                  break;
                default:
                  vector2_2 = Vector2.Zero;
                  break;
              }
              break;
            case 2:
              switch (this.lastUser.FarmerSprite.currentAnimationIndex)
              {
                case 0:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(8f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 4.0)));
                  break;
                case 1:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(22f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 4.0)));
                  break;
                case 2:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(28f, (float) ((double) this.lastUser.armOffset.Y - 64.0 + 40.0)));
                  break;
                case 3:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(28f, this.lastUser.armOffset.Y - 8f));
                  break;
                case 4:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(28f, this.lastUser.armOffset.Y + 32f));
                  break;
                case 5:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(28f, this.lastUser.armOffset.Y + 32f));
                  break;
                default:
                  vector2_2 = Vector2.Zero;
                  break;
              }
              break;
            case 3:
              switch (this.lastUser.FarmerSprite.currentAnimationIndex)
              {
                case 0:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(112f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 8.0)));
                  break;
                case 1:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(80f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 20.0)));
                  break;
                case 2:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-20f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 20.0)));
                  break;
                case 3:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-48f, (float) ((double) this.lastUser.armOffset.Y - 32.0 - 20.0)));
                  break;
                case 4:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-56f, (float) ((double) this.lastUser.armOffset.Y - 32.0 + 8.0)));
                  break;
                case 5:
                  vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-56f, (float) ((double) this.lastUser.armOffset.Y - 32.0 + 8.0)));
                  break;
              }
              break;
            default:
              vector2_2 = Vector2.Zero;
              break;
          }
        }
        else if (this.isReeling)
        {
          if (this.lastUser != null && this.lastUser.IsLocalPlayer && Game1.didPlayerJustClickAtAll())
          {
            switch (this.lastUser.FacingDirection)
            {
              case 0:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(24f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 12.0)));
                break;
              case 1:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(20f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 12.0)));
                break;
              case 2:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(12f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 8.0)));
                break;
              case 3:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(48f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 12.0)));
                break;
            }
          }
          else
          {
            switch (this.lastUser.FacingDirection)
            {
              case 0:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(25f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 4.0)));
                break;
              case 1:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(28f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 8.0)));
                break;
              case 2:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(12f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 4.0)));
                break;
              case 3:
                vector2_2 = Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(36f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 8.0)));
                break;
            }
          }
        }
        else
        {
          switch (this.lastUser.FacingDirection)
          {
            case 0:
              vector2_2 = this.pullingOutOfWater ? Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(22f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 4.0))) : Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(28f, (float) ((double) this.lastUser.armOffset.Y - 64.0 - 12.0)));
              break;
            case 1:
              vector2_2 = this.pullingOutOfWater ? Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-48f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 8.0))) : Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(120f, (float) ((double) this.lastUser.armOffset.Y - 64.0 + 16.0)));
              break;
            case 2:
              vector2_2 = this.pullingOutOfWater ? Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(8f, (float) ((double) this.lastUser.armOffset.Y - 96.0 + 4.0))) : Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(28f, (float) ((double) this.lastUser.armOffset.Y + 64.0 - 12.0)));
              break;
            case 3:
              vector2_2 = this.pullingOutOfWater ? Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(112f, (float) ((double) this.lastUser.armOffset.Y - 96.0 - 8.0))) : Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-56f, (float) ((double) this.lastUser.armOffset.Y - 64.0 + 16.0)));
              break;
            default:
              vector2_2 = Vector2.Zero;
              break;
          }
        }
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, vector2_1 + new Vector2(0.0f, (float) (-2.0 * (double) scale + (this.bobberBob == 1 ? 4.0 : 0.0))));
        if (this.isTimingCast || this.isCasting && !this.lastUser.IsLocalPlayer)
          return;
        if (this.isReeling)
        {
          Utility.drawLineWithScreenCoordinates((int) vector2_2.X, (int) vector2_2.Y, (int) local.X, (int) local.Y, b, Color.White * 0.5f);
        }
        else
        {
          Vector2 p0 = vector2_2;
          Vector2 p1 = new Vector2(vector2_2.X + (float) (((double) local.X - (double) vector2_2.X) / 3.0), vector2_2.Y + (float) (((double) local.Y - (double) vector2_2.Y) * 2.0 / 3.0));
          Vector2 p2 = new Vector2(vector2_2.X + (float) (((double) local.X - (double) vector2_2.X) * 2.0 / 3.0), vector2_2.Y + (float) (((double) local.Y - (double) vector2_2.Y) * (this.isFishing ? 6.0 : 2.0) / 5.0));
          Vector2 p3 = local;
          for (float t = 0.0f; (double) t < 1.0; t += 0.025f)
          {
            Vector2 curvePoint = Utility.GetCurvePoint(t, p0, p1, p2, p3);
            Utility.drawLineWithScreenCoordinates((int) vector2_2.X, (int) vector2_2.Y, (int) curvePoint.X, (int) curvePoint.Y, b, Color.White * 0.5f, (float) ((double) this.lastUser.getStandingY() / 10000.0 + (this.lastUser.FacingDirection != 0 ? 0.00499999988824129 : -1.0 / 1000.0)));
            vector2_2 = curvePoint;
          }
        }
      }
      else
      {
        if (!this.fishCaught)
          return;
        float num = (float) (4.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-120f, num - 288f)), new Rectangle?(new Rectangle(31, 1870, 73, 49)), Color.White * 0.8f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 0.0599999986588955));
        if (this.itemCategory == "Object")
        {
          b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-124f, num - 284f) + new Vector2(44f, 68f)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.whichFish, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 9.99999974737875E-05 + 0.0599999986588955));
          if (this.caughtDoubleFish)
            Utility.drawTinyDigits(2, b, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-120f, num - 284f) + new Vector2(23f, 29f) * 4f), 3f, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 9.99999974737875E-05 + 0.0610000006854534), Color.White);
          b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(0.0f, -56f)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.whichFish, 16, 16)), Color.White, this.fishSize == -1 || this.whichFish == 800 || this.whichFish == 798 || this.whichFish == 149 || this.whichFish == 151 ? 0.0f : 2.356194f, new Vector2(8f, 8f), 3f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 1.0 / 500.0 + 0.0599999986588955));
          if (this.caughtDoubleFish)
            b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-8f, -56f)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.whichFish, 16, 16)), Color.White, this.fishSize == -1 || this.whichFish == 800 || this.whichFish == 798 || this.whichFish == 149 || this.whichFish == 151 ? 0.0f : 2.513274f, new Vector2(8f, 8f), 3f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 1.0 / 500.0 + 0.0579999983310699));
        }
        else
        {
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(-124f, num - 284f) + new Vector2(44f, 68f)), new Rectangle?(new Rectangle(228, 408, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 9.99999974737875E-05 + 0.0599999986588955));
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(0.0f, -56f)), new Rectangle?(new Rectangle(228, 408, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), 3f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 1.0 / 500.0 + 0.0599999986588955));
        }
        string text = "???";
        if (this.itemCategory == "Object")
          text = Game1.objectInformation[this.whichFish].Split('/')[4];
        b.DrawString(Game1.smallFont, text, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2((float) (26.0 - (double) Game1.smallFont.MeasureString(text).X / 2.0), num - 278f)), this.bossFish ? new Color(126, 61, 237) : Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 1.0 / 500.0 + 0.0599999986588955));
        if (this.fishSize == -1)
          return;
        b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14082"), Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(20f, num - 214f)), Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 1.0 / 500.0 + 0.0599999986588955));
        b.DrawString(Game1.smallFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (object) (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? Math.Round((double) this.fishSize * 2.54) : (double) this.fishSize)), Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2((float) (85.0 - (double) Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (object) (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? Math.Round((double) this.fishSize * 2.54) : (double) this.fishSize))).X / 2.0), num - 179f)), this.recordSize ? Color.Blue * Math.Min(1f, (float) ((double) num / 8.0 + 1.5)) : Game1.textColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 1.0 / 500.0 + 0.0599999986588955));
      }
    }

    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
      if ((double) who.Stamina <= 1.0 && who.IsLocalPlayer)
      {
        if (!who.isEmoting)
          who.doEmote(36);
        who.CanMove = !Game1.eventUp;
        who.UsingTool = false;
        who.canReleaseTool = false;
        this.doneFishing((Farmer) null);
        return true;
      }
      this.usedGamePadToCast = false;
      if (Game1.input.GetGamePadState().IsButtonDown(Buttons.X))
        this.usedGamePadToCast = true;
      this.bossFish = false;
      this.originalFacingDirection = who.FacingDirection;
      who.Halt();
      this.treasureCaught = false;
      this.showingTreasure = false;
      this.isFishing = false;
      this.hit = false;
      this.favBait = false;
      if (this.attachments != null && this.attachments.Length > 1 && this.attachments[1] != null)
        this.hadBobber = true;
      this.isNibbling = false;
      this.lastUser = who;
      this.isTimingCast = true;
      this._totalMotionBufferIndex = 0;
      for (int index = 0; index < this._totalMotionBuffer.Length; ++index)
        this._totalMotionBuffer[index] = Vector2.Zero;
      this._totalMotion.Value = Vector2.Zero;
      this._lastAppliedMotion = Vector2.Zero;
      who.UsingTool = true;
      this.whichFish = -1;
      this.itemCategory = "";
      this.recastTimerMs = 0;
      who.canMove = false;
      this.fishCaught = false;
      this.doneWithAnimation = false;
      who.canReleaseTool = false;
      this.hasDoneFucntionYet = false;
      this.isReeling = false;
      this.pullingOutOfWater = false;
      this.castingPower = 0.0f;
      this.castingChosenCountdown = 0.0f;
      this.animations.Clear();
      this.sparklingText = (SparklingText) null;
      this.setTimingCastAnimation(who);
      return true;
    }

    public void setTimingCastAnimation(Farmer who)
    {
      if (who.CurrentTool == null)
        return;
      switch (who.FacingDirection)
      {
        case 0:
          who.FarmerSprite.setCurrentFrame(295);
          who.CurrentTool.Update(0, 0, who);
          break;
        case 1:
          who.FarmerSprite.setCurrentFrame(296);
          who.CurrentTool.Update(1, 0, who);
          break;
        case 2:
          who.FarmerSprite.setCurrentFrame(297);
          who.CurrentTool.Update(2, 0, who);
          break;
        case 3:
          who.FarmerSprite.setCurrentFrame(298);
          who.CurrentTool.Update(3, 0, who);
          break;
      }
    }

    public void doneFishing(Farmer who, bool consumeBaitAndTackle = false) => this.doneFishingEvent.Fire(consumeBaitAndTackle);

    private void doDoneFishing(bool consumeBaitAndTackle)
    {
      if (consumeBaitAndTackle && this.lastUser != null && this.lastUser.IsLocalPlayer)
      {
        float num = 1f;
        if (this.hasEnchantmentOfType<PreservingEnchantment>())
          num = 0.5f;
        if (this.attachments[0] != null && Game1.random.NextDouble() < (double) num)
        {
          --this.attachments[0].Stack;
          if (this.attachments[0].Stack <= 0)
          {
            this.attachments[0] = (StardewValley.Object) null;
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14085"));
          }
        }
        if (this.attachments[1] != null && !this.lastCatchWasJunk && Game1.random.NextDouble() < (double) num)
        {
          ++this.attachments[1].uses.Value;
          if (this.attachments[1].uses.Value >= FishingRod.maxTackleUses)
          {
            this.attachments[1] = (StardewValley.Object) null;
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14086"));
          }
        }
      }
      if (this.lastUser != null && this.lastUser.IsLocalPlayer)
        this.bobber.Set(Vector2.Zero);
      this.isNibbling = false;
      this.fishCaught = false;
      this.isFishing = false;
      this.isReeling = false;
      this.isCasting = false;
      this.isTimingCast = false;
      this.treasureCaught = false;
      this.showingTreasure = false;
      this.doneWithAnimation = false;
      this.pullingOutOfWater = false;
      this.fromFishPond = false;
      this.caughtDoubleFish = false;
      this.fishingBiteAccumulator = 0.0f;
      this.fishingNibbleAccumulator = 0.0f;
      this.timeUntilFishingBite = -1f;
      this.timeUntilFishingNibbleDone = -1f;
      this.bobberTimeAccumulator = 0.0f;
      if (FishingRod.chargeSound != null && FishingRod.chargeSound.IsPlaying && this.lastUser.IsLocalPlayer)
      {
        FishingRod.chargeSound.Stop(AudioStopOptions.Immediate);
        FishingRod.chargeSound = (ICue) null;
      }
      if (FishingRod.reelSound != null && FishingRod.reelSound.IsPlaying)
      {
        FishingRod.reelSound.Stop(AudioStopOptions.Immediate);
        FishingRod.reelSound = (ICue) null;
      }
      if (this.lastUser == null)
        return;
      this.lastUser.UsingTool = false;
      this.lastUser.CanMove = true;
      this.lastUser.completelyStopAnimatingOrDoingAction();
      if (this.lastUser != Game1.player)
        return;
      this.lastUser.faceDirection(this.originalFacingDirection);
    }

    public static void doneWithCastingAnimation(Farmer who)
    {
      if (who.CurrentTool == null || !(who.CurrentTool is FishingRod))
        return;
      (who.CurrentTool as FishingRod).doneWithAnimation = true;
      if (!(who.CurrentTool as FishingRod).hasDoneFucntionYet)
        return;
      who.canReleaseTool = true;
      who.UsingTool = false;
      who.canMove = true;
      Farmer.canMoveNow(who);
    }

    public void castingEndFunction(int extraInfo)
    {
      this.castedButBobberStillInAir = false;
      if (this.lastUser == null)
        return;
      float stamina = this.lastUser.Stamina;
      this.lastUser.CurrentTool.DoFunction(this.lastUser.currentLocation, (int) this.bobber.X, (int) this.bobber.Y, 1, this.lastUser);
      this.lastUser.lastClick = Vector2.Zero;
      if (FishingRod.reelSound != null)
        FishingRod.reelSound.Stop(AudioStopOptions.Immediate);
      FishingRod.reelSound = (ICue) null;
      if ((double) this.lastUser.Stamina <= 0.0 && (double) stamina > 0.0)
        this.lastUser.doEmote(36);
      Game1.toolHold = 0.0f;
      if (this.isFishing || !this.doneWithAnimation)
        return;
      this.castingEndEnableMovement();
    }

    private void castingEndEnableMovement() => this.castingEndEnableMovementEvent.Fire();

    private void doCastingEndEnableMovement() => Farmer.canMoveNow(this.lastUser);

    public override void tickUpdate(GameTime time, Farmer who)
    {
      this.lastUser = who;
      this.beginReelingEvent.Poll();
      this.putAwayEvent.Poll();
      this.startCastingEvent.Poll();
      this.pullFishFromWaterEvent.Poll();
      this.doneFishingEvent.Poll();
      this.castingEndEnableMovementEvent.Poll();
      if (this.recastTimerMs > 0 && who.IsLocalPlayer)
      {
        if (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed || Game1.didPlayerJustClickAtAll() || Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
        {
          this.recastTimerMs -= time.ElapsedGameTime.Milliseconds;
          if (this.recastTimerMs <= 0)
          {
            this.recastTimerMs = 0;
            if (Game1.activeClickableMenu == null)
              who.BeginUsingTool();
          }
        }
        else
          this.recastTimerMs = 0;
      }
      if (this.isFishing && !Game1.shouldTimePass())
      {
        switch (Game1.activeClickableMenu)
        {
          case null:
          case BobberBar _:
            break;
          default:
            return;
        }
      }
      if (who.CurrentTool != null && who.CurrentTool.Equals((object) this) && who.UsingTool)
        who.CanMove = false;
      else if (Game1.currentMinigame == null && (who.CurrentTool == null || !(who.CurrentTool is FishingRod) || !who.UsingTool))
      {
        if (FishingRod.chargeSound == null || !FishingRod.chargeSound.IsPlaying || !who.IsLocalPlayer)
          return;
        FishingRod.chargeSound.Stop(AudioStopOptions.Immediate);
        FishingRod.chargeSound = (ICue) null;
        return;
      }
      for (int index = this.animations.Count - 1; index >= 0; --index)
      {
        if (this.animations[index].update(time))
          this.animations.RemoveAt(index);
      }
      if (this.sparklingText != null && this.sparklingText.update(time))
        this.sparklingText = (SparklingText) null;
      if ((double) this.castingChosenCountdown > 0.0)
      {
        this.castingChosenCountdown -= (float) time.ElapsedGameTime.Milliseconds;
        if ((double) this.castingChosenCountdown <= 0.0 && who.CurrentTool != null)
        {
          switch (who.FacingDirection)
          {
            case 0:
              who.FarmerSprite.animateOnce(295, 1f, 1);
              who.CurrentTool.Update(0, 0, who);
              break;
            case 1:
              who.FarmerSprite.animateOnce(296, 1f, 1);
              who.CurrentTool.Update(1, 0, who);
              break;
            case 2:
              who.FarmerSprite.animateOnce(297, 1f, 1);
              who.CurrentTool.Update(2, 0, who);
              break;
            case 3:
              who.FarmerSprite.animateOnce(298, 1f, 1);
              who.CurrentTool.Update(3, 0, who);
              break;
          }
          if (who.FacingDirection == 1 || who.FacingDirection == 3)
          {
            float num1 = Math.Max(128f, (float) ((double) this.castingPower * (double) (this.getAddedDistance(who) + 4) * 64.0)) - 8f;
            float y = 0.005f;
            float num2 = num1 * (float) Math.Sqrt((double) y / (2.0 * ((double) num1 + 96.0)));
            float animationInterval = (float) (2.0 * ((double) num2 / (double) y)) + ((float) Math.Sqrt((double) num2 * (double) num2 + 2.0 * (double) y * 96.0) - num2) / y;
            if (this.lastUser.IsLocalPlayer)
              this.bobber.Set(new Vector2((float) who.getStandingX() + (who.FacingDirection == 3 ? -1f : 1f) * num1, (float) who.getStandingY()));
            this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(170, 1903, 7, 8), animationInterval, 1, 0, who.Position + new Vector2(0.0f, -96f), false, false, (float) who.getStandingY() / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-20, 20) / 100f)
            {
              motion = new Vector2((who.FacingDirection == 3 ? -1f : 1f) * num2, -num2),
              acceleration = new Vector2(0.0f, y),
              endFunction = new TemporaryAnimatedSprite.endBehavior(this.castingEndFunction),
              timeBasedMotion = true
            });
          }
          else
          {
            float num3 = -Math.Max(128f, (float) ((double) this.castingPower * (double) (this.getAddedDistance(who) + 3) * 64.0));
            float num4 = Math.Abs(num3 - 64f);
            if (this.lastUser.FacingDirection == 0)
            {
              num3 = -num3;
              num4 += 64f;
            }
            float y = 0.005f;
            float num5 = (float) Math.Sqrt(2.0 * (double) y * (double) num4);
            float animationInterval = (float) (Math.Sqrt(2.0 * ((double) num4 - (double) num3) / (double) y) + (double) num5 / (double) y) * 1.05f;
            if (this.lastUser.FacingDirection == 0)
              animationInterval *= 1.05f;
            if (this.lastUser.IsLocalPlayer)
              this.bobber.Set(new Vector2((float) who.getStandingX(), (float) who.getStandingY() - num3));
            this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(170, 1903, 7, 8), animationInterval, 1, 0, who.Position + new Vector2(24f, -96f), false, false, this.bobber.Y / 10000f, 0.0f, Color.White, 4f, 0.0f, 0.0f, (float) Game1.random.Next(-20, 20) / 100f)
            {
              alphaFade = 0.0001f,
              motion = new Vector2(0.0f, -num5),
              acceleration = new Vector2(0.0f, y),
              endFunction = new TemporaryAnimatedSprite.endBehavior(this.castingEndFunction),
              timeBasedMotion = true
            });
          }
          this._hasPlayerAdjustedBobber = false;
          this.castedButBobberStillInAir = true;
          this.isCasting = false;
          if (who.IsLocalPlayer)
            who.currentLocation.playSound("cast");
          if (who.IsLocalPlayer && Game1.soundBank != null)
          {
            FishingRod.reelSound = Game1.soundBank.GetCue("slowReel");
            FishingRod.reelSound.SetVariable("Pitch", 1600);
            FishingRod.reelSound.Play();
          }
        }
      }
      else if (!this.isTimingCast && (double) this.castingChosenCountdown <= 0.0)
        who.jitterStrength = 0.0f;
      if (this.isTimingCast)
      {
        if (FishingRod.chargeSound == null && Game1.soundBank != null)
          FishingRod.chargeSound = Game1.soundBank.GetCue("SinWave");
        if (who.IsLocalPlayer && FishingRod.chargeSound != null && !FishingRod.chargeSound.IsPlaying)
          FishingRod.chargeSound.Play();
        this.castingPower = Math.Max(0.0f, Math.Min(1f, this.castingPower + this.castingTimerSpeed * (float) time.ElapsedGameTime.Milliseconds));
        if (who.IsLocalPlayer && FishingRod.chargeSound != null)
          FishingRod.chargeSound.SetVariable("Pitch", 2400f * this.castingPower);
        if ((double) this.castingPower == 1.0 || (double) this.castingPower == 0.0)
          this.castingTimerSpeed = -this.castingTimerSpeed;
        who.armOffset.Y = (float) (2.0 * Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2));
        who.jitterStrength = Math.Max(0.0f, this.castingPower - 0.5f);
        if (!who.IsLocalPlayer || (this.usedGamePadToCast || Game1.input.GetMouseState().LeftButton != ButtonState.Released) && (!this.usedGamePadToCast || !Game1.options.gamepadControls || !Game1.input.GetGamePadState().IsButtonUp(Buttons.X)) || !Game1.areAllOfTheseKeysUp(Game1.GetKeyboardState(), Game1.options.useToolButton))
          return;
        this.startCasting();
      }
      else if (this.isReeling)
      {
        TimeSpan timeSpan;
        if (who.IsLocalPlayer && Game1.didPlayerJustClickAtAll())
        {
          if (Game1.isAnyGamePadButtonBeingPressed())
            Game1.lastCursorMotionWasMouse = false;
          switch (who.FacingDirection)
          {
            case 0:
              who.FarmerSprite.setCurrentSingleFrame(76);
              break;
            case 1:
              who.FarmerSprite.setCurrentSingleFrame(72, (short) 100);
              break;
            case 2:
              who.FarmerSprite.setCurrentSingleFrame(75);
              break;
            case 3:
              who.FarmerSprite.setCurrentSingleFrame(72, (short) 100, flip: true);
              break;
          }
          ref Vector2 local = ref who.armOffset;
          timeSpan = Game1.currentGameTime.TotalGameTime;
          double num = Math.Round(Math.Sin(timeSpan.TotalMilliseconds / 250.0), 2);
          local.Y = (float) num;
          who.jitterStrength = 1f;
        }
        else
        {
          switch (who.FacingDirection)
          {
            case 0:
              who.FarmerSprite.setCurrentSingleFrame(36);
              break;
            case 1:
              who.FarmerSprite.setCurrentSingleFrame(48, (short) 100);
              break;
            case 2:
              who.FarmerSprite.setCurrentSingleFrame(66);
              break;
            case 3:
              who.FarmerSprite.setCurrentSingleFrame(48, (short) 100, flip: true);
              break;
          }
          who.stopJittering();
        }
        who.armOffset = new Vector2((float) Game1.random.Next(-10, 11) / 10f, (float) Game1.random.Next(-10, 11) / 10f);
        double bobberTimeAccumulator = (double) this.bobberTimeAccumulator;
        timeSpan = time.ElapsedGameTime;
        double milliseconds = (double) timeSpan.Milliseconds;
        this.bobberTimeAccumulator = (float) (bobberTimeAccumulator + milliseconds);
      }
      else if (this.isFishing)
      {
        TimeSpan timeSpan;
        if (this.lastUser.IsLocalPlayer)
        {
          NetPosition bobber = this.bobber;
          double y = (double) bobber.Y;
          timeSpan = Game1.currentGameTime.TotalGameTime;
          double num = 0.100000001490116 * Math.Sin(timeSpan.TotalMilliseconds / 250.0);
          bobber.Y = (float) (y + num);
        }
        who.canReleaseTool = true;
        double bobberTimeAccumulator = (double) this.bobberTimeAccumulator;
        timeSpan = time.ElapsedGameTime;
        double milliseconds1 = (double) timeSpan.Milliseconds;
        this.bobberTimeAccumulator = (float) (bobberTimeAccumulator + milliseconds1);
        switch (who.FacingDirection)
        {
          case 0:
            who.FarmerSprite.setCurrentFrame(44);
            break;
          case 1:
            who.FarmerSprite.setCurrentFrame(89);
            break;
          case 2:
            who.FarmerSprite.setCurrentFrame(70);
            break;
          case 3:
            who.FarmerSprite.setCurrentFrame(89, 0, 10, 1, true, false);
            break;
        }
        ref Vector2 local = ref who.armOffset;
        timeSpan = Game1.currentGameTime.TotalGameTime;
        double num6 = Math.Round(Math.Sin(timeSpan.TotalMilliseconds / 250.0), 2) + (who.FacingDirection == 1 || who.FacingDirection == 3 ? 1.0 : -1.0);
        local.Y = (float) num6;
        if (!who.IsLocalPlayer)
          return;
        if ((double) this.timeUntilFishingBite != -1.0)
        {
          double fishingBiteAccumulator = (double) this.fishingBiteAccumulator;
          timeSpan = time.ElapsedGameTime;
          double milliseconds2 = (double) timeSpan.Milliseconds;
          this.fishingBiteAccumulator = (float) (fishingBiteAccumulator + milliseconds2);
          if ((double) this.fishingBiteAccumulator > (double) this.timeUntilFishingBite)
          {
            this.fishingBiteAccumulator = 0.0f;
            this.timeUntilFishingBite = -1f;
            this.isNibbling = true;
            if (this.hasEnchantmentOfType<AutoHookEnchantment>())
            {
              this.timePerBobberBob = 1f;
              this.timeUntilFishingNibbleDone = (float) FishingRod.maxTimeToNibble;
              this.DoFunction(who.currentLocation, (int) this.bobber.X, (int) this.bobber.Y, 1, who);
              Rumble.rumble(0.95f, 200f);
              return;
            }
            who.PlayFishBiteChime();
            Rumble.rumble(0.75f, 250f);
            this.timeUntilFishingNibbleDone = (float) FishingRod.maxTimeToNibble;
            if (Game1.currentMinigame == null)
              Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(395, 497, 3, 8), new Vector2((float) (this.lastUser.getStandingX() - Game1.viewport.X), (float) (this.lastUser.getStandingY() - 128 - 8 - Game1.viewport.Y)), false, 0.02f, Color.White)
              {
                scale = 5f,
                scaleChange = -0.01f,
                motion = new Vector2(0.0f, -0.5f),
                shakeIntensityChange = -0.005f,
                shakeIntensity = 1f
              });
            this.timePerBobberBob = 1f;
          }
        }
        if ((double) this.timeUntilFishingNibbleDone == -1.0 || this.hit)
          return;
        double nibbleAccumulator = (double) this.fishingNibbleAccumulator;
        timeSpan = time.ElapsedGameTime;
        double milliseconds3 = (double) timeSpan.Milliseconds;
        this.fishingNibbleAccumulator = (float) (nibbleAccumulator + milliseconds3);
        if ((double) this.fishingNibbleAccumulator <= (double) this.timeUntilFishingNibbleDone)
          return;
        this.fishingNibbleAccumulator = 0.0f;
        this.timeUntilFishingNibbleDone = -1f;
        this.isNibbling = false;
        this.timeUntilFishingBite = this.calculateTimeUntilFishingBite(this.calculateBobberTile(), false, who);
      }
      else if (who.UsingTool && this.castedButBobberStillInAir)
      {
        Vector2 zero1 = Vector2.Zero;
        if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveDownButton))
        {
          if (Game1.options.gamepadControls)
          {
            GamePadState oldPadState = Game1.oldPadState;
            if (!Game1.oldPadState.IsButtonDown(Buttons.DPadDown) && (double) Game1.input.GetGamePadState().ThumbSticks.Left.Y >= 0.0)
              goto label_99;
          }
          else
            goto label_99;
        }
        if (who.FacingDirection != 2 && who.FacingDirection != 0)
        {
          zero1.Y += 4f;
          this._hasPlayerAdjustedBobber = true;
        }
label_99:
        if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveRightButton))
        {
          if (Game1.options.gamepadControls)
          {
            GamePadState oldPadState = Game1.oldPadState;
            if (!Game1.oldPadState.IsButtonDown(Buttons.DPadRight) && (double) Game1.input.GetGamePadState().ThumbSticks.Left.X <= 0.0)
              goto label_104;
          }
          else
            goto label_104;
        }
        if (who.FacingDirection != 1 && who.FacingDirection != 3)
        {
          zero1.X += 2f;
          this._hasPlayerAdjustedBobber = true;
        }
label_104:
        if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveUpButton))
        {
          if (Game1.options.gamepadControls)
          {
            GamePadState oldPadState = Game1.oldPadState;
            if (!Game1.oldPadState.IsButtonDown(Buttons.DPadUp) && (double) Game1.input.GetGamePadState().ThumbSticks.Left.Y <= 0.0)
              goto label_109;
          }
          else
            goto label_109;
        }
        if (who.FacingDirection != 0 && who.FacingDirection != 2)
        {
          zero1.Y -= 4f;
          this._hasPlayerAdjustedBobber = true;
        }
label_109:
        if (!Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.moveLeftButton))
        {
          if (Game1.options.gamepadControls)
          {
            GamePadState oldPadState = Game1.oldPadState;
            if (!Game1.oldPadState.IsButtonDown(Buttons.DPadLeft) && (double) Game1.input.GetGamePadState().ThumbSticks.Left.X >= 0.0)
              goto label_114;
          }
          else
            goto label_114;
        }
        if (who.FacingDirection != 3 && who.FacingDirection != 1)
        {
          zero1.X -= 2f;
          this._hasPlayerAdjustedBobber = true;
        }
label_114:
        if (!this._hasPlayerAdjustedBobber)
        {
          Vector2 bobberTile = this.calculateBobberTile();
          if (!this.lastUser.currentLocation.isTileFishable((int) bobberTile.X, (int) bobberTile.Y))
          {
            if (this.lastUser.FacingDirection == 3 || this.lastUser.FacingDirection == 1)
            {
              int num = 1;
              if ((double) bobberTile.Y % 1.0 < 0.5)
                num = -1;
              if (this.lastUser.currentLocation.isTileFishable((int) bobberTile.X, (int) bobberTile.Y + num))
                zero1.Y += (float) num * 4f;
              else if (this.lastUser.currentLocation.isTileFishable((int) bobberTile.X, (int) bobberTile.Y - num))
                zero1.Y -= (float) num * 4f;
            }
            if (this.lastUser.FacingDirection == 0 || this.lastUser.FacingDirection == 2)
            {
              int num = 1;
              if ((double) bobberTile.X % 1.0 < 0.5)
                num = -1;
              if (this.lastUser.currentLocation.isTileFishable((int) bobberTile.X + num, (int) bobberTile.Y))
                zero1.X += (float) num * 4f;
              else if (this.lastUser.currentLocation.isTileFishable((int) bobberTile.X - num, (int) bobberTile.Y))
                zero1.X -= (float) num * 4f;
            }
          }
        }
        if (who.IsLocalPlayer)
        {
          this.bobber.Set((Vector2) (NetPausableField<Vector2, NetVector2, NetVector2>) this.bobber + zero1);
          this._totalMotion.Set(this._totalMotion.Value + zero1);
        }
        if (this.animations.Count <= 0)
          return;
        Vector2 zero2 = Vector2.Zero;
        Vector2 vector2;
        if (who.IsLocalPlayer)
        {
          vector2 = this._totalMotion.Value;
        }
        else
        {
          this._totalMotionBuffer[this._totalMotionBufferIndex] = this._totalMotion.Value;
          for (int index = 0; index < this._totalMotionBuffer.Length; ++index)
            zero2 += this._totalMotionBuffer[index];
          vector2 = zero2 / (float) this._totalMotionBuffer.Length;
          this._totalMotionBufferIndex = (this._totalMotionBufferIndex + 1) % this._totalMotionBuffer.Length;
        }
        this.animations[0].position -= this._lastAppliedMotion;
        this._lastAppliedMotion = vector2;
        this.animations[0].position += vector2;
      }
      else if (this.showingTreasure)
        who.FarmerSprite.setCurrentSingleFrame(0);
      else if (this.fishCaught)
      {
        if (!Game1.isFestival())
        {
          who.faceDirection(2);
          who.FarmerSprite.setCurrentFrame(84);
        }
        if (Game1.random.NextDouble() < 0.025)
          who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(653, 858, 1, 1), 9999f, 1, 1, who.Position + new Vector2((float) (Game1.random.Next(-3, 2) * 4), -32f), false, false, (float) ((double) who.getStandingY() / 10000.0 + 1.0 / 500.0), 0.04f, Color.LightBlue, 5f, 0.0f, 0.0f, 0.0f)
          {
            acceleration = new Vector2(0.0f, 0.25f)
          });
        if (!who.IsLocalPlayer || Game1.input.GetMouseState().LeftButton != ButtonState.Pressed && !Game1.didPlayerJustClickAtAll() && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
          return;
        who.currentLocation.localSound("coin");
        if (!this.treasureCaught)
        {
          this.recastTimerMs = 200;
          StardewValley.Object @object = (StardewValley.Object) null;
          if (this.itemCategory == "Object")
          {
            @object = new StardewValley.Object(this.whichFish, 1, quality: this.fishQuality);
            if (this.whichFish == GameLocation.CAROLINES_NECKLACE_ITEM)
              @object.questItem.Value = true;
            if (this.whichFish == 79 || this.whichFish == 842)
            {
              @object = who.currentLocation.tryToCreateUnseenSecretNote(this.lastUser);
              if (@object == null)
                return;
            }
            if (this.caughtDoubleFish)
              @object.Stack = 2;
          }
          else if (this.itemCategory == "Furniture")
            @object = (StardewValley.Object) new Furniture(this.whichFish, Vector2.Zero);
          bool fromFishPond = this.fromFishPond;
          this.lastUser.completelyStopAnimatingOrDoingAction();
          this.doneFishing(this.lastUser, !fromFishPond);
          if (!Game1.isFestival() && !fromFishPond && this.itemCategory == "Object" && Game1.player.team.specialOrders != null)
          {
            foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
            {
              if (specialOrder.onFishCaught != null)
                specialOrder.onFishCaught(Game1.player, (Item) @object);
            }
          }
          if (Game1.isFestival() || this.lastUser.addItemToInventoryBool((Item) @object))
            return;
          Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) new List<Item>()
          {
            (Item) @object
          }, (object) this).setEssential(true);
        }
        else
        {
          this.fishCaught = false;
          this.showingTreasure = true;
          who.UsingTool = true;
          int initialStack = 1;
          if (this.caughtDoubleFish)
            initialStack = 2;
          StardewValley.Object @object = new StardewValley.Object(this.whichFish, initialStack, quality: this.fishQuality);
          if (Game1.player.team.specialOrders != null)
          {
            foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
            {
              if (specialOrder.onFishCaught != null)
                specialOrder.onFishCaught(Game1.player, (Item) @object);
            }
          }
          bool inventoryBool = this.lastUser.addItemToInventoryBool((Item) @object);
          this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(64, 1920, 32, 32), 500f, 1, 0, this.lastUser.Position + new Vector2(-32f, -160f), false, false, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
          {
            motion = new Vector2(0.0f, -0.128f),
            timeBasedMotion = true,
            endFunction = new TemporaryAnimatedSprite.endBehavior(this.openChestEndFunction),
            extraInfoForEndBehavior = inventoryBool ? 0 : 1,
            alpha = 0.0f,
            alphaFade = -1f / 500f
          });
        }
      }
      else if (who.UsingTool && this.castedButBobberStillInAir && this.doneWithAnimation)
      {
        switch (who.FacingDirection)
        {
          case 0:
            who.FarmerSprite.setCurrentFrame(39);
            break;
          case 1:
            who.FarmerSprite.setCurrentFrame(89);
            break;
          case 2:
            who.FarmerSprite.setCurrentFrame(28);
            break;
          case 3:
            who.FarmerSprite.setCurrentFrame(89, 0, 10, 1, true, false);
            break;
        }
        who.armOffset.Y = (float) Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
      }
      else
      {
        if (this.castedButBobberStillInAir || this.whichFish == -1 || this.animations.Count <= 0 || (double) this.animations[0].timer <= 500.0 || Game1.eventUp)
          return;
        this.lastUser.faceDirection(2);
        this.lastUser.FarmerSprite.setCurrentFrame(57);
      }
    }

    private void startCasting() => this.startCastingEvent.Fire();

    public void beginReeling() => this.isReeling = true;

    private void doStartCasting()
    {
      if (FishingRod.chargeSound != null && this.lastUser.IsLocalPlayer)
      {
        FishingRod.chargeSound.Stop(AudioStopOptions.Immediate);
        FishingRod.chargeSound = (ICue) null;
      }
      if (this.lastUser.currentLocation == null)
        return;
      if (this.lastUser.IsLocalPlayer)
      {
        this.lastUser.currentLocation.localSound("button1");
        Rumble.rumble(0.5f, 150f);
      }
      this.lastUser.UsingTool = true;
      this.isTimingCast = false;
      this.isCasting = true;
      this.castingChosenCountdown = 350f;
      this.lastUser.armOffset.Y = 0.0f;
      if ((double) this.castingPower <= 0.990000009536743 || !this.lastUser.IsLocalPlayer)
        return;
      Game1.screenOverlayTempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(545, 1921, 53, 19), 800f, 1, 0, Game1.GlobalToLocal(Game1.viewport, this.lastUser.Position + new Vector2(0.0f, -192f)), false, false, 1f, 0.01f, Color.White, 2f, 0.0f, 0.0f, 0.0f, true)
      {
        motion = new Vector2(0.0f, -4f),
        acceleration = new Vector2(0.0f, 0.2f),
        delayBeforeAnimationStart = 200
      });
      DelayedAction.playSoundAfterDelay("crit", 200);
    }

    public void openChestEndFunction(int extra)
    {
      this.lastUser.currentLocation.localSound("openChest");
      this.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(64, 1920, 32, 32), 200f, 4, 0, this.lastUser.Position + new Vector2(-32f, -228f), false, false, (float) ((double) this.lastUser.getStandingY() / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
      {
        endFunction = new TemporaryAnimatedSprite.endBehavior(this.openTreasureMenuEndFunction),
        extraInfoForEndBehavior = extra
      });
      this.sparklingText = (SparklingText) null;
    }

    public override bool doesShowTileLocationMarker() => false;

    public void openTreasureMenuEndFunction(int extra)
    {
      this.lastUser.gainExperience(5, 10 * (this.clearWaterDistance + 1));
      this.lastUser.UsingTool = false;
      int initialStack = 1;
      if (this.caughtDoubleFish)
        initialStack = 2;
      this.lastUser.completelyStopAnimatingOrDoingAction();
      this.doneFishing(this.lastUser, true);
      List<Item> objList1 = new List<Item>();
      if (extra == 1)
        objList1.Add((Item) new StardewValley.Object(this.whichFish, initialStack, quality: this.fishQuality));
      float num1 = 1f;
      while (Game1.random.NextDouble() <= (double) num1)
      {
        num1 *= 0.4f;
        if (Game1.currentSeason.Equals("spring") && !(this.lastUser.currentLocation is Beach) && Game1.random.NextDouble() < 0.1)
          objList1.Add((Item) new StardewValley.Object(273, Game1.random.Next(2, 6) + (Game1.random.NextDouble() < 0.25 ? 5 : 0)));
        if (this.caughtDoubleFish && Game1.random.NextDouble() < 0.5)
          objList1.Add((Item) new StardewValley.Object(774, 2 + (Game1.random.NextDouble() < 0.25 ? 2 : 0)));
        if (Game1.random.NextDouble() <= 0.33 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
          objList1.Add((Item) new StardewValley.Object(890, Game1.random.Next(1, 3) + (Game1.random.NextDouble() < 0.25 ? 2 : 0)));
        switch (Game1.random.Next(4))
        {
          case 0:
            if (this.clearWaterDistance >= 5 && Game1.random.NextDouble() < 0.03)
            {
              objList1.Add((Item) new StardewValley.Object(386, Game1.random.Next(1, 3)));
              continue;
            }
            List<int> source = new List<int>();
            if (this.clearWaterDistance >= 4)
              source.Add(384);
            if (this.clearWaterDistance >= 3 && (source.Count == 0 || Game1.random.NextDouble() < 0.6))
              source.Add(380);
            if (source.Count == 0 || Game1.random.NextDouble() < 0.6)
              source.Add(378);
            if (source.Count == 0 || Game1.random.NextDouble() < 0.6)
              source.Add(388);
            if (source.Count == 0 || Game1.random.NextDouble() < 0.6)
              source.Add(390);
            source.Add(382);
            objList1.Add((Item) new StardewValley.Object(source.ElementAt<int>(Game1.random.Next(source.Count)), Game1.random.Next(2, 7) * (Game1.random.NextDouble() < 0.05 + (double) (int) (NetFieldBase<int, NetInt>) this.lastUser.luckLevel * 0.015 ? 2 : 1)));
            if (Game1.random.NextDouble() < 0.05 + (double) this.lastUser.LuckLevel * 0.03)
            {
              objList1.Last<Item>().Stack *= 2;
              continue;
            }
            continue;
          case 1:
            if (this.clearWaterDistance >= 4 && Game1.random.NextDouble() < 0.1 && this.lastUser.FishingLevel >= 6)
            {
              objList1.Add((Item) new StardewValley.Object(687, 1));
              continue;
            }
            if (Game1.random.NextDouble() < 0.25 && this.lastUser.craftingRecipes.ContainsKey("Wild Bait"))
            {
              objList1.Add((Item) new StardewValley.Object(774, 5 + (Game1.random.NextDouble() < 0.25 ? 5 : 0)));
              continue;
            }
            if (this.lastUser.FishingLevel >= 6)
            {
              objList1.Add((Item) new StardewValley.Object(685, 1));
              continue;
            }
            objList1.Add((Item) new StardewValley.Object(685, 10));
            continue;
          case 2:
            if (Game1.random.NextDouble() < 0.1 && (int) (NetFieldBase<int, NetIntDelta>) Game1.netWorldState.Value.LostBooksFound < 21 && this.lastUser != null && this.lastUser.hasOrWillReceiveMail("lostBookFound"))
            {
              objList1.Add((Item) new StardewValley.Object(102, 1));
              continue;
            }
            if (this.lastUser.archaeologyFound.Count() > 0)
            {
              if (Game1.random.NextDouble() < 0.25 && this.lastUser.FishingLevel > 1)
              {
                objList1.Add((Item) new StardewValley.Object(Game1.random.Next(585, 588), 1));
                continue;
              }
              if (Game1.random.NextDouble() < 0.5 && this.lastUser.FishingLevel > 1)
              {
                objList1.Add((Item) new StardewValley.Object(Game1.random.Next(103, 120), 1));
                continue;
              }
              objList1.Add((Item) new StardewValley.Object(535, 1));
              continue;
            }
            objList1.Add((Item) new StardewValley.Object(382, Game1.random.Next(1, 3)));
            continue;
          case 3:
            switch (Game1.random.Next(3))
            {
              case 0:
                if (this.clearWaterDistance >= 4)
                  objList1.Add((Item) new StardewValley.Object(537 + (Game1.random.NextDouble() < 0.4 ? Game1.random.Next(-2, 0) : 0), Game1.random.Next(1, 4)));
                else if (this.clearWaterDistance >= 3)
                  objList1.Add((Item) new StardewValley.Object(536 + (Game1.random.NextDouble() < 0.4 ? -1 : 0), Game1.random.Next(1, 4)));
                else
                  objList1.Add((Item) new StardewValley.Object(535, Game1.random.Next(1, 4)));
                if (Game1.random.NextDouble() < 0.05 + (double) this.lastUser.LuckLevel * 0.03)
                {
                  objList1.Last<Item>().Stack *= 2;
                  continue;
                }
                continue;
              case 1:
                if (this.lastUser.FishingLevel < 2)
                {
                  objList1.Add((Item) new StardewValley.Object(382, Game1.random.Next(1, 4)));
                  continue;
                }
                if (this.clearWaterDistance >= 4)
                  objList1.Add((Item) new StardewValley.Object(Game1.random.NextDouble() < 0.3 ? 82 : (Game1.random.NextDouble() < 0.5 ? 64 : 60), Game1.random.Next(1, 3)));
                else if (this.clearWaterDistance >= 3)
                  objList1.Add((Item) new StardewValley.Object(Game1.random.NextDouble() < 0.3 ? 84 : (Game1.random.NextDouble() < 0.5 ? 70 : 62), Game1.random.Next(1, 3)));
                else
                  objList1.Add((Item) new StardewValley.Object(Game1.random.NextDouble() < 0.3 ? 86 : (Game1.random.NextDouble() < 0.5 ? 66 : 68), Game1.random.Next(1, 3)));
                if (Game1.random.NextDouble() < 0.028 * ((double) this.clearWaterDistance / 5.0))
                  objList1.Add((Item) new StardewValley.Object(72, 1));
                if (Game1.random.NextDouble() < 0.05)
                {
                  objList1.Last<Item>().Stack *= 2;
                  continue;
                }
                continue;
              case 2:
                if (this.lastUser.FishingLevel < 2)
                {
                  objList1.Add((Item) new StardewValley.Object(770, Game1.random.Next(1, 4)));
                  continue;
                }
                float num2 = (float) ((1.0 + this.lastUser.DailyLuck) * ((double) this.clearWaterDistance / 5.0));
                if (Game1.random.NextDouble() < 0.05 * (double) num2 && !this.lastUser.specialItems.Contains(14))
                {
                  List<Item> objList2 = objList1;
                  MeleeWeapon meleeWeapon = new MeleeWeapon(14);
                  meleeWeapon.specialItem = true;
                  objList2.Add((Item) meleeWeapon);
                }
                if (Game1.random.NextDouble() < 0.05 * (double) num2 && !this.lastUser.specialItems.Contains(51))
                {
                  List<Item> objList3 = objList1;
                  MeleeWeapon meleeWeapon = new MeleeWeapon(51);
                  meleeWeapon.specialItem = true;
                  objList3.Add((Item) meleeWeapon);
                }
                if (Game1.random.NextDouble() < 0.07 * (double) num2)
                {
                  switch (Game1.random.Next(3))
                  {
                    case 0:
                      objList1.Add((Item) new Ring(516 + (Game1.random.NextDouble() < (double) this.lastUser.LuckLevel / 11.0 ? 1 : 0)));
                      break;
                    case 1:
                      objList1.Add((Item) new Ring(518 + (Game1.random.NextDouble() < (double) this.lastUser.LuckLevel / 11.0 ? 1 : 0)));
                      break;
                    case 2:
                      objList1.Add((Item) new Ring(Game1.random.Next(529, 535)));
                      break;
                  }
                }
                if (Game1.random.NextDouble() < 0.02 * (double) num2)
                  objList1.Add((Item) new StardewValley.Object(166, 1));
                if (this.lastUser.FishingLevel > 5 && Game1.random.NextDouble() < 0.001 * (double) num2)
                  objList1.Add((Item) new StardewValley.Object(74, 1));
                if (Game1.random.NextDouble() < 0.01 * (double) num2)
                  objList1.Add((Item) new StardewValley.Object((int) sbyte.MaxValue, 1));
                if (Game1.random.NextDouble() < 0.01 * (double) num2)
                  objList1.Add((Item) new StardewValley.Object(126, 1));
                if (Game1.random.NextDouble() < 0.01 * (double) num2)
                  objList1.Add((Item) new Ring(527));
                if (Game1.random.NextDouble() < 0.01 * (double) num2)
                  objList1.Add((Item) new Boots(Game1.random.Next(504, 514)));
                if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && Game1.random.NextDouble() < 0.01 * (double) num2)
                  objList1.Add((Item) new StardewValley.Object(928, 1));
                if (objList1.Count == 1)
                {
                  objList1.Add((Item) new StardewValley.Object(72, 1));
                  continue;
                }
                continue;
              default:
                continue;
            }
          default:
            continue;
        }
      }
      if (objList1.Count == 0)
        objList1.Add((Item) new StardewValley.Object(685, Game1.random.Next(1, 4) * 5));
      Game1.activeClickableMenu = (IClickableMenu) new ItemGrabMenu((IList<Item>) objList1, (object) this).setEssential(true);
      (Game1.activeClickableMenu as ItemGrabMenu).source = 3;
      this.lastUser.completelyStopAnimatingOrDoingAction();
    }
  }
}
