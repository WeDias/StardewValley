// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.FruitTree
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.TerrainFeatures
{
  public class FruitTree : TerrainFeature
  {
    public const float shakeRate = 0.01570796f;
    public const float shakeDecayRate = 0.003067962f;
    public const int minWoodDebrisForFallenTree = 12;
    public const int minWoodDebrisForStump = 5;
    public const int startingHealth = 10;
    public const int leafFallRate = 3;
    public const int DaysUntilMaturity = 28;
    public const int maxFruitsOnTrees = 3;
    public const int seedStage = 0;
    public const int sproutStage = 1;
    public const int saplingStage = 2;
    public const int bushStage = 3;
    public const int treeStage = 4;
    public static Texture2D texture;
    [XmlElement("growthStage")]
    public readonly NetInt growthStage = new NetInt();
    [XmlElement("treeType")]
    public readonly NetInt treeType = new NetInt(-1);
    [XmlElement("indexOfFruit")]
    public readonly NetInt indexOfFruit = new NetInt();
    [XmlElement("daysUntilMature")]
    public readonly NetInt daysUntilMature = new NetInt();
    [XmlElement("fruitsOnTree")]
    public readonly NetInt fruitsOnTree = new NetInt();
    [XmlElement("struckByLightningCountdown")]
    public readonly NetInt struckByLightningCountdown = new NetInt();
    [XmlElement("health")]
    public readonly NetFloat health = new NetFloat();
    [XmlElement("flipped")]
    public readonly NetBool flipped = new NetBool();
    [XmlElement("stump")]
    public readonly NetBool stump = new NetBool();
    [XmlElement("greenHouseTree")]
    public readonly NetBool greenHouseTree = new NetBool();
    [XmlElement("greenHouseTileTree")]
    public readonly NetBool greenHouseTileTree = new NetBool();
    [XmlElement("shakeLeft")]
    public readonly NetBool shakeLeft = new NetBool();
    [XmlElement("falling")]
    private readonly NetBool falling = new NetBool();
    private bool destroy;
    private float shakeRotation;
    private float maxShake;
    private float alpha = 1f;
    private List<Leaf> leaves = new List<Leaf>();
    [XmlElement("lastPlayerToHit")]
    private readonly NetLong lastPlayerToHit = new NetLong();
    [XmlElement("fruitSeason")]
    public readonly NetString fruitSeason = new NetString();
    private float shakeTimer;

    [XmlIgnore]
    public bool GreenHouseTree
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.greenHouseTree;
      set => this.greenHouseTree.Value = value;
    }

    [XmlIgnore]
    public bool GreenHouseTileTree
    {
      get => (bool) (NetFieldBase<bool, NetBool>) this.greenHouseTileTree;
      set => this.greenHouseTileTree.Value = value;
    }

    public FruitTree()
      : base(true)
    {
      this.loadSprite();
      this.NetFields.AddFields((INetSerializable) this.growthStage, (INetSerializable) this.treeType, (INetSerializable) this.indexOfFruit, (INetSerializable) this.daysUntilMature, (INetSerializable) this.fruitsOnTree, (INetSerializable) this.struckByLightningCountdown, (INetSerializable) this.health, (INetSerializable) this.flipped, (INetSerializable) this.stump, (INetSerializable) this.greenHouseTree, (INetSerializable) this.greenHouseTileTree, (INetSerializable) this.shakeLeft, (INetSerializable) this.falling, (INetSerializable) this.lastPlayerToHit, (INetSerializable) this.fruitSeason);
    }

    public FruitTree(int saplingIndex, int growthStage)
      : this()
    {
      this.growthStage.Value = growthStage;
      this.flipped.Value = Game1.random.NextDouble() < 0.5;
      this.health.Value = 10f;
      this.loadData(saplingIndex);
      this.daysUntilMature.Value = 28;
    }

    public FruitTree(int saplingIndex)
      : this()
    {
      this.flipped.Value = Game1.random.NextDouble() < 0.5;
      this.health.Value = 10f;
      this.loadData(saplingIndex);
      this.daysUntilMature.Value = 28;
    }

    private void loadData(int saplingIndex)
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\fruitTrees");
      if (!dictionary.ContainsKey(saplingIndex))
        return;
      string[] strArray = dictionary[saplingIndex].Split('/');
      this.treeType.Value = Convert.ToInt32(strArray[0]);
      this.fruitSeason.Value = strArray[1];
      this.indexOfFruit.Value = Convert.ToInt32(strArray[2]);
    }

    public override void loadSprite()
    {
      try
      {
        if (FruitTree.texture != null)
          return;
        FruitTree.texture = Game1.content.Load<Texture2D>("TileSheets\\fruitTrees");
      }
      catch (Exception ex)
      {
      }
    }

    public override bool isActionable() => true;

    public override Rectangle getBoundingBox(Vector2 tileLocation) => new Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 64, 64);

    public override Rectangle getRenderBounds(Vector2 tileLocation) => (bool) (NetFieldBase<bool, NetBool>) this.stump || (int) (NetFieldBase<int, NetInt>) this.growthStage < 4 ? new Rectangle((int) ((double) tileLocation.X - 0.0) * 64, (int) ((double) tileLocation.Y - 1.0) * 64, 64, 128) : new Rectangle((int) ((double) tileLocation.X - 1.0) * 64, (int) ((double) tileLocation.Y - 5.0) * 64, 192, 448);

    public override bool performUseAction(Vector2 tileLocation, GameLocation location)
    {
      if ((double) this.maxShake == 0.0 && !(bool) (NetFieldBase<bool, NetBool>) this.stump && (int) (NetFieldBase<int, NetInt>) this.growthStage >= 3 && (!Game1.GetSeasonForLocation(location).Equals("winter") || location.SeedsIgnoreSeasonsHere()))
        location.playSound("leafrustle");
      this.shake(tileLocation, false, location);
      return true;
    }

    public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
    {
      if (this.destroy)
        return true;
      this.alpha = Math.Min(1f, this.alpha + 0.05f);
      if ((double) this.shakeTimer > 0.0)
        this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 4 && !(bool) (NetFieldBase<bool, NetBool>) this.falling && !(bool) (NetFieldBase<bool, NetBool>) this.stump && Game1.player.GetBoundingBox().Intersects(new Rectangle(64 * ((int) tileLocation.X - 1), 64 * ((int) tileLocation.Y - 4), 192, 224)))
        this.alpha = Math.Max(0.4f, this.alpha - 0.09f);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.falling)
      {
        if ((double) Math.Abs(this.shakeRotation) > Math.PI / 2.0 && this.leaves.Count <= 0 && (double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
          return true;
        if ((double) this.maxShake > 0.0)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft)
          {
            this.shakeRotation -= (int) (NetFieldBase<int, NetInt>) this.growthStage >= 4 ? (float) Math.PI / 600f : (float) Math.PI / 200f;
            if ((double) this.shakeRotation <= -(double) this.maxShake)
              this.shakeLeft.Value = false;
          }
          else
          {
            this.shakeRotation += (int) (NetFieldBase<int, NetInt>) this.growthStage >= 4 ? (float) Math.PI / 600f : (float) Math.PI / 200f;
            if ((double) this.shakeRotation >= (double) this.maxShake)
              this.shakeLeft.Value = true;
          }
        }
        if ((double) this.maxShake > 0.0)
          this.maxShake = Math.Max(0.0f, this.maxShake - ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 4 ? 0.001022654f : 0.003067962f));
        if ((int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 && Game1.random.NextDouble() < 0.01)
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(tileLocation.X * 64f + (float) Game1.random.Next(-64, 96), (float) ((double) tileLocation.Y * 64.0 - 192.0) + (float) Game1.random.Next(-64, 128)), false, 1f / 500f, Color.Gray)
          {
            alpha = 0.75f,
            motion = new Vector2(0.0f, -0.5f),
            interval = 99999f,
            layerDepth = 1f,
            scale = 2f,
            scaleChange = 0.01f
          });
      }
      else
      {
        this.shakeRotation += (bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? (float) -((double) this.maxShake * (double) this.maxShake) : this.maxShake * this.maxShake;
        this.maxShake += 0.001533981f;
        if (Game1.random.NextDouble() < 0.01 && !Game1.GetSeasonForLocation(location).Equals("winter"))
          location.localSound("leafrustle");
        if ((double) Math.Abs(this.shakeRotation) > Math.PI / 2.0)
        {
          this.falling.Value = false;
          this.maxShake = 0.0f;
          location.localSound("treethud");
          int num = Game1.random.Next(90, 120);
          for (int index = 0; index < num; ++index)
            this.leaves.Add(new Leaf(new Vector2((float) (Game1.random.Next((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.X * 64.0 + 192.0)) + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -320 : 256)), (float) ((double) tileLocation.Y * 64.0 - 64.0)), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(10, 40) / 10f));
          Game1.createRadialDebris(location, 12, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, (int) ((Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * 12.0), true);
          Game1.createRadialDebris(location, 12, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, (int) ((Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * 12.0), false);
          if (Game1.IsMultiplayer)
          {
            Game1.recentMultiplayerRandom = new Random((int) tileLocation.X * 1000 + (int) tileLocation.Y);
            Random multiplayerRandom = Game1.recentMultiplayerRandom;
          }
          else
          {
            Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
          }
          if (Game1.IsMultiplayer)
            Game1.createMultipleObjectDebris(92, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, 10, (long) this.lastPlayerToHit, location);
          else
            Game1.createMultipleObjectDebris(92, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, 10, location);
          if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
            this.health.Value = -100f;
        }
      }
      for (int index = this.leaves.Count - 1; index >= 0; --index)
      {
        this.leaves.ElementAt<Leaf>(index).position.Y -= this.leaves.ElementAt<Leaf>(index).yVelocity - 3f;
        this.leaves.ElementAt<Leaf>(index).yVelocity = Math.Max(0.0f, this.leaves.ElementAt<Leaf>(index).yVelocity - 0.01f);
        this.leaves.ElementAt<Leaf>(index).rotation += this.leaves.ElementAt<Leaf>(index).rotationRate;
        if ((double) this.leaves.ElementAt<Leaf>(index).position.Y >= (double) tileLocation.Y * 64.0 + 64.0)
          this.leaves.RemoveAt(index);
      }
      return false;
    }

    public virtual void shake(
      Vector2 tileLocation,
      bool doEvenIfStillShaking,
      GameLocation location)
    {
      if ((double) this.maxShake == 0.0 | doEvenIfStillShaking && (int) (NetFieldBase<int, NetInt>) this.growthStage >= 3 && !(bool) (NetFieldBase<bool, NetBool>) this.stump)
      {
        this.shakeLeft.Value = (double) Game1.player.getStandingX() > ((double) tileLocation.X + 0.5) * 64.0 || (double) Game1.player.getTileLocation().X == (double) tileLocation.X && Game1.random.NextDouble() < 0.5;
        this.maxShake = (int) (NetFieldBase<int, NetInt>) this.growthStage >= 4 ? (float) Math.PI / 128f : (float) Math.PI / 64f;
        if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 4)
        {
          if (Game1.random.NextDouble() < 0.66 && Game1.GetSeasonForLocation(location) != "winter")
          {
            int num = Game1.random.Next(1, 6);
            for (int index = 0; index < num; ++index)
              this.leaves.Add(new Leaf(new Vector2((float) Game1.random.Next((int) ((double) tileLocation.X * 64.0 - 64.0), (int) ((double) tileLocation.X * 64.0 + 128.0)), (float) Game1.random.Next((int) ((double) tileLocation.Y * 64.0 - 256.0), (int) ((double) tileLocation.Y * 64.0 - 192.0))), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(5) / 10f));
          }
          int num1 = 0;
          if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= -112)
            num1 = 1;
          if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= -224)
            num1 = 2;
          if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= -336)
            num1 = 4;
          if ((int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0)
            num1 = 0;
          if (!location.terrainFeatures.ContainsKey(tileLocation) || !location.terrainFeatures[tileLocation].Equals((object) this))
            return;
          for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.fruitsOnTree; ++index)
          {
            Vector2 vector2 = new Vector2(0.0f, 0.0f);
            switch (index)
            {
              case 0:
                vector2.X = -64f;
                break;
              case 1:
                vector2.X = 64f;
                vector2.Y = -32f;
                break;
              case 2:
                vector2.Y = 32f;
                break;
            }
            Debris debris = new Debris((int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 ? 382 : (int) (NetFieldBase<int, NetInt>) this.indexOfFruit, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 3.0) * 64.0 + 32.0)) + vector2, new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()))
            {
              itemQuality = num1
            };
            debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
            debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
            location.debris.Add(debris);
          }
          this.fruitsOnTree.Value = 0;
        }
        else
        {
          if (Game1.random.NextDouble() >= 0.66 || !(Game1.GetSeasonForLocation(location) != "winter"))
            return;
          int num = Game1.random.Next(1, 3);
          for (int index = 0; index < num; ++index)
            this.leaves.Add(new Leaf(new Vector2((float) Game1.random.Next((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.X * 64.0 + 48.0)), (float) ((double) tileLocation.Y * 64.0 - 96.0)), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(30) / 10f));
        }
      }
      else
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.stump)
          return;
        this.shakeTimer = 100f;
      }
    }

    public override bool isPassable(Character c = null) => (double) (float) (NetFieldBase<float, NetFloat>) this.health <= -99.0;

    public static bool IsGrowthBlocked(Vector2 tileLocation, GameLocation environment)
    {
      foreach (Vector2 surroundingTileLocations in Utility.getSurroundingTileLocationsArray(tileLocation))
      {
        bool flag = environment.terrainFeatures.ContainsKey(surroundingTileLocations) && environment.terrainFeatures[surroundingTileLocations] is HoeDirt && (environment.terrainFeatures[surroundingTileLocations] as HoeDirt).crop == null;
        if (environment.isTileOccupied(surroundingTileLocations, ignoreAllCharacters: true) && !flag)
        {
          StardewValley.Object objectAtTile = environment.getObjectAtTile((int) surroundingTileLocations.X, (int) surroundingTileLocations.Y);
          if (objectAtTile == null || !Utility.IsNormalObjectAtParentSheetIndex((Item) objectAtTile, 590))
            return true;
        }
      }
      return false;
    }

    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= -99.0)
        this.destroy = true;
      if ((int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0)
      {
        --this.struckByLightningCountdown.Value;
        if ((int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown <= 0)
          this.fruitsOnTree.Value = 0;
      }
      bool flag = FruitTree.IsGrowthBlocked(tileLocation, environment);
      if (!flag || (int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= 0)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature > 28)
          this.daysUntilMature.Value = 28;
        --this.daysUntilMature.Value;
        if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= 0)
          this.growthStage.Value = 4;
        else if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= 7)
          this.growthStage.Value = 3;
        else if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= 14)
          this.growthStage.Value = 2;
        else if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= 21)
          this.growthStage.Value = 1;
        else
          this.growthStage.Value = 0;
      }
      else if (flag && this.growthStage.Value != 4)
        Game1.multiplayer.broadcastGlobalMessage("Strings\\UI:FruitTree_Warning", true, Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.indexOfFruit].Split('/')[4]);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.stump && (int) (NetFieldBase<int, NetInt>) this.growthStage == 4 && ((int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 && !Game1.IsWinter || this.IsInSeasonHere(environment) || environment.SeedsIgnoreSeasonsHere()))
      {
        this.fruitsOnTree.Value = Math.Min(3, (int) (NetFieldBase<int, NetInt>) this.fruitsOnTree + 1);
        if (environment.IsGreenhouse)
          this.greenHouseTree.Value = true;
      }
      if (!(bool) (NetFieldBase<bool, NetBool>) this.stump)
        return;
      this.fruitsOnTree.Value = 0;
    }

    public virtual bool IsInSeasonHere(GameLocation location)
    {
      if (!(this.fruitSeason.Value == "island"))
        return Game1.GetSeasonForLocation(location).Equals((string) (NetFieldBase<string, NetString>) this.fruitSeason);
      return location.GetLocationContext() == GameLocation.LocationContext.Island || Game1.GetSeasonForLocation(location).Equals("summer");
    }

    public override bool seasonUpdate(bool onLoad)
    {
      if (!this.IsInSeasonHere(this.currentLocation) && !onLoad && !(bool) (NetFieldBase<bool, NetBool>) this.greenHouseTree)
        this.fruitsOnTree.Value = 0;
      return false;
    }

    public override bool performToolAction(
      Tool t,
      int explosion,
      Vector2 tileLocation,
      GameLocation location)
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= -99.0 || t != null && t is MeleeWeapon)
        return false;
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 4)
      {
        if (t != null && t is Axe)
        {
          location.playSound("axchop");
          location.debris.Add(new Debris(12, Game1.random.Next((int) (NetFieldBase<int, NetInt>) t.upgradeLevel * 2, (int) (NetFieldBase<int, NetInt>) t.upgradeLevel * 4), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), t.getLastFarmerToUse().Position, 0, -1));
          this.lastPlayerToHit.Value = t.getLastFarmerToUse().UniqueMultiplayerID;
          int num = 0;
          if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= -112)
            num = 1;
          if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= -224)
            num = 2;
          if ((int) (NetFieldBase<int, NetInt>) this.daysUntilMature <= -336)
            num = 4;
          if ((int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0)
            num = 0;
          if (location.terrainFeatures.ContainsKey(tileLocation) && location.terrainFeatures[tileLocation].Equals((object) this))
          {
            for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.fruitsOnTree; ++index)
            {
              Vector2 vector2 = new Vector2(0.0f, 0.0f);
              switch (index)
              {
                case 0:
                  vector2.X = -64f;
                  break;
                case 1:
                  vector2.X = 64f;
                  vector2.Y = -32f;
                  break;
                case 2:
                  vector2.Y = 32f;
                  break;
              }
              Debris debris = new Debris((int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 ? 382 : (int) (NetFieldBase<int, NetInt>) this.indexOfFruit, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 3.0) * 64.0 + 32.0)) + vector2, new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()))
              {
                itemQuality = num
              };
              debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
              debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
              location.debris.Add(debris);
            }
            this.fruitsOnTree.Value = 0;
          }
        }
        else if (explosion <= 0)
          return false;
        this.shake(tileLocation, true, location);
        float num1;
        if (explosion > 0)
        {
          num1 = (float) explosion;
        }
        else
        {
          if (t == null)
            return false;
          switch ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel)
          {
            case 0:
              num1 = 1f;
              break;
            case 1:
              num1 = 1.25f;
              break;
            case 2:
              num1 = 1.67f;
              break;
            case 3:
              num1 = 2.5f;
              break;
            case 4:
              num1 = 5f;
              break;
            default:
              num1 = (float) ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel + 1);
              break;
          }
        }
        this.health.Value -= num1;
        if (t is Axe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double) num1 / 5.0)
        {
          Debris debris = new Debris(388, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()));
          debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
          debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
          location.debris.Add(debris);
        }
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
        {
          if (!(bool) (NetFieldBase<bool, NetBool>) this.stump)
          {
            location.playSound("treecrack");
            this.stump.Value = true;
            this.health.Value = 5f;
            this.falling.Value = true;
            if (t == null || t.getLastFarmerToUse() == null)
              this.shakeLeft.Value = true;
            else
              this.shakeLeft.Value = (double) t.getLastFarmerToUse().getStandingX() > ((double) tileLocation.X + 0.5) * 64.0;
          }
          else
          {
            this.health.Value = -100f;
            Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(30, 40), false);
            int index = 92;
            if (Game1.IsMultiplayer)
            {
              Game1.recentMultiplayerRandom = new Random((int) tileLocation.X * 2000 + (int) tileLocation.Y);
              Random multiplayerRandom = Game1.recentMultiplayerRandom;
            }
            else
            {
              Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
            }
            if (t == null || t.getLastFarmerToUse() == null)
              Game1.createMultipleObjectDebris(92, (int) tileLocation.X, (int) tileLocation.Y, 2, location);
            else if (Game1.IsMultiplayer)
            {
              Game1.createMultipleObjectDebris(index, (int) tileLocation.X, (int) tileLocation.Y, 1, (long) this.lastPlayerToHit, location);
              Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 5 : 4, true);
            }
            else
            {
              Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, (int) ((Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * 5.0), true);
              Game1.createMultipleObjectDebris(index, (int) tileLocation.X, (int) tileLocation.Y, 1, location);
            }
          }
        }
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 3)
      {
        if (t != null && t.BaseName.Contains("Ax"))
        {
          location.playSound("axchop");
          location.playSound("leafrustle");
          location.debris.Add(new Debris(12, Game1.random.Next((int) (NetFieldBase<int, NetInt>) t.upgradeLevel * 2, (int) (NetFieldBase<int, NetInt>) t.upgradeLevel * 4), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), new Vector2((float) t.getLastFarmerToUse().GetBoundingBox().Center.X, (float) t.getLastFarmerToUse().GetBoundingBox().Center.Y), 0, -1));
        }
        else if (explosion <= 0)
          return false;
        this.shake(tileLocation, true, location);
        float num = 1f;
        Random random = !Game1.IsMultiplayer ? new Random((int) ((double) Game1.uniqueIDForThisGame + (double) tileLocation.X * 7.0 + (double) tileLocation.Y * 11.0 + (double) Game1.stats.DaysPlayed + (double) (float) (NetFieldBase<float, NetFloat>) this.health)) : Game1.recentMultiplayerRandom;
        if (explosion > 0)
        {
          num = (float) explosion;
        }
        else
        {
          switch ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel)
          {
            case 0:
              num = 2f;
              break;
            case 1:
              num = 2.5f;
              break;
            case 2:
              num = 3.34f;
              break;
            case 3:
              num = 5f;
              break;
            case 4:
              num = 10f;
              break;
          }
        }
        int numberOfChunks = 0;
        while (t != null && random.NextDouble() < (double) num * 0.08 + (double) t.getLastFarmerToUse().ForagingLevel / 200.0)
          ++numberOfChunks;
        this.health.Value -= num;
        if (numberOfChunks > 0)
          Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, numberOfChunks, location);
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
        {
          Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(20, 30), false);
          return true;
        }
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 1)
      {
        if (explosion > 0)
          return true;
        if (t != null && t.BaseName.Contains("Axe"))
        {
          location.playSound("axchop");
          Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(10, 20), false);
        }
        if (t is Axe || t is Pickaxe || t is Hoe || t is MeleeWeapon)
        {
          Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(10, 20), false);
          if (t.BaseName.Contains("Axe") && Game1.recentMultiplayerRandom.NextDouble() < (double) t.getLastFarmerToUse().ForagingLevel / 10.0)
            Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, 1, location);
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));
          return true;
        }
      }
      else
      {
        if (explosion > 0)
          return true;
        if (t.BaseName.Contains("Axe") || t.BaseName.Contains("Pick") || t.BaseName.Contains("Hoe"))
        {
          location.playSound("woodyHit");
          location.playSound("axchop");
          Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));
          return true;
        }
      }
      return false;
    }

    public override void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 positionOnScreen,
      Vector2 tileLocation,
      float scale,
      float layerDepth)
    {
      layerDepth += positionOnScreen.X / 100000f;
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage < 4)
      {
        Rectangle rectangle = Rectangle.Empty;
        switch ((int) (NetFieldBase<int, NetInt>) this.growthStage)
        {
          case 0:
            rectangle = new Rectangle(128, 512, 64, 64);
            break;
          case 1:
            rectangle = new Rectangle(0, 512, 64, 64);
            break;
          case 2:
            rectangle = new Rectangle(64, 512, 64, 64);
            break;
          default:
            rectangle = new Rectangle(0, 384, 64, 128);
            break;
        }
        spriteBatch.Draw(FruitTree.texture, positionOnScreen - new Vector2(0.0f, (float) rectangle.Height * scale), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, scale, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + (double) rectangle.Height * (double) scale) / 20000.0));
      }
      else
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.falling)
          spriteBatch.Draw(FruitTree.texture, positionOnScreen + new Vector2(0.0f, -64f * scale), new Rectangle?(new Rectangle(128, 384, 64, 128)), Color.White, 0.0f, Vector2.Zero, scale, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale - 1.0) / 20000.0));
        if ((bool) (NetFieldBase<bool, NetBool>) this.stump && !(bool) (NetFieldBase<bool, NetBool>) this.falling)
          return;
        spriteBatch.Draw(FruitTree.texture, positionOnScreen + new Vector2(-64f * scale, -320f * scale), new Rectangle?(new Rectangle(0, 0, 192, 384)), Color.White, this.shakeRotation, Vector2.Zero, scale, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale) / 20000.0));
      }
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
    {
      string seasonForLocation = Game1.GetSeasonForLocation(this.currentLocation);
      if ((bool) (NetFieldBase<bool, NetBool>) this.greenHouseTileTree)
        spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(new Rectangle(669, 1957, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage < 4)
      {
        Vector2 vector2 = new Vector2((float) Math.Max(-8.0, Math.Min(64.0, Math.Sin((double) tileLocation.X * 200.0 / (2.0 * Math.PI)) * -16.0)), (float) Math.Max(-8.0, Math.Min(64.0, Math.Sin((double) tileLocation.X * 200.0 / (2.0 * Math.PI)) * -16.0))) / 2f;
        Rectangle rectangle = Rectangle.Empty;
        switch ((int) (NetFieldBase<int, NetInt>) this.growthStage)
        {
          case 0:
            rectangle = new Rectangle(0, (int) (NetFieldBase<int, NetInt>) this.treeType * 5 * 16, 48, 80);
            break;
          case 1:
            rectangle = new Rectangle(48, (int) (NetFieldBase<int, NetInt>) this.treeType * 5 * 16, 48, 80);
            break;
          case 2:
            rectangle = new Rectangle(96, (int) (NetFieldBase<int, NetInt>) this.treeType * 5 * 16, 48, 80);
            break;
          default:
            rectangle = new Rectangle(144, (int) (NetFieldBase<int, NetInt>) this.treeType * 5 * 16, 48, 80);
            break;
        }
        spriteBatch.Draw(FruitTree.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0) + vector2.X, (float) ((double) tileLocation.Y * 64.0 - (double) rectangle.Height + 128.0) + vector2.Y)), new Rectangle?(rectangle), Color.White, this.shakeRotation, new Vector2(24f, 80f), 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((double) this.getBoundingBox(tileLocation).Bottom / 10000.0 - (double) tileLocation.X / 1000000.0));
      }
      else
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.stump || (bool) (NetFieldBase<bool, NetBool>) this.falling)
        {
          if (!(bool) (NetFieldBase<bool, NetBool>) this.falling)
            spriteBatch.Draw(FruitTree.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 64.0))), new Rectangle?(new Rectangle((12 + ((bool) (NetFieldBase<bool, NetBool>) this.greenHouseTree ? 1 : Utility.getSeasonNumber(seasonForLocation)) * 3) * 16, (int) (NetFieldBase<int, NetInt>) this.treeType * 5 * 16 + 64, 48, 16)), (int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 ? Color.Gray * this.alpha : Color.White * this.alpha, 0.0f, new Vector2(24f, 16f), 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-07f);
          spriteBatch.Draw(FruitTree.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 64.0))), new Rectangle?(new Rectangle((12 + ((bool) (NetFieldBase<bool, NetBool>) this.greenHouseTree ? 1 : Utility.getSeasonNumber(seasonForLocation)) * 3) * 16, (int) (NetFieldBase<int, NetInt>) this.treeType * 5 * 16, 48, 64)), (int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 ? Color.Gray * this.alpha : Color.White * this.alpha, this.shakeRotation, new Vector2(24f, 80f), 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((double) this.getBoundingBox(tileLocation).Bottom / 10000.0 + 1.0 / 1000.0 - (double) tileLocation.X / 1000000.0));
        }
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.health >= 1.0 || !(bool) (NetFieldBase<bool, NetBool>) this.falling && (double) (float) (NetFieldBase<float, NetFloat>) this.health > -99.0)
          spriteBatch.Draw(FruitTree.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0 + ((double) this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 2.0 : 0.0)), (float) ((double) tileLocation.Y * 64.0 + 64.0))), new Rectangle?(new Rectangle(384, (int) (NetFieldBase<int, NetInt>) this.treeType * 5 * 16 + 48, 48, 32)), (int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 ? Color.Gray * this.alpha : Color.White * this.alpha, 0.0f, new Vector2(24f, 32f), 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, !(bool) (NetFieldBase<bool, NetBool>) this.stump || (bool) (NetFieldBase<bool, NetBool>) this.falling ? (float) ((double) this.getBoundingBox(tileLocation).Bottom / 10000.0 - 1.0 / 1000.0 - (double) tileLocation.X / 1000000.0) : (float) this.getBoundingBox(tileLocation).Bottom / 10000f);
        for (int index = 0; index < (int) (NetFieldBase<int, NetInt>) this.fruitsOnTree; ++index)
        {
          switch (index)
          {
            case 0:
              spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 - 64.0 + (double) tileLocation.X * 200.0 % 64.0 / 2.0), (float) ((double) tileLocation.Y * 64.0 - 192.0 - (double) tileLocation.X % 64.0 / 3.0))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 ? 382 : (int) (NetFieldBase<int, NetInt>) this.indexOfFruit, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.getBoundingBox(tileLocation).Bottom / 10000.0 + 1.0 / 500.0 - (double) tileLocation.X / 1000000.0));
              break;
            case 1:
              spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 - 256.0 + (double) tileLocation.X * 232.0 % 64.0 / 3.0))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 ? 382 : (int) (NetFieldBase<int, NetInt>) this.indexOfFruit, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.getBoundingBox(tileLocation).Bottom / 10000.0 + 1.0 / 500.0 - (double) tileLocation.X / 1000000.0));
              break;
            case 2:
              spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + (double) tileLocation.X * 200.0 % 64.0 / 3.0), (float) ((double) tileLocation.Y * 64.0 - 160.0 + (double) tileLocation.X * 200.0 % 64.0 / 3.0))), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int) (NetFieldBase<int, NetInt>) this.struckByLightningCountdown > 0 ? 382 : (int) (NetFieldBase<int, NetInt>) this.indexOfFruit, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, (float) ((double) this.getBoundingBox(tileLocation).Bottom / 10000.0 + 1.0 / 500.0 - (double) tileLocation.X / 1000000.0));
              break;
          }
        }
      }
      foreach (Leaf leaf in this.leaves)
        spriteBatch.Draw(FruitTree.texture, Game1.GlobalToLocal(Game1.viewport, leaf.position), new Rectangle?(new Rectangle((24 + Utility.getSeasonNumber(seasonForLocation)) * 16, (int) (NetFieldBase<int, NetInt>) this.treeType * 5 * 16, 8, 8)), Color.White, leaf.rotation, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.getBoundingBox(tileLocation).Bottom / 10000.0 + 0.00999999977648258));
    }
  }
}
