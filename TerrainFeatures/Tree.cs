// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Tree
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace StardewValley.TerrainFeatures
{
  public class Tree : TerrainFeature
  {
    public const float chanceForDailySeed = 0.05f;
    public const float shakeRate = 0.01570796f;
    public const float shakeDecayRate = 0.003067962f;
    public const int minWoodDebrisForFallenTree = 12;
    public const int minWoodDebrisForStump = 5;
    public const int startingHealth = 10;
    public const int leafFallRate = 3;
    public const int bushyTree = 1;
    public const int leafyTree = 2;
    public const int pineTree = 3;
    public const int winterTree1 = 4;
    public const int winterTree2 = 5;
    public const int palmTree = 6;
    public const int mushroomTree = 7;
    public const int mahoganyTree = 8;
    public const int palmTree2 = 9;
    public const int seedStage = 0;
    public const int sproutStage = 1;
    public const int saplingStage = 2;
    public const int bushStage = 3;
    public const int treeStage = 5;
    public Lazy<Texture2D> texture;
    private string season;
    [XmlElement("growthStage")]
    public readonly NetInt growthStage = new NetInt();
    [XmlElement("treeType")]
    public readonly NetInt treeType = new NetInt();
    [XmlElement("health")]
    public readonly NetFloat health = new NetFloat();
    [XmlElement("flipped")]
    public readonly NetBool flipped = new NetBool();
    [XmlElement("stump")]
    public readonly NetBool stump = new NetBool();
    [XmlElement("tapped")]
    public readonly NetBool tapped = new NetBool();
    [XmlElement("hasSeed")]
    public readonly NetBool hasSeed = new NetBool();
    [XmlElement("fertilized")]
    public readonly NetBool fertilized = new NetBool();
    [XmlElement("shakeLeft")]
    public readonly NetBool shakeLeft = new NetBool().Interpolated(false, false);
    [XmlElement("falling")]
    private readonly NetBool falling = new NetBool();
    [XmlElement("destroy")]
    private readonly NetBool destroy = new NetBool();
    private float shakeRotation;
    private float maxShake;
    private float alpha = 1f;
    private List<Leaf> leaves = new List<Leaf>();
    [XmlElement("lastPlayerToHit")]
    private readonly NetLong lastPlayerToHit = new NetLong();
    private float shakeTimer;
    public Microsoft.Xna.Framework.Rectangle treeTopSourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 96);
    public static Microsoft.Xna.Framework.Rectangle stumpSourceRect = new Microsoft.Xna.Framework.Rectangle(32, 96, 16, 32);
    public static Microsoft.Xna.Framework.Rectangle shadowSourceRect = new Microsoft.Xna.Framework.Rectangle(663, 1011, 41, 30);

    public Tree()
      : base(true)
    {
      this.resetTexture();
      this.NetFields.AddFields((INetSerializable) this.growthStage, (INetSerializable) this.treeType, (INetSerializable) this.health, (INetSerializable) this.flipped, (INetSerializable) this.stump, (INetSerializable) this.tapped, (INetSerializable) this.hasSeed, (INetSerializable) this.fertilized, (INetSerializable) this.shakeLeft, (INetSerializable) this.falling, (INetSerializable) this.destroy, (INetSerializable) this.lastPlayerToHit);
      this.treeType.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((a, b, c) => this.resetTexture());
    }

    public Tree(int which, int growthStage)
      : this()
    {
      this.growthStage.Value = growthStage;
      this.treeType.Value = which;
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 4)
        this.treeType.Value = 1;
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 5)
        this.treeType.Value = 2;
      this.flipped.Value = Game1.random.NextDouble() < 0.5;
      this.health.Value = 10f;
    }

    public Tree(int which)
      : this()
    {
      this.treeType.Value = which;
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 4)
        this.treeType.Value = 1;
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 5)
        this.treeType.Value = 2;
      this.flipped.Value = Game1.random.NextDouble() < 0.5;
      this.health.Value = 10f;
    }

    protected void resetTexture() => this.texture = new Lazy<Texture2D>(new Func<Texture2D>(this.loadTexture));

    protected Texture2D loadTexture()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 7)
        return Game1.content.Load<Texture2D>("TerrainFeatures\\mushroom_tree");
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 9)
        return Game1.content.Load<Texture2D>("TerrainFeatures\\tree_palm2");
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 6)
        return Game1.content.Load<Texture2D>("TerrainFeatures\\tree_palm");
      string str = Game1.GetSeasonForLocation(this.currentLocation);
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 3 && str.Equals("summer"))
        str = "spring";
      if (Game1.currentLocation != null && (Game1.currentLocation.Name.Equals("Desert") || Game1.currentLocation is MineShaft))
        str = "spring";
      return Game1.content.Load<Texture2D>("TerrainFeatures\\tree" + Math.Max(1, (int) (NetFieldBase<int, NetInt>) this.treeType).ToString() + "_" + str);
    }

    public override Microsoft.Xna.Framework.Rectangle getBoundingBox(Vector2 tileLocation) => new Microsoft.Xna.Framework.Rectangle((int) tileLocation.X * 64, (int) tileLocation.Y * 64, 64, 64);

    public override Microsoft.Xna.Framework.Rectangle getRenderBounds(Vector2 tileLocation) => (bool) (NetFieldBase<bool, NetBool>) this.stump || (int) (NetFieldBase<int, NetInt>) this.growthStage < 5 ? new Microsoft.Xna.Framework.Rectangle((int) ((double) tileLocation.X - 0.0) * 64, (int) ((double) tileLocation.Y - 1.0) * 64, 64, 128) : new Microsoft.Xna.Framework.Rectangle((int) ((double) tileLocation.X - 1.0) * 64, (int) ((double) tileLocation.Y - 5.0) * 64, 192, 448);

    public override bool performUseAction(Vector2 tileLocation, GameLocation location)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.tapped)
      {
        if ((double) this.maxShake == 0.0 && !(bool) (NetFieldBase<bool, NetBool>) this.stump && (int) (NetFieldBase<int, NetInt>) this.growthStage >= 3 && (!Game1.GetSeasonForLocation(this.currentLocation).Equals("winter") || location.Name.Equals("Desert") || (int) (NetFieldBase<int, NetInt>) this.treeType == 3))
          location.localSound("leafrustle");
        this.shake(tileLocation, false, location);
      }
      return Game1.player.ActiveObject == null || !Game1.player.ActiveObject.canBePlacedHere(location, tileLocation);
    }

    private int extraWoodCalculator(Vector2 tileLocation)
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
      int num = 0;
      if (random.NextDouble() < Game1.player.DailyLuck)
        ++num;
      if (random.NextDouble() < (double) Game1.player.ForagingLevel / 12.5)
        ++num;
      if (random.NextDouble() < (double) Game1.player.ForagingLevel / 12.5)
        ++num;
      if (random.NextDouble() < (double) Game1.player.LuckLevel / 25.0)
        ++num;
      return num;
    }

    public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
    {
      if (this.season != Game1.GetSeasonForLocation(this.currentLocation))
      {
        this.resetTexture();
        this.season = Game1.GetSeasonForLocation(this.currentLocation);
      }
      if ((double) this.shakeTimer > 0.0)
        this.shakeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      if ((bool) (NetFieldBase<bool, NetBool>) this.destroy)
        return true;
      this.alpha = Math.Min(1f, this.alpha + 0.05f);
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 5 && !(bool) (NetFieldBase<bool, NetBool>) this.falling && !(bool) (NetFieldBase<bool, NetBool>) this.stump && Game1.player.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(64 * ((int) tileLocation.X - 1), 64 * ((int) tileLocation.Y - 5), 192, 288)))
        this.alpha = Math.Max(0.4f, this.alpha - 0.09f);
      if (!(bool) (NetFieldBase<bool, NetBool>) this.falling)
      {
        if ((double) Math.Abs(this.shakeRotation) > Math.PI / 2.0 && this.leaves.Count <= 0 && (double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
          return true;
        if ((double) this.maxShake > 0.0)
        {
          if ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft)
          {
            this.shakeRotation -= (int) (NetFieldBase<int, NetInt>) this.growthStage >= 5 ? (float) Math.PI / 600f : (float) Math.PI / 200f;
            if ((double) this.shakeRotation <= -(double) this.maxShake)
              this.shakeLeft.Value = false;
          }
          else
          {
            this.shakeRotation += (int) (NetFieldBase<int, NetInt>) this.growthStage >= 5 ? (float) Math.PI / 600f : (float) Math.PI / 200f;
            if ((double) this.shakeRotation >= (double) this.maxShake)
              this.shakeLeft.Value = true;
          }
        }
        if ((double) this.maxShake > 0.0)
          this.maxShake = Math.Max(0.0f, this.maxShake - ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 5 ? 0.001022654f : 0.003067962f));
      }
      else
      {
        this.shakeRotation += (bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? (float) -((double) this.maxShake * (double) this.maxShake) : this.maxShake * this.maxShake;
        this.maxShake += 0.001533981f;
        if (Game1.random.NextDouble() < 0.01 && (int) (NetFieldBase<int, NetInt>) this.treeType != 7)
          location.localSound("leafrustle");
        if ((double) Math.Abs(this.shakeRotation) > Math.PI / 2.0)
        {
          this.falling.Value = false;
          this.maxShake = 0.0f;
          location.localSound("treethud");
          int num = Game1.random.Next(90, 120);
          if (location.Objects.ContainsKey(tileLocation))
            location.Objects.Remove(tileLocation);
          for (int index = 0; index < num; ++index)
            this.leaves.Add(new Leaf(new Vector2((float) (Game1.random.Next((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.X * 64.0 + 192.0)) + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -320 : 256)), (float) ((double) tileLocation.Y * 64.0 - 64.0)), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(10, 40) / 10f));
          if ((int) (NetFieldBase<int, NetInt>) this.treeType != 7)
          {
            if ((int) (NetFieldBase<int, NetInt>) this.treeType != 8)
            {
              Game1.createRadialDebris(location, 12, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, (int) ((Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * (double) (12 + this.extraWoodCalculator(tileLocation))), true);
              Game1.createRadialDebris(location, 12, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, (int) ((Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * (double) (12 + this.extraWoodCalculator(tileLocation))), false);
            }
            Random random;
            if (Game1.IsMultiplayer)
            {
              Game1.recentMultiplayerRandom = new Random((int) tileLocation.X * 1000 + (int) tileLocation.Y);
              random = Game1.recentMultiplayerRandom;
            }
            else
              random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
            if (Game1.IsMultiplayer)
            {
              if ((int) (NetFieldBase<int, NetInt>) this.treeType != 8)
                Game1.createMultipleObjectDebris(92, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, 5, (long) this.lastPlayerToHit, location);
              int number = 0;
              if (Game1.getFarmer((long) this.lastPlayerToHit) != null)
              {
                while (Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(14) && random.NextDouble() < 0.5)
                  ++number;
              }
              if ((int) (NetFieldBase<int, NetInt>) this.treeType == 8)
              {
                number += random.Next(7, 12);
                if (Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(14))
                  number += (int) ((double) number * 0.25 + 0.899999976158142);
              }
              if (number > 0)
                Game1.createMultipleObjectDebris(709, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, number, (long) this.lastPlayerToHit, location);
              if (Game1.getFarmer((long) this.lastPlayerToHit).getEffectiveSkillLevel(2) >= 1 && random.NextDouble() < 0.75)
              {
                if ((int) (NetFieldBase<int, NetInt>) this.treeType < 4)
                  Game1.createMultipleObjectDebris(308 + (int) (NetFieldBase<int, NetInt>) this.treeType, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, random.Next(1, 3), (long) this.lastPlayerToHit, location);
                else if ((int) (NetFieldBase<int, NetInt>) this.treeType == 8 && random.NextDouble() < 0.75)
                  Game1.createMultipleObjectDebris(292, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, random.Next(1, 3), (long) this.lastPlayerToHit, location);
              }
            }
            else
            {
              if ((int) (NetFieldBase<int, NetInt>) this.treeType != 8)
                Game1.createMultipleObjectDebris(92, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, 5, location);
              int number = 0;
              if (Game1.getFarmer((long) this.lastPlayerToHit) != null)
              {
                while (Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(14) && random.NextDouble() < 0.5)
                  ++number;
              }
              if ((int) (NetFieldBase<int, NetInt>) this.treeType == 8)
              {
                number += random.Next(7, 12);
                if (Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(14))
                  number += (int) ((double) number * 0.25 + 0.899999976158142);
              }
              if (number > 0)
                Game1.createMultipleObjectDebris(709, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, number, location);
              if ((long) this.lastPlayerToHit != 0L && Game1.getFarmer((long) this.lastPlayerToHit).getEffectiveSkillLevel(2) >= 1 && random.NextDouble() < 0.75)
              {
                if ((int) (NetFieldBase<int, NetInt>) this.treeType < 4)
                  Game1.createMultipleObjectDebris(308 + (int) (NetFieldBase<int, NetInt>) this.treeType, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, random.Next(1, 3), location);
                else if ((int) (NetFieldBase<int, NetInt>) this.treeType == 8 && random.NextDouble() < 0.75)
                  Game1.createMultipleObjectDebris(292, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, random.Next(1, 3), location);
              }
            }
          }
          else
          {
            Game1.createMultipleObjectDebris(420, (int) tileLocation.X + ((bool) (NetFieldBase<bool, NetBool>) this.shakeLeft ? -4 : 4), (int) tileLocation.Y, 5, location);
            if (Game1.random.NextDouble() < 0.01)
              location.debris.Add(new Debris((Item) new Hat(42), tileLocation * 64f + new Vector2(32f, 32f)));
          }
          if ((double) (float) (NetFieldBase<float, NetFloat>) this.health == -100.0)
            return true;
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

    private void shake(Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location)
    {
      if ((double) this.maxShake == 0.0 | doEvenIfStillShaking && (int) (NetFieldBase<int, NetInt>) this.growthStage >= 3 && !(bool) (NetFieldBase<bool, NetBool>) this.stump)
      {
        this.shakeLeft.Value = (double) Game1.player.getStandingX() > ((double) tileLocation.X + 0.5) * 64.0 || (double) Game1.player.getTileLocation().X == (double) tileLocation.X && Game1.random.NextDouble() < 0.5;
        this.maxShake = (int) (NetFieldBase<int, NetInt>) this.growthStage >= 5 ? (float) Math.PI / 128f : (float) Math.PI / 64f;
        if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 5)
        {
          if (Game1.random.NextDouble() < 0.66)
          {
            int num = Game1.random.Next(1, 6);
            for (int index = 0; index < num; ++index)
              this.leaves.Add(new Leaf(new Vector2((float) Game1.random.Next((int) ((double) tileLocation.X * 64.0 - 64.0), (int) ((double) tileLocation.X * 64.0 + 128.0)), (float) Game1.random.Next((int) ((double) tileLocation.Y * 64.0 - 256.0), (int) ((double) tileLocation.Y * 64.0 - 192.0))), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(5) / 10f));
          }
          if (Game1.random.NextDouble() < 0.01 && (Game1.GetSeasonForLocation(this.currentLocation).Equals("spring") || Game1.GetSeasonForLocation(this.currentLocation).Equals("summer") || this.currentLocation.GetLocationContext() == GameLocation.LocationContext.Island))
          {
            while (Game1.random.NextDouble() < 0.8)
              location.addCritter((Critter) new Butterfly(new Vector2(tileLocation.X + (float) Game1.random.Next(1, 3), tileLocation.Y - 2f + (float) Game1.random.Next(-1, 2)), this.currentLocation.GetLocationContext() == GameLocation.LocationContext.Island));
          }
          if (!(bool) (NetFieldBase<bool, NetBool>) this.hasSeed || !Game1.IsMultiplayer && Game1.player.ForagingLevel < 1)
            return;
          int objectIndex = -1;
          switch ((int) (NetFieldBase<int, NetInt>) this.treeType)
          {
            case 1:
              objectIndex = 309;
              break;
            case 2:
              objectIndex = 310;
              break;
            case 3:
              objectIndex = 311;
              break;
            case 6:
            case 9:
              objectIndex = 88;
              break;
            case 8:
              objectIndex = 292;
              break;
          }
          if (Game1.GetSeasonForLocation(this.currentLocation).Equals("fall") && (int) (NetFieldBase<int, NetInt>) this.treeType == 2 && Game1.dayOfMonth >= 14)
            objectIndex = 408;
          if (objectIndex != -1)
            Game1.createObjectDebris(objectIndex, (int) tileLocation.X, (int) tileLocation.Y - 3, ((int) tileLocation.Y + 1) * 64, 0, 1f, location);
          if (objectIndex == 88 && new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + (int) tileLocation.X * 13 + (int) tileLocation.Y * 54).NextDouble() < 0.1 && location != null && location is IslandLocation)
            Game1.createObjectDebris(791, (int) tileLocation.X, (int) tileLocation.Y - 3, ((int) tileLocation.Y + 1) * 64, 0, 1f, location);
          if (Game1.random.NextDouble() <= 0.5 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            Game1.createObjectDebris(890, (int) tileLocation.X, (int) tileLocation.Y - 3, ((int) tileLocation.Y + 1) * 64, 0, 1f, location);
          this.hasSeed.Value = false;
        }
        else
        {
          if (Game1.random.NextDouble() >= 0.66)
            return;
          int num = Game1.random.Next(1, 3);
          for (int index = 0; index < num; ++index)
            this.leaves.Add(new Leaf(new Vector2((float) Game1.random.Next((int) ((double) tileLocation.X * 64.0), (int) ((double) tileLocation.X * 64.0 + 48.0)), (float) ((double) tileLocation.Y * 64.0 - 32.0)), (float) Game1.random.Next(-10, 10) / 100f, Game1.random.Next(4), (float) Game1.random.Next(30) / 10f));
        }
      }
      else
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.stump)
          return;
        this.shakeTimer = 100f;
      }
    }

    public override bool isPassable(Character c = null) => (double) (float) (NetFieldBase<float, NetFloat>) this.health <= -99.0 || (int) (NetFieldBase<int, NetInt>) this.growthStage == 0;

    public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
    {
      Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int) (((double) tileLocation.X - 1.0) * 64.0), (int) (((double) tileLocation.Y - 1.0) * 64.0), 192, 192);
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= -100.0)
        this.destroy.Value = true;
      if (this.tapped.Value)
      {
        StardewValley.Object objectAtTile = environment.getObjectAtTile((int) tileLocation.X, (int) tileLocation.Y);
        if (objectAtTile == null || !(bool) (NetFieldBase<bool, NetBool>) objectAtTile.bigCraftable || (int) (NetFieldBase<int, NetInt>) objectAtTile.parentSheetIndex != 105)
          this.tapped.Value = false;
      }
      if (!Game1.GetSeasonForLocation(this.currentLocation).Equals("winter") || (int) (NetFieldBase<int, NetInt>) this.treeType == 6 || (int) (NetFieldBase<int, NetInt>) this.treeType == 9 || environment.CanPlantTreesHere(-1, (int) tileLocation.X, (int) tileLocation.Y) || this.fertilized.Value)
      {
        switch (environment.doesTileHaveProperty((int) tileLocation.X, (int) tileLocation.Y, "NoSpawn", "Back"))
        {
          case "All":
            return;
          case nameof (Tree):
            return;
          case "True":
            return;
          default:
            if ((int) (NetFieldBase<int, NetInt>) this.growthStage == 4)
            {
              foreach (KeyValuePair<Vector2, TerrainFeature> pair in environment.terrainFeatures.Pairs)
              {
                if (pair.Value is Tree && !pair.Value.Equals((object) this) && (int) (NetFieldBase<int, NetInt>) ((Tree) pair.Value).growthStage >= 5 && pair.Value.getBoundingBox(pair.Key).Intersects(rectangle))
                  return;
              }
            }
            else if ((int) (NetFieldBase<int, NetInt>) this.growthStage == 0 && environment.objects.ContainsKey(tileLocation))
              return;
            if ((int) (NetFieldBase<int, NetInt>) this.treeType == 8)
            {
              if (Game1.random.NextDouble() < 0.15 || this.fertilized.Value && Game1.random.NextDouble() < 0.6)
              {
                ++this.growthStage.Value;
                break;
              }
              break;
            }
            if (Game1.random.NextDouble() < 0.2 || this.fertilized.Value)
            {
              ++this.growthStage.Value;
              break;
            }
            break;
        }
      }
      if (Game1.GetSeasonForLocation(this.currentLocation).Equals("winter") && (int) (NetFieldBase<int, NetInt>) this.treeType == 7)
        this.stump.Value = true;
      else if ((int) (NetFieldBase<int, NetInt>) this.treeType == 7 && Game1.dayOfMonth <= 1 && Game1.currentSeason.Equals("spring"))
      {
        this.stump.Value = false;
        this.health.Value = 10f;
        this.shakeRotation = 0.0f;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 5 && environment is Farm && Game1.random.NextDouble() < 0.15)
      {
        int num1 = Game1.random.Next(-3, 4) + (int) tileLocation.X;
        int num2 = Game1.random.Next(-3, 4) + (int) tileLocation.Y;
        Vector2 vector2 = new Vector2((float) num1, (float) num2);
        switch (environment.doesTileHaveProperty(num1, num2, "NoSpawn", "Back"))
        {
          case nameof (Tree):
          case "All":
          case "True":
            break;
          default:
            if (environment.isTileLocationOpen(new Location(num1, num2)) && !environment.isTileOccupied(vector2) && environment.doesTileHaveProperty(num1, num2, "Water", "Back") == null && environment.isTileOnMap(vector2))
            {
              environment.terrainFeatures.Add(vector2, (TerrainFeature) new Tree((int) (NetFieldBase<int, NetInt>) this.treeType, 0));
              break;
            }
            break;
        }
      }
      this.hasSeed.Value = false;
      float num = 0.05f;
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 9)
        num *= 3f;
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage < 5 || Game1.random.NextDouble() >= (double) num)
        return;
      this.hasSeed.Value = true;
    }

    public override bool seasonUpdate(bool onLoad)
    {
      this.loadSprite();
      return false;
    }

    public override bool isActionable() => !(bool) (NetFieldBase<bool, NetBool>) this.tapped && (int) (NetFieldBase<int, NetInt>) this.growthStage >= 3;

    public override bool performToolAction(
      Tool t,
      int explosion,
      Vector2 tileLocation,
      GameLocation location)
    {
      if (location == null)
        location = Game1.currentLocation;
      if (explosion > 0)
        this.tapped.Value = false;
      if ((bool) (NetFieldBase<bool, NetBool>) this.tapped || (double) (float) (NetFieldBase<float, NetFloat>) this.health <= -99.0)
        return false;
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 5)
      {
        if (t != null && t is Axe)
        {
          location.playSound("axchop");
          this.lastPlayerToHit.Value = t.getLastFarmerToUse().UniqueMultiplayerID;
          location.debris.Add(new Debris(12, Game1.random.Next(1, 3), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), t.getLastFarmerToUse().Position, 0, (int) (NetFieldBase<int, NetInt>) this.treeType == 7 ? 10000 : -1));
          if (!(bool) (NetFieldBase<bool, NetBool>) this.stump && t.getLastFarmerToUse() != null && location.HasUnlockedAreaSecretNotes(t.getLastFarmerToUse()) && Game1.random.NextDouble() < 0.005)
          {
            StardewValley.Object unseenSecretNote = location.tryToCreateUnseenSecretNote(t.getLastFarmerToUse());
            if (unseenSecretNote != null)
              Game1.createItemDebris((Item) unseenSecretNote, new Vector2(tileLocation.X, tileLocation.Y - 3f) * 64f, -1, location, Game1.player.getStandingY() - 32);
          }
        }
        else if (explosion <= 0)
          return false;
        this.shake(tileLocation, true, location);
        float num = 1f;
        if (explosion > 0)
        {
          num = (float) explosion;
        }
        else
        {
          if (t == null)
            return false;
          switch ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel)
          {
            case 0:
              num = 1f;
              break;
            case 1:
              num = 1.25f;
              break;
            case 2:
              num = 1.67f;
              break;
            case 3:
              num = 2.5f;
              break;
            case 4:
              num = 5f;
              break;
          }
          if ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel > 4)
            num = (float) ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel + 1);
        }
        if (t is Axe && t.hasEnchantmentOfType<ShavingEnchantment>() && Game1.random.NextDouble() <= (double) num / 5.0)
        {
          Debris debris = new Debris(388, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) (((double) tileLocation.Y - 0.5) * 64.0 + 32.0)), new Vector2((float) Game1.player.getStandingX(), (float) Game1.player.getStandingY()));
          debris.Chunks[0].xVelocity.Value += (float) Game1.random.Next(-10, 11) / 10f;
          debris.chunkFinalYLevel = (int) ((double) tileLocation.Y * 64.0 + 64.0);
          location.debris.Add(debris);
        }
        this.health.Value -= num;
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0 && this.performTreeFall(t, explosion, tileLocation, location))
          return true;
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 3)
      {
        if (t != null && t.BaseName.Contains("Ax"))
        {
          location.playSound("axchop");
          if ((int) (NetFieldBase<int, NetInt>) this.treeType != 7)
            location.playSound("leafrustle");
          location.debris.Add(new Debris(12, Game1.random.Next((int) (NetFieldBase<int, NetInt>) t.upgradeLevel * 2, (int) (NetFieldBase<int, NetInt>) t.upgradeLevel * 4), t.getLastFarmerToUse().GetToolLocation() + new Vector2(16f, 0.0f), new Vector2((float) t.getLastFarmerToUse().GetBoundingBox().Center.X, (float) t.getLastFarmerToUse().GetBoundingBox().Center.Y), 0, -1));
        }
        else if (explosion <= 0)
          return false;
        this.shake(tileLocation, true, location);
        float num = 1f;
        if (Game1.IsMultiplayer)
        {
          Random multiplayerRandom = Game1.recentMultiplayerRandom;
        }
        else
        {
          Random random = new Random((int) ((double) Game1.uniqueIDForThisGame + (double) tileLocation.X * 7.0 + (double) tileLocation.Y * 11.0 + (double) Game1.stats.DaysPlayed + (double) (float) (NetFieldBase<float, NetFloat>) this.health));
        }
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
          if ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel > 4)
            num = (float) (10 + ((int) (NetFieldBase<int, NetInt>) t.upgradeLevel - 4));
        }
        this.health.Value -= num;
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.health <= 0.0)
        {
          this.performBushDestroy(tileLocation, location);
          return true;
        }
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 1)
      {
        if (explosion > 0)
        {
          location.playSound("cut");
          return true;
        }
        if (t != null && t.BaseName.Contains("Axe"))
        {
          location.playSound("axchop");
          Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(10, 20), false);
        }
        if (t is Axe || t is Pickaxe || t is Hoe || t is MeleeWeapon)
        {
          location.playSound("cut");
          this.performSproutDestroy(t, tileLocation, location);
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
          this.performSeedDestroy(t, tileLocation, location);
          return true;
        }
      }
      return false;
    }

    public bool fertilize(GameLocation location)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 5)
      {
        Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:TreeFertilizer1");
        location.playSound("cancel");
        return false;
      }
      if (this.fertilized.Value)
      {
        Game1.showRedMessageUsingLoadString("Strings\\StringsFromCSFiles:TreeFertilizer2");
        location.playSound("cancel");
        return false;
      }
      this.fertilized.Value = true;
      location.playSound("dirtyHit");
      return true;
    }

    public bool instantDestroy(Vector2 tileLocation, GameLocation location)
    {
      if (location == null)
        location = Game1.currentLocation;
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 5)
        return this.performTreeFall((Tool) null, 0, tileLocation, location);
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 3)
      {
        this.performBushDestroy(tileLocation, location);
        return true;
      }
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 1)
      {
        this.performSproutDestroy((Tool) null, tileLocation, location);
        return true;
      }
      this.performSeedDestroy((Tool) null, tileLocation, location);
      return true;
    }

    private void performSeedDestroy(Tool t, Vector2 tileLocation, GameLocation location)
    {
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));
      if ((long) this.lastPlayerToHit != 0L && Game1.getFarmer((long) this.lastPlayerToHit).getEffectiveSkillLevel(2) >= 1)
        Game1.createMultipleObjectDebris(308 + (int) (NetFieldBase<int, NetInt>) this.treeType, (int) tileLocation.X, (int) tileLocation.Y, 1, (long) t.getLastFarmerToUse().uniqueMultiplayerID, location);
      else if (Game1.player.getEffectiveSkillLevel(2) >= 1 && (int) (NetFieldBase<int, NetInt>) this.treeType <= 3)
      {
        Game1.createMultipleObjectDebris(308 + (int) (NetFieldBase<int, NetInt>) this.treeType, (int) tileLocation.X, (int) tileLocation.Y, 1, (long) (t == null ? Game1.player.uniqueMultiplayerID : t.getLastFarmerToUse().uniqueMultiplayerID), location);
      }
      else
      {
        if (Game1.player.getEffectiveSkillLevel(2) < 1 || (int) (NetFieldBase<int, NetInt>) this.treeType != 8)
          return;
        Game1.createMultipleObjectDebris(292, (int) tileLocation.X, (int) tileLocation.Y, 1, (long) (t == null ? Game1.player.uniqueMultiplayerID : t.getLastFarmerToUse().uniqueMultiplayerID), location);
      }
    }

    public void UpdateTapperProduct(StardewValley.Object tapper_instance, StardewValley.Object previous_object = null)
    {
      float num = 1f;
      if (tapper_instance != null && (int) (NetFieldBase<int, NetInt>) tapper_instance.parentSheetIndex == 264)
        num = 0.5f;
      switch ((int) (NetFieldBase<int, NetInt>) this.treeType)
      {
        case 1:
          tapper_instance.heldObject.Value = new StardewValley.Object(725, 1);
          tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int) Math.Max(1.0, Math.Floor(7.0 * (double) num)));
          break;
        case 2:
          tapper_instance.heldObject.Value = new StardewValley.Object(724, 1);
          tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int) Math.Max(1.0, Math.Floor(9.0 * (double) num)));
          break;
        case 3:
          tapper_instance.heldObject.Value = new StardewValley.Object(726, 1);
          tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int) Math.Max(1.0, Math.Floor(5.0 * (double) num)));
          break;
        case 7:
          if (previous_object == null)
          {
            tapper_instance.heldObject.Value = new StardewValley.Object(420, 1);
            tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
            if (Game1.GetSeasonForLocation(this.currentLocation).Equals("fall"))
              break;
            tapper_instance.heldObject.Value = new StardewValley.Object(404, 1);
            tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 2);
            break;
          }
          switch (previous_object.ParentSheetIndex)
          {
            case 404:
            case 420:
              tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
              tapper_instance.heldObject.Value = new StardewValley.Object(previous_object.ParentSheetIndex, 1);
              if (!Game1.GetSeasonForLocation(this.currentLocation).Equals("fall"))
              {
                tapper_instance.heldObject.Value = new StardewValley.Object(404, 1);
                tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 2);
              }
              if (Game1.dayOfMonth % 10 == 0)
                tapper_instance.heldObject.Value = new StardewValley.Object(422, 1);
              if (!Game1.GetSeasonForLocation(this.currentLocation).Equals("winter"))
                return;
              int daysElapsed = new WorldDate(Game1.year + 1, "spring", 1).TotalDays - Game1.Date.TotalDays;
              tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, daysElapsed);
              return;
            case 422:
              tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
              tapper_instance.heldObject.Value = new StardewValley.Object(420, 1);
              return;
            default:
              return;
          }
        case 8:
          Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + 73137);
          tapper_instance.heldObject.Value = new StardewValley.Object(92, random.Next(3, 8));
          tapper_instance.minutesUntilReady.Value = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int) Math.Max(1.0, Math.Floor(1.0 * (double) num)));
          break;
      }
    }

    private void performSproutDestroy(Tool t, Vector2 tileLocation, GameLocation location)
    {
      Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(10, 20), false);
      if (t != null && t.BaseName.Contains("Axe") && Game1.recentMultiplayerRandom.NextDouble() < (double) t.getLastFarmerToUse().ForagingLevel / 10.0)
        Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, 1);
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(17, tileLocation * 64f, Color.White));
    }

    private void performBushDestroy(Vector2 tileLocation, GameLocation location)
    {
      if ((int) (NetFieldBase<int, NetInt>) this.treeType == 7)
      {
        Game1.createMultipleObjectDebris(420, (int) tileLocation.X, (int) tileLocation.Y, 1, location);
        Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(20, 30), false, color: 10000);
      }
      else
      {
        Game1.createDebris(12, (int) tileLocation.X, (int) tileLocation.Y, (int) ((Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * 4.0), location);
        Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(20, 30), false);
      }
    }

    private bool performTreeFall(
      Tool t,
      int explosion,
      Vector2 tileLocation,
      GameLocation location)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.stump)
      {
        if (t != null || explosion > 0)
          location.playSound("treecrack");
        this.stump.Value = true;
        this.health.Value = 5f;
        this.falling.Value = true;
        if (t != null && t.getLastFarmerToUse().IsLocalPlayer)
        {
          t?.getLastFarmerToUse().gainExperience(2, 12);
          if (t == null || t.getLastFarmerToUse() == null)
            this.shakeLeft.Value = true;
          else
            this.shakeLeft.Value = (double) t.getLastFarmerToUse().getStandingX() > ((double) tileLocation.X + 0.5) * 64.0;
        }
      }
      else
      {
        if (t != null && (double) (float) (NetFieldBase<float, NetFloat>) this.health != -100.0 && t.getLastFarmerToUse().IsLocalPlayer && t != null)
          t.getLastFarmerToUse().gainExperience(2, 1);
        this.health.Value = -100f;
        Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, Game1.random.Next(30, 40), false, color: ((int) (NetFieldBase<int, NetInt>) this.treeType == 7 ? 10000 : -1));
        int index = (int) (NetFieldBase<int, NetInt>) this.treeType != 7 || (double) tileLocation.X % 7.0 != 0.0 ? ((int) (NetFieldBase<int, NetInt>) this.treeType == 7 ? 420 : ((int) (NetFieldBase<int, NetInt>) this.treeType == 8 ? 709 : 92)) : 422;
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
        {
          if (location.Equals(Game1.currentLocation))
          {
            Game1.createMultipleObjectDebris(92, (int) tileLocation.X, (int) tileLocation.Y, 2, location);
          }
          else
          {
            Game1.createItemDebris((Item) new StardewValley.Object(92, 1), tileLocation * 64f, 2, location);
            Game1.createItemDebris((Item) new StardewValley.Object(92, 1), tileLocation * 64f, 2, location);
          }
        }
        else if (Game1.IsMultiplayer)
        {
          Game1.createMultipleObjectDebris(index, (int) tileLocation.X, (int) tileLocation.Y, 1, (long) this.lastPlayerToHit, location);
          if ((int) (NetFieldBase<int, NetInt>) this.treeType != 7 && (int) (NetFieldBase<int, NetInt>) this.treeType != 8)
            Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, (int) ((Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * 4.0), true);
        }
        else
        {
          if ((int) (NetFieldBase<int, NetInt>) this.treeType != 7 && (int) (NetFieldBase<int, NetInt>) this.treeType != 8)
            Game1.createRadialDebris(location, 12, (int) tileLocation.X, (int) tileLocation.Y, (int) ((Game1.getFarmer((long) this.lastPlayerToHit).professions.Contains(12) ? 1.25 : 1.0) * (double) (5 + this.extraWoodCalculator(tileLocation))), true);
          Game1.createMultipleObjectDebris(index, (int) tileLocation.X, (int) tileLocation.Y, 1, location);
        }
        if (Game1.random.NextDouble() <= 0.25 && Game1.player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
          Game1.createObjectDebris(890, (int) tileLocation.X, (int) tileLocation.Y - 3, ((int) tileLocation.Y + 1) * 64, 0, 1f, location);
        location.playSound("treethud");
        if (!(bool) (NetFieldBase<bool, NetBool>) this.falling)
          return true;
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
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage < 5)
      {
        Microsoft.Xna.Framework.Rectangle rectangle = Microsoft.Xna.Framework.Rectangle.Empty;
        switch ((int) (NetFieldBase<int, NetInt>) this.growthStage)
        {
          case 0:
            rectangle = new Microsoft.Xna.Framework.Rectangle(32, 128, 16, 16);
            break;
          case 1:
            rectangle = new Microsoft.Xna.Framework.Rectangle(0, 128, 16, 16);
            break;
          case 2:
            rectangle = new Microsoft.Xna.Framework.Rectangle(16, 128, 16, 16);
            break;
          default:
            rectangle = new Microsoft.Xna.Framework.Rectangle(0, 96, 16, 32);
            break;
        }
        spriteBatch.Draw(this.texture.Value, positionOnScreen - new Vector2(0.0f, (float) rectangle.Height * scale), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, scale, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + (double) rectangle.Height * (double) scale) / 20000.0));
      }
      else
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.falling)
          spriteBatch.Draw(this.texture.Value, positionOnScreen + new Vector2(0.0f, -64f * scale), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 96, 16, 32)), Color.White, 0.0f, Vector2.Zero, scale, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale - 1.0) / 20000.0));
        if ((bool) (NetFieldBase<bool, NetBool>) this.stump && !(bool) (NetFieldBase<bool, NetBool>) this.falling)
          return;
        spriteBatch.Draw(this.texture.Value, positionOnScreen + new Vector2(-64f * scale, -320f * scale), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 96)), Color.White, this.shakeRotation, Vector2.Zero, scale, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (float) (((double) positionOnScreen.Y + 448.0 * (double) scale) / 20000.0));
      }
    }

    public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
    {
      if (this.isTemporarilyInvisible)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.growthStage < 5)
      {
        Microsoft.Xna.Framework.Rectangle rectangle = Microsoft.Xna.Framework.Rectangle.Empty;
        switch ((int) (NetFieldBase<int, NetInt>) this.growthStage)
        {
          case 0:
            rectangle = new Microsoft.Xna.Framework.Rectangle(32, 128, 16, 16);
            break;
          case 1:
            rectangle = new Microsoft.Xna.Framework.Rectangle(0, 128, 16, 16);
            break;
          case 2:
            rectangle = new Microsoft.Xna.Framework.Rectangle(16, 128, 16, 16);
            break;
          default:
            rectangle = new Microsoft.Xna.Framework.Rectangle(0, 96, 16, 32);
            break;
        }
        spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 - (double) (rectangle.Height * 4 - 64) + ((int) (NetFieldBase<int, NetInt>) this.growthStage >= 3 ? 128.0 : 64.0)))), new Microsoft.Xna.Framework.Rectangle?(rectangle), (bool) (NetFieldBase<bool, NetBool>) this.fertilized ? Color.HotPink : Color.White, this.shakeRotation, new Vector2(8f, (int) (NetFieldBase<int, NetInt>) this.growthStage >= 3 ? 32f : 16f), 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (int) (NetFieldBase<int, NetInt>) this.growthStage == 0 ? 0.0001f : (float) this.getBoundingBox(tileLocation).Bottom / 10000f);
      }
      else
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) this.stump || (bool) (NetFieldBase<bool, NetBool>) this.falling)
        {
          spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 - 51.0), (float) ((double) tileLocation.Y * 64.0 - 16.0))), new Microsoft.Xna.Framework.Rectangle?(Tree.shadowSourceRect), Color.White * (1.570796f - Math.Abs(this.shakeRotation)), 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1E-06f);
          Microsoft.Xna.Framework.Rectangle treeTopSourceRect = this.treeTopSourceRect;
          if (this.treeType.Value == 9)
            treeTopSourceRect.X = !this.hasSeed.Value ? 0 : 48;
          spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + 32.0), (float) ((double) tileLocation.Y * 64.0 + 64.0))), new Microsoft.Xna.Framework.Rectangle?(treeTopSourceRect), Color.White * this.alpha, this.shakeRotation, new Vector2(24f, 96f), 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) ((double) (this.getBoundingBox(tileLocation).Bottom + 2) / 10000.0 - (double) tileLocation.X / 1000000.0));
        }
        if ((double) (float) (NetFieldBase<float, NetFloat>) this.health >= 1.0 || !(bool) (NetFieldBase<bool, NetBool>) this.falling && (double) (float) (NetFieldBase<float, NetFloat>) this.health > -99.0)
          spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + ((double) this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 3.0 : 0.0)), (float) ((double) tileLocation.Y * 64.0 - 64.0))), new Microsoft.Xna.Framework.Rectangle?(Tree.stumpSourceRect), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) this.getBoundingBox(tileLocation).Bottom / 10000f);
        if ((bool) (NetFieldBase<bool, NetBool>) this.stump && (double) (float) (NetFieldBase<float, NetFloat>) this.health < 4.0 && (double) (float) (NetFieldBase<float, NetFloat>) this.health > -99.0)
          spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + ((double) this.shakeTimer > 0.0 ? Math.Sin(2.0 * Math.PI / (double) this.shakeTimer) * 3.0 : 0.0)), tileLocation.Y * 64f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Math.Min(2, (int) (3.0 - (double) (float) (NetFieldBase<float, NetFloat>) this.health)) * 16, 144, 16, 16)), Color.White * this.alpha, 0.0f, Vector2.Zero, 4f, (bool) (NetFieldBase<bool, NetBool>) this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (this.getBoundingBox(tileLocation).Bottom + 1) / 10000f);
      }
      foreach (Leaf leaf in this.leaves)
        spriteBatch.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, leaf.position), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(16 + leaf.type % 2 * 8, 112 + leaf.type / 2 * 8, 8, 8)), Color.White, leaf.rotation, Vector2.Zero, 4f, SpriteEffects.None, (float) ((double) this.getBoundingBox(tileLocation).Bottom / 10000.0 + 0.00999999977648258));
    }
  }
}
