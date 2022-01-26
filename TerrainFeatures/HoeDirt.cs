// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.HoeDirt
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  public class HoeDirt : TerrainFeature
  {
    public const float defaultShakeRate = 0.03926991f;
    public const float maximumShake = 0.3926991f;
    public const float shakeDecayRate = 0.01047198f;
    public const byte N = 1;
    public const byte E = 2;
    public const byte S = 4;
    public const byte W = 8;
    public const byte Cardinals = 15;
    public static readonly Vector2 N_Offset = new Vector2(0.0f, -1f);
    public static readonly Vector2 E_Offset = new Vector2(1f, 0.0f);
    public static readonly Vector2 S_Offset = new Vector2(0.0f, 1f);
    public static readonly Vector2 W_Offset = new Vector2(-1f, 0.0f);
    public const float paddyGrowBonus = 0.25f;
    public const int dry = 0;
    public const int watered = 1;
    public const int invisible = 2;
    public const int noFertilizer = 0;
    public const int fertilizerLowQuality = 368;
    public const int fertilizerHighQuality = 369;
    public const int waterRetentionSoil = 370;
    public const int waterRetentionSoilQuality = 371;
    public const int speedGro = 465;
    public const int superSpeedGro = 466;
    public const int hyperSpeedGro = 918;
    public const int fertilizerDeluxeQuality = 919;
    public const int waterRetentionSoilDeluxe = 920;
    public static Texture2D lightTexture;
    public static Texture2D darkTexture;
    public static Texture2D snowTexture;
    private readonly NetRef<Crop> netCrop = new NetRef<Crop>();
    public static Dictionary<byte, int> drawGuide;
    [XmlElement("state")]
    public readonly NetInt state = new NetInt();
    [XmlElement("fertilizer")]
    public readonly NetInt fertilizer = new NetInt();
    private bool shakeLeft;
    private float shakeRotation;
    private float maxShake;
    private float shakeRate;
    [XmlElement("c")]
    private readonly NetColor c = new NetColor(Color.White);
    private List<Action<GameLocation, Vector2>> queuedActions = new List<Action<GameLocation, Vector2>>();
    [XmlElement("isGreenhouseDirt")]
    public readonly NetBool isGreenhouseDirt = new NetBool(false);
    private byte neighborMask;
    private byte wateredNeighborMask;
    [XmlIgnore]
    public NetInt nearWaterForPaddy = new NetInt(-1);
    private byte drawSum;
    private int sourceRectPosition;
    private int wateredRectPosition;
    private Texture2D texture;
    private static readonly HoeDirt.NeighborLoc[] _offsets = new HoeDirt.NeighborLoc[4]
    {
      new HoeDirt.NeighborLoc(HoeDirt.N_Offset, (byte) 1, (byte) 4),
      new HoeDirt.NeighborLoc(HoeDirt.S_Offset, (byte) 4, (byte) 1),
      new HoeDirt.NeighborLoc(HoeDirt.E_Offset, (byte) 2, (byte) 8),
      new HoeDirt.NeighborLoc(HoeDirt.W_Offset, (byte) 8, (byte) 2)
    };
    private List<HoeDirt.Neighbor> _neighbors = new List<HoeDirt.Neighbor>();

    public Crop crop
    {
      get => (Crop) (NetFieldBase<Crop, NetRef<Crop>>) this.netCrop;
      set => this.netCrop.Value = value;
    }

    public HoeDirt()
      : base(true)
    {
      this.NetFields.AddFields((INetSerializable) this.netCrop, (INetSerializable) this.state, (INetSerializable) this.fertilizer, (INetSerializable) this.c, (INetSerializable) this.isGreenhouseDirt, (INetSerializable) this.nearWaterForPaddy);
      this.state.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((x, y, z) => this.OnAdded(this.currentLocation, this.currentTileLocation));
      this.netCrop.fieldChangeVisibleEvent += (NetFieldBase<Crop, NetRef<Crop>>.FieldChange) ((x, y, z) =>
      {
        this.nearWaterForPaddy.Value = -1;
        this.updateNeighbors(this.currentLocation, this.currentTileLocation);
        if (this.netCrop.Value == null)
          return;
        this.netCrop.Value.updateDrawMath(this.currentTileLocation);
      });
      this.loadSprite();
      if (HoeDirt.drawGuide == null)
        HoeDirt.populateDrawGuide();
      this.initialize(Game1.currentLocation);
      this.nearWaterForPaddy.Interpolated(false, false);
      this.netCrop.Interpolated(false, false);
      this.netCrop.OnConflictResolve += (NetRefBase<Crop, NetRef<Crop>>.ConflictResolveEvent) ((rejected, accepted) =>
      {
        if (!Game1.IsMasterGame || rejected == null || rejected.netSeedIndex.Value == -1)
          return;
        this.queuedActions.Add((Action<GameLocation, Vector2>) ((gLocation, tileLocation) =>
        {
          Vector2 vector2 = tileLocation * 64f;
          gLocation.debris.Add(new Debris((int) (NetFieldBase<int, NetInt>) rejected.netSeedIndex, vector2, vector2));
        }));
        this.NeedsUpdate = true;
      });
    }

    public HoeDirt(int startingState, GameLocation location = null)
      : this()
    {
      this.state.Value = startingState;
      if (location == null)
        return;
      this.initialize(location);
    }

    public HoeDirt(int startingState, Crop crop)
      : this()
    {
      this.state.Value = startingState;
      this.crop = crop;
    }

    private void initialize(GameLocation location)
    {
      if (location == null)
        location = Game1.currentLocation;
      if (location == null)
        return;
      if (location is MineShaft)
      {
        if ((location as MineShaft).GetAdditionalDifficulty() > 0)
        {
          if ((location as MineShaft).getMineArea() == 0 || (location as MineShaft).getMineArea() == 10)
            this.c.Value = new Color(80, 100, 140) * 0.5f;
        }
        else if ((location as MineShaft).getMineArea() == 80)
          this.c.Value = Color.MediumPurple * 0.4f;
      }
      else
      {
        if (location.GetSeasonForLocation() == "fall" && location.IsOutdoors)
        {
          switch (location)
          {
            case Beach _:
            case Desert _:
              break;
            default:
              this.c.Value = new Color(250, 210, 240);
              goto label_14;
          }
        }
        if (location is VolcanoDungeon)
          this.c.Value = Color.MediumPurple * 0.7f;
      }
label_14:
      this.isGreenhouseDirt.Value = location.IsGreenhouse;
    }

    public float getShakeRotation() => this.shakeRotation;

    public float getMaxShake() => this.maxShake;

    public override Rectangle getBoundingBox(Vector2 tileLocation) => new Rectangle((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.Y * 64.0), 64, 64);

    public override void doCollisionAction(
      Rectangle positionOfCollider,
      int speedOfCollision,
      Vector2 tileLocation,
      Character who,
      GameLocation location)
    {
      if (this.crop != null && (int) (NetFieldBase<int, NetInt>) this.crop.currentPhase != 0 && speedOfCollision > 0 && (double) this.maxShake == 0.0 && positionOfCollider.Intersects(this.getBoundingBox(tileLocation)) && Utility.isOnScreen(Utility.Vector2ToPoint(tileLocation), 64, location))
      {
        if (Game1.soundBank != null && (who == null || !(who is FarmAnimal)) && !Grass.grassSound.IsPlaying)
        {
          Grass.grassSound = Game1.soundBank.GetCue("grassyStep");
          Grass.grassSound.Play();
        }
        this.shake((float) (0.392699092626572 / (double) ((5 + Game1.player.addedSpeed) / speedOfCollision) - (speedOfCollision > 2 ? (double) (int) (NetFieldBase<int, NetInt>) this.crop.currentPhase * 3.14159274101257 / 64.0 : 0.0)), (float) Math.PI / 80f / (float) ((5 + Game1.player.addedSpeed) / speedOfCollision), (double) positionOfCollider.Center.X > (double) tileLocation.X * 64.0 + 32.0);
      }
      if (this.crop == null || (int) (NetFieldBase<int, NetInt>) this.crop.currentPhase == 0 || !(who is Farmer) || !(who as Farmer).running)
        return;
      (who as Farmer).temporarySpeedBuff = -1f;
    }

    public void shake(float shake, float rate, bool left)
    {
      if (this.crop != null)
      {
        this.maxShake = shake * ((bool) (NetFieldBase<bool, NetBool>) this.crop.raisedSeeds ? 0.6f : 1.5f);
        this.shakeRate = rate * 0.5f;
        this.shakeRotation = 0.0f;
        this.shakeLeft = left;
      }
      this.NeedsUpdate = true;
    }

    public bool needsWatering()
    {
      if (this.crop == null)
        return false;
      return !this.readyForHarvest() || (int) (NetFieldBase<int, NetInt>) this.crop.regrowAfterHarvest != -1;
    }

    public static void populateDrawGuide()
    {
      HoeDirt.drawGuide = new Dictionary<byte, int>();
      HoeDirt.drawGuide.Add((byte) 0, 0);
      HoeDirt.drawGuide.Add((byte) 8, 15);
      HoeDirt.drawGuide.Add((byte) 2, 13);
      HoeDirt.drawGuide.Add((byte) 1, 12);
      HoeDirt.drawGuide.Add((byte) 4, 4);
      HoeDirt.drawGuide.Add((byte) 9, 11);
      HoeDirt.drawGuide.Add((byte) 3, 9);
      HoeDirt.drawGuide.Add((byte) 5, 8);
      HoeDirt.drawGuide.Add((byte) 6, 1);
      HoeDirt.drawGuide.Add((byte) 12, 3);
      HoeDirt.drawGuide.Add((byte) 10, 14);
      HoeDirt.drawGuide.Add((byte) 7, 5);
      HoeDirt.drawGuide.Add((byte) 15, 6);
      HoeDirt.drawGuide.Add((byte) 13, 7);
      HoeDirt.drawGuide.Add((byte) 11, 10);
      HoeDirt.drawGuide.Add((byte) 14, 2);
    }

    public override void loadSprite()
    {
      if (HoeDirt.lightTexture == null)
      {
        try
        {
          HoeDirt.lightTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirt");
        }
        catch (Exception ex)
        {
        }
      }
      if (HoeDirt.darkTexture == null)
      {
        try
        {
          HoeDirt.darkTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirtDark");
        }
        catch (Exception ex)
        {
        }
      }
      if (HoeDirt.snowTexture == null)
      {
        try
        {
          HoeDirt.snowTexture = Game1.content.Load<Texture2D>("TerrainFeatures\\hoeDirtSnow");
        }
        catch (Exception ex)
        {
        }
      }
      this.nearWaterForPaddy.Value = -1;
      if (this.crop == null)
        return;
      this.crop.updateDrawMath(this.currentTileLocation);
    }

    public override bool isPassable(Character c) => this.crop == null || !(bool) (NetFieldBase<bool, NetBool>) this.crop.raisedSeeds || c is JunimoHarvester;

    public bool readyForHarvest()
    {
      if (this.crop == null || (bool) (NetFieldBase<bool, NetBool>) this.crop.fullyGrown && (int) (NetFieldBase<int, NetInt>) this.crop.dayOfCurrentPhase > 0 || (int) (NetFieldBase<int, NetInt>) this.crop.currentPhase < this.crop.phaseDays.Count - 1 || (bool) (NetFieldBase<bool, NetBool>) this.crop.dead)
        return false;
      return !(bool) (NetFieldBase<bool, NetBool>) this.crop.forageCrop || (int) (NetFieldBase<int, NetInt>) this.crop.whichForageCrop != 2;
    }

    public override bool performUseAction(Vector2 tileLocation, GameLocation location)
    {
      if (this.crop == null)
        return false;
      bool flag = (int) (NetFieldBase<int, NetInt>) this.crop.currentPhase >= this.crop.phaseDays.Count - 1 && (!(bool) (NetFieldBase<bool, NetBool>) this.crop.fullyGrown || (int) (NetFieldBase<int, NetInt>) this.crop.dayOfCurrentPhase <= 0);
      if ((int) (NetFieldBase<int, NetInt>) this.crop.harvestMethod == 0 && this.crop.harvest((int) tileLocation.X, (int) tileLocation.Y, this))
      {
        if (location != null && location is IslandLocation && Game1.random.NextDouble() < 0.05)
          Game1.player.team.RequestLimitedNutDrops("IslandFarming", location, (int) tileLocation.X * 64, (int) tileLocation.Y * 64, 5);
        this.destroyCrop(tileLocation, false, location);
        return true;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.crop.harvestMethod == 1 && this.readyForHarvest())
      {
        if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon && (Game1.player.CurrentTool as MeleeWeapon).isScythe())
        {
          Game1.player.CanMove = false;
          Game1.player.UsingTool = true;
          Game1.player.canReleaseTool = true;
          Game1.player.Halt();
          try
          {
            Game1.player.CurrentTool.beginUsing(Game1.currentLocation, (int) Game1.player.lastClick.X, (int) Game1.player.lastClick.Y, Game1.player);
          }
          catch (Exception ex)
          {
          }
          ((MeleeWeapon) Game1.player.CurrentTool).setFarmerAnimating(Game1.player);
        }
        else
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13915"));
      }
      return flag;
    }

    public bool plant(
      int index,
      int tileX,
      int tileY,
      Farmer who,
      bool isFertilizer,
      GameLocation location)
    {
      if (isFertilizer)
      {
        if (this.crop != null && (int) (NetFieldBase<int, NetInt>) this.crop.currentPhase != 0 && (index == 368 || index == 369) || (int) (NetFieldBase<int, NetInt>) this.fertilizer != 0)
          return false;
        this.fertilizer.Value = index;
        this.applySpeedIncreases(who);
        location.playSound("dirtyHit");
        return true;
      }
      Crop crop = new Crop(index, tileX, tileY);
      if (crop.seasonsToGrowIn.Count == 0)
        return false;
      if (!(bool) (NetFieldBase<bool, NetBool>) who.currentLocation.isFarm && !who.currentLocation.IsGreenhouse && !who.currentLocation.CanPlantSeedsHere(index, tileX, tileY) && who.currentLocation.IsOutdoors)
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13919"));
        return false;
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) who.currentLocation.isOutdoors || who.currentLocation.IsGreenhouse || crop.seasonsToGrowIn.Contains(location.GetSeasonForLocation()) || who.currentLocation.SeedsIgnoreSeasonsHere())
      {
        this.crop = crop;
        if ((bool) (NetFieldBase<bool, NetBool>) crop.raisedSeeds)
          location.playSound("stoneStep");
        location.playSound("dirtyHit");
        ++Game1.stats.SeedsSown;
        this.applySpeedIncreases(who);
        this.nearWaterForPaddy.Value = -1;
        if (this.hasPaddyCrop() && this.paddyWaterCheck(location, new Vector2((float) tileX, (float) tileY)))
        {
          this.state.Value = 1;
          this.updateNeighbors(location, new Vector2((float) tileX, (float) tileY));
        }
        return true;
      }
      if (crop.seasonsToGrowIn.Count > 0 && !crop.seasonsToGrowIn.Contains(location.GetSeasonForLocation()))
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13924"));
      else
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13925"));
      return false;
    }

    protected void applySpeedIncreases(Farmer who)
    {
      if (this.crop == null)
        return;
      bool flag = false;
      if (this.currentLocation != null && this.paddyWaterCheck(this.currentLocation, this.currentTileLocation))
        flag = true;
      if ((((int) (NetFieldBase<int, NetInt>) this.fertilizer == 465 || (int) (NetFieldBase<int, NetInt>) this.fertilizer == 466 || (int) (NetFieldBase<int, NetInt>) this.fertilizer == 918 ? 1 : (who.professions.Contains(5) ? 1 : 0)) | (flag ? 1 : 0)) == 0)
        return;
      this.crop.ResetPhaseDays();
      int num1 = 0;
      for (int index = 0; index < this.crop.phaseDays.Count - 1; ++index)
        num1 += this.crop.phaseDays[index];
      float num2 = 0.0f;
      if ((int) (NetFieldBase<int, NetInt>) this.fertilizer == 465)
        num2 += 0.1f;
      else if ((int) (NetFieldBase<int, NetInt>) this.fertilizer == 466)
        num2 += 0.25f;
      else if ((int) (NetFieldBase<int, NetInt>) this.fertilizer == 918)
        num2 += 0.33f;
      if (flag)
        num2 += 0.25f;
      if (who.professions.Contains(5))
        num2 += 0.1f;
      int num3 = (int) Math.Ceiling((double) num1 * (double) num2);
      for (int index1 = 0; num3 > 0 && index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < this.crop.phaseDays.Count; ++index2)
        {
          if ((index2 > 0 || this.crop.phaseDays[index2] > 1) && this.crop.phaseDays[index2] != 99999)
          {
            this.crop.phaseDays[index2]--;
            --num3;
          }
          if (num3 <= 0)
            break;
        }
      }
    }

    public void destroyCrop(Vector2 tileLocation, bool showAnimation, GameLocation location)
    {
      if (this.crop != null & showAnimation && location != null)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.crop.currentPhase < 1 && !(bool) (NetFieldBase<bool, NetBool>) this.crop.dead)
        {
          Game1.multiplayer.broadcastSprites(Game1.player.currentLocation, new TemporaryAnimatedSprite(12, tileLocation * 64f, Color.White));
          location.playSound("dirtyHit");
        }
        else
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(50, tileLocation * 64f, (bool) (NetFieldBase<bool, NetBool>) this.crop.dead ? new Color(207, 193, 43) : Color.ForestGreen));
      }
      this.crop = (Crop) null;
      this.nearWaterForPaddy.Value = -1;
      if (location == null)
        return;
      this.updateNeighbors(location, tileLocation);
    }

    public override bool performToolAction(
      Tool t,
      int damage,
      Vector2 tileLocation,
      GameLocation location)
    {
      switch (t)
      {
        case null:
          if (damage > 0 && this.crop != null)
          {
            if (damage == 50)
            {
              this.crop.Kill();
              goto label_19;
            }
            else
            {
              this.destroyCrop(tileLocation, true, location);
              goto label_19;
            }
          }
          else
            goto label_19;
        case Hoe _:
          if (this.crop != null && this.crop.hitWithHoe((int) tileLocation.X, (int) tileLocation.Y, location, this))
          {
            this.destroyCrop(tileLocation, true, location);
            break;
          }
          break;
        case Pickaxe _ when this.crop == null:
          return true;
        case WateringCan _:
          this.state.Value = 1;
          break;
        case MeleeWeapon _ when (t as MeleeWeapon).isScythe():
          if (this.crop != null && (int) (NetFieldBase<int, NetInt>) this.crop.harvestMethod == 1)
          {
            if ((int) (NetFieldBase<int, NetInt>) this.crop.indexOfHarvest == 771 && (t as MeleeWeapon).hasEnchantmentOfType<HaymakerEnchantment>())
            {
              Game1.createItemDebris((Item) new StardewValley.Object(771, 1), new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 32.0)), -1);
              Game1.createItemDebris((Item) new StardewValley.Object(771, 1), new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 32.0)), -1);
            }
            if (this.crop.harvest((int) tileLocation.X, (int) tileLocation.Y, this))
              this.destroyCrop(tileLocation, true, location);
          }
          if (this.crop != null && (bool) (NetFieldBase<bool, NetBool>) this.crop.dead)
          {
            this.destroyCrop(tileLocation, true, location);
            break;
          }
          break;
        default:
          if (t.isHeavyHitter() && !(t is Hoe) && !(t is MeleeWeapon) && this.crop != null)
          {
            this.destroyCrop(tileLocation, true, location);
            break;
          }
          break;
      }
      this.shake((float) Math.PI / 32f, (float) Math.PI / 40f, (double) tileLocation.X * 64.0 < (double) Game1.player.Position.X);
label_19:
      return false;
    }

    public bool canPlantThisSeedHere(int objectIndex, int tileX, int tileY, bool isFertilizer = false)
    {
      if (isFertilizer)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.fertilizer == 0)
          return true;
      }
      else if (this.crop == null)
      {
        Crop crop = new Crop(objectIndex, tileX, tileY);
        if (crop.seasonsToGrowIn.Count == 0)
          return false;
        if (!Game1.currentLocation.IsOutdoors || Game1.currentLocation.IsGreenhouse || Game1.currentLocation.SeedsIgnoreSeasonsHere() || crop.seasonsToGrowIn.Contains(Game1.currentLocation.GetSeasonForLocation()))
          return !(bool) (NetFieldBase<bool, NetBool>) crop.raisedSeeds || !Utility.doesRectangleIntersectTile(Game1.player.GetBoundingBox(), tileX, tileY);
        if (objectIndex == 309 || objectIndex == 310 || objectIndex == 311)
          return true;
        if (Game1.didPlayerJustClickAtAll() && !Game1.doesHUDMessageExist(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13924")))
        {
          Game1.playSound("cancel");
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13924"));
        }
      }
      return false;
    }

    public override void performPlayerEntryAction(Vector2 tileLocation)
    {
      base.performPlayerEntryAction(tileLocation);
      if (this.crop == null)
        return;
      this.crop.updateDrawMath(tileLocation);
    }

    public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
    {
      foreach (Action<GameLocation, Vector2> queuedAction in this.queuedActions)
        queuedAction(location, tileLocation);
      this.queuedActions.Clear();
      if ((double) this.maxShake > 0.0)
      {
        if (this.shakeLeft)
        {
          this.shakeRotation -= this.shakeRate;
          if ((double) Math.Abs(this.shakeRotation) >= (double) this.maxShake)
            this.shakeLeft = false;
        }
        else
        {
          this.shakeRotation += this.shakeRate;
          if ((double) this.shakeRotation >= (double) this.maxShake)
          {
            this.shakeLeft = true;
            this.shakeRotation -= this.shakeRate;
          }
        }
        this.maxShake = Math.Max(0.0f, this.maxShake - (float) Math.PI / 300f);
      }
      else
      {
        this.shakeRotation /= 2f;
        if ((double) this.shakeRotation <= 0.00999999977648258)
        {
          this.NeedsUpdate = false;
          this.shakeRotation = 0.0f;
        }
      }
      return (int) (NetFieldBase<int, NetInt>) this.state == 2 && this.crop == null;
    }

    public bool hasPaddyCrop() => this.crop != null && this.crop.isPaddyCrop();

    public bool paddyWaterCheck(GameLocation location, Vector2 tile_location)
    {
      if (this.nearWaterForPaddy.Value >= 0)
        return this.nearWaterForPaddy.Value == 1;
      if (!this.hasPaddyCrop())
      {
        this.nearWaterForPaddy.Value = 0;
        return false;
      }
      if (location.getObjectAtTile((int) tile_location.X, (int) tile_location.Y) is IndoorPot)
      {
        this.nearWaterForPaddy.Value = 0;
        return false;
      }
      int num = 3;
      for (int index1 = -num; index1 <= num; ++index1)
      {
        for (int index2 = -num; index2 <= num; ++index2)
        {
          if (location.isWaterTile((int) ((double) tile_location.X + (double) index1), (int) ((double) tile_location.Y + (double) index2)))
          {
            this.nearWaterForPaddy.Value = 1;
            return true;
          }
        }
      }
      this.nearWaterForPaddy.Value = 0;
      return false;
    }

    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
      if (this.crop != null)
      {
        this.crop.newDay((int) (NetFieldBase<int, NetInt>) this.state, (int) (NetFieldBase<int, NetInt>) this.fertilizer, (int) tileLocation.X, (int) tileLocation.Y, environment);
        if ((bool) (NetFieldBase<bool, NetBool>) environment.isOutdoors && Game1.GetSeasonForLocation(environment).Equals("winter") && this.crop != null && !this.crop.isWildSeedCrop() && (int) (NetFieldBase<int, NetInt>) this.crop.indexOfHarvest != 771 && (int) (NetFieldBase<int, NetInt>) this.crop.indexOfHarvest != 889 && !environment.IsGreenhouse && !environment.SeedsIgnoreSeasonsHere())
          this.destroyCrop(tileLocation, false, environment);
      }
      if ((!this.hasPaddyCrop() || !this.paddyWaterCheck(environment, tileLocation)) && ((int) (NetFieldBase<int, NetInt>) this.fertilizer != 370 || Game1.random.NextDouble() >= 0.33) && ((int) (NetFieldBase<int, NetInt>) this.fertilizer != 371 || Game1.random.NextDouble() >= 0.66) && (int) (NetFieldBase<int, NetInt>) this.fertilizer != 920)
        this.state.Value = 0;
      if (!environment.IsGreenhouse)
        return;
      this.isGreenhouseDirt.Value = true;
      this.c.Value = Color.White;
    }

    public override bool seasonUpdate(bool onLoad)
    {
      if (!onLoad && !this.currentLocation.SeedsIgnoreSeasonsHere() && (this.crop == null || (bool) (NetFieldBase<bool, NetBool>) this.crop.dead || !this.crop.seasonsToGrowIn.Contains(Game1.currentLocation.GetSeasonForLocation())))
        this.fertilizer.Value = 0;
      if (Game1.currentLocation.GetSeasonForLocation() == "fall" && !this.isGreenhouseDirt.Value)
        this.c.Value = new Color(250, 210, 240);
      else
        this.c.Value = Color.White;
      this.texture = (Texture2D) null;
      return false;
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 positionOnScreen,
      Vector2 tileLocation,
      float scale,
      float layerDepth)
    {
      byte key1 = 0;
      Vector2 key2 = tileLocation;
      ++key2.X;
      GameLocation locationFromName = Game1.getLocationFromName("Farm");
      if (locationFromName.terrainFeatures.ContainsKey(key2) && locationFromName.terrainFeatures[key2] is HoeDirt)
        key1 += (byte) 2;
      key2.X -= 2f;
      if (locationFromName.terrainFeatures.ContainsKey(key2) && locationFromName.terrainFeatures[key2] is HoeDirt)
        key1 += (byte) 8;
      ++key2.X;
      ++key2.Y;
      if (Game1.currentLocation.terrainFeatures.ContainsKey(key2) && locationFromName.terrainFeatures[key2] is HoeDirt)
        key1 += (byte) 4;
      key2.Y -= 2f;
      if (locationFromName.terrainFeatures.ContainsKey(key2) && locationFromName.terrainFeatures[key2] is HoeDirt)
        ++key1;
      int num = HoeDirt.drawGuide[key1];
      spriteBatch.Draw(HoeDirt.lightTexture, positionOnScreen, new Rectangle?(new Rectangle(num % 4 * 64, num / 4 * 64, 64, 64)), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth + positionOnScreen.Y / 20000f);
      if (this.crop == null)
        return;
      this.crop.drawInMenu(spriteBatch, positionOnScreen + new Vector2(64f * scale, 64f * scale), Color.White, 0.0f, scale, layerDepth + (float) (((double) positionOnScreen.Y + 64.0 * (double) scale) / 20000.0));
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation) => this.DrawOptimized(spriteBatch, spriteBatch, spriteBatch, tileLocation);

    public void DrawOptimized(
      SpriteBatch dirt_batch,
      SpriteBatch fert_batch,
      SpriteBatch crop_batch,
      Vector2 tileLocation)
    {
      int num = this.state.Value;
      if (num != 2 && (dirt_batch != null || fert_batch != null))
      {
        if (dirt_batch != null && this.texture == null)
        {
          this.texture = Game1.currentLocation.Name.Equals("Mountain") || Game1.currentLocation.Name.Equals("Mine") || Game1.currentLocation is MineShaft && (Game1.currentLocation as MineShaft).shouldShowDarkHoeDirt() || Game1.currentLocation is VolcanoDungeon ? HoeDirt.darkTexture : HoeDirt.lightTexture;
          if (Game1.GetSeasonForLocation(Game1.currentLocation).Equals("winter") && !(Game1.currentLocation is Desert) && !Game1.currentLocation.IsGreenhouse && !Game1.currentLocation.SeedsIgnoreSeasonsHere() && !(Game1.currentLocation is MineShaft) || Game1.currentLocation is MineShaft && (Game1.currentLocation as MineShaft).shouldUseSnowTextureHoeDirt())
            this.texture = HoeDirt.snowTexture;
        }
        Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f));
        if (dirt_batch != null)
        {
          dirt_batch.Draw(this.texture, local, new Rectangle?(new Rectangle(this.sourceRectPosition % 4 * 16, this.sourceRectPosition / 4 * 16, 16, 16)), (Color) (NetFieldBase<Color, NetColor>) this.c, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
          if (num == 1)
            dirt_batch.Draw(this.texture, local, new Rectangle?(new Rectangle(this.wateredRectPosition % 4 * 16 + (this.paddyWaterCheck(Game1.currentLocation, tileLocation) ? 128 : 64), this.wateredRectPosition / 4 * 16, 16, 16)), (Color) (NetFieldBase<Color, NetColor>) this.c, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1.2E-08f);
        }
        if (fert_batch != null)
        {
          int fertilizer = this.fertilizer.Value;
          if (fertilizer != 0)
          {
            Rectangle fertilizerSourceRect = this.GetFertilizerSourceRect(fertilizer);
            fert_batch.Draw(Game1.mouseCursors, local, new Rectangle?(fertilizerSourceRect), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1.9E-08f);
          }
        }
      }
      if (this.crop == null || crop_batch == null)
        return;
      this.crop.draw(crop_batch, tileLocation, num != 1 || (int) (NetFieldBase<int, NetInt>) this.crop.currentPhase != 0 || !this.crop.shouldDrawDarkWhenWatered() ? Color.White : new Color(180, 100, 200) * 1f, this.shakeRotation);
    }

    public Rectangle GetFertilizerSourceRect(int fertilizer)
    {
      int num = 0;
      switch (fertilizer)
      {
        case 369:
          num = 1;
          break;
        case 370:
          num = 3;
          break;
        case 371:
          num = 4;
          break;
        case 465:
          num = 6;
          break;
        case 466:
          num = 7;
          break;
        case 918:
          num = 8;
          break;
        case 919:
          num = 2;
          break;
        case 920:
          num = 5;
          break;
      }
      return new Rectangle(173 + num / 3 * 16, 462 + num % 3 * 16, 16, 16);
    }

    private List<HoeDirt.Neighbor> gatherNeighbors(GameLocation loc, Vector2 tilePos)
    {
      List<HoeDirt.Neighbor> neighbors = this._neighbors;
      neighbors.Clear();
      TerrainFeature terrainFeature = (TerrainFeature) null;
      NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>> terrainFeatures = loc.terrainFeatures;
      foreach (HoeDirt.NeighborLoc offset in HoeDirt._offsets)
      {
        Vector2 key = tilePos + offset.Offset;
        if (terrainFeatures.TryGetValue(key, out terrainFeature) && terrainFeature != null && terrainFeature is HoeDirt a && a.state.Value != 2)
        {
          HoeDirt.Neighbor neighbor = new HoeDirt.Neighbor(a, offset.Direction, offset.InvDirection);
          neighbors.Add(neighbor);
        }
      }
      return neighbors;
    }

    public void updateNeighbors(GameLocation loc, Vector2 tilePos)
    {
      if (loc == null)
        return;
      List<HoeDirt.Neighbor> neighborList = this.gatherNeighbors(loc, tilePos);
      this.neighborMask = (byte) 0;
      this.wateredNeighborMask = (byte) 0;
      foreach (HoeDirt.Neighbor neighbor in neighborList)
      {
        this.neighborMask |= neighbor.direction;
        if ((int) (NetFieldBase<int, NetInt>) this.state != 2)
          neighbor.feature.OnNeighborAdded(neighbor.invDirection);
        if ((int) (NetFieldBase<int, NetInt>) this.state == 1 && (int) (NetFieldBase<int, NetInt>) neighbor.feature.state == 1)
        {
          if (neighbor.feature.paddyWaterCheck(neighbor.feature.currentLocation, neighbor.feature.currentTileLocation) == this.paddyWaterCheck(loc, tilePos))
          {
            this.wateredNeighborMask |= neighbor.direction;
            neighbor.feature.wateredNeighborMask |= neighbor.invDirection;
          }
          else
            neighbor.feature.wateredNeighborMask &= ~neighbor.invDirection;
        }
        neighbor.feature.UpdateDrawSums();
      }
      this.UpdateDrawSums();
    }

    public void OnAdded(GameLocation loc, Vector2 tilePos) => this.updateNeighbors(loc, tilePos);

    public void OnRemoved(GameLocation loc, Vector2 tilePos)
    {
      if (loc == null)
        return;
      List<HoeDirt.Neighbor> neighborList = this.gatherNeighbors(loc, tilePos);
      this.neighborMask = (byte) 0;
      this.wateredNeighborMask = (byte) 0;
      foreach (HoeDirt.Neighbor neighbor in neighborList)
      {
        neighbor.feature.OnNeighborRemoved(neighbor.invDirection);
        if ((int) (NetFieldBase<int, NetInt>) this.state == 1)
          neighbor.feature.wateredNeighborMask &= ~neighbor.invDirection;
        neighbor.feature.UpdateDrawSums();
      }
      this.UpdateDrawSums();
    }

    public virtual void UpdateDrawSums()
    {
      this.drawSum = (byte) ((uint) this.neighborMask & 15U);
      this.sourceRectPosition = HoeDirt.drawGuide[this.drawSum];
      this.wateredRectPosition = HoeDirt.drawGuide[this.wateredNeighborMask];
    }

    public void OnNeighborAdded(byte direction) => this.neighborMask |= direction;

    public void OnNeighborRemoved(byte direction) => this.neighborMask &= ~direction;

    private struct NeighborLoc
    {
      public readonly Vector2 Offset;
      public readonly byte Direction;
      public readonly byte InvDirection;

      public NeighborLoc(Vector2 a, byte b, byte c)
      {
        this.Offset = a;
        this.Direction = b;
        this.InvDirection = c;
      }
    }

    private struct Neighbor
    {
      public readonly HoeDirt feature;
      public readonly byte direction;
      public readonly byte invDirection;

      public Neighbor(HoeDirt a, byte b, byte c)
      {
        this.feature = a;
        this.direction = b;
        this.invDirection = c;
      }
    }
  }
}
