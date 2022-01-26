// Decompiled with JetBrains decompiler
// Type: StardewValley.Crop
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class Crop : INetObject<NetFields>
  {
    public const int mixedSeedIndex = 770;
    public const int seedPhase = 0;
    public const int grabHarvest = 0;
    public const int sickleHarvest = 1;
    public const int rowOfWildSeeds = 23;
    public const int finalPhaseLength = 99999;
    public const int forageCrop_springOnion = 1;
    public const int forageCrop_ginger = 2;
    public readonly NetIntList phaseDays = new NetIntList();
    [XmlElement("rowInSpriteSheet")]
    public readonly NetInt rowInSpriteSheet = new NetInt();
    [XmlElement("phaseToShow")]
    public readonly NetInt phaseToShow = new NetInt(-1);
    [XmlElement("currentPhase")]
    public readonly NetInt currentPhase = new NetInt();
    [XmlElement("harvestMethod")]
    public readonly NetInt harvestMethod = new NetInt();
    [XmlElement("indexOfHarvest")]
    public readonly NetInt indexOfHarvest = new NetInt();
    [XmlElement("regrowAfterHarvest")]
    public readonly NetInt regrowAfterHarvest = new NetInt();
    [XmlElement("dayOfCurrentPhase")]
    public readonly NetInt dayOfCurrentPhase = new NetInt();
    [XmlElement("minHarvest")]
    public readonly NetInt minHarvest = new NetInt();
    [XmlElement("maxHarvest")]
    public readonly NetInt maxHarvest = new NetInt();
    [XmlElement("maxHarvestIncreasePerFarmingLevel")]
    public readonly NetInt maxHarvestIncreasePerFarmingLevel = new NetInt();
    [XmlElement("daysOfUnclutteredGrowth")]
    public readonly NetInt daysOfUnclutteredGrowth = new NetInt();
    [XmlElement("whichForageCrop")]
    public readonly NetInt whichForageCrop = new NetInt();
    public readonly NetStringList seasonsToGrowIn = new NetStringList();
    [XmlElement("tintColor")]
    public readonly NetColor tintColor = new NetColor();
    [XmlElement("flip")]
    public readonly NetBool flip = new NetBool();
    [XmlElement("fullGrown")]
    public readonly NetBool fullyGrown = new NetBool();
    [XmlElement("raisedSeeds")]
    public readonly NetBool raisedSeeds = new NetBool();
    [XmlElement("programColored")]
    public readonly NetBool programColored = new NetBool();
    [XmlElement("dead")]
    public readonly NetBool dead = new NetBool();
    [XmlElement("forageCrop")]
    public readonly NetBool forageCrop = new NetBool();
    [XmlElement("chanceForExtraCrops")]
    public readonly NetDouble chanceForExtraCrops = new NetDouble(0.0);
    [XmlElement("seedIndex")]
    public readonly NetInt netSeedIndex = new NetInt(-1);
    private Vector2 drawPosition;
    private Vector2 tilePosition;
    private float layerDepth;
    private float coloredLayerDepth;
    private Rectangle sourceRect;
    private Rectangle coloredSourceRect;
    private static Vector2 origin = new Vector2(8f, 24f);
    private static Vector2 smallestTileSizeOrigin = new Vector2(8f, 8f);

    public NetFields NetFields { get; } = new NetFields();

    public Crop()
    {
      this.NetFields.AddFields((INetSerializable) this.phaseDays, (INetSerializable) this.rowInSpriteSheet, (INetSerializable) this.phaseToShow, (INetSerializable) this.currentPhase, (INetSerializable) this.harvestMethod, (INetSerializable) this.indexOfHarvest, (INetSerializable) this.regrowAfterHarvest, (INetSerializable) this.dayOfCurrentPhase, (INetSerializable) this.minHarvest, (INetSerializable) this.maxHarvest, (INetSerializable) this.maxHarvestIncreasePerFarmingLevel, (INetSerializable) this.daysOfUnclutteredGrowth, (INetSerializable) this.whichForageCrop, (INetSerializable) this.seasonsToGrowIn, (INetSerializable) this.tintColor, (INetSerializable) this.flip, (INetSerializable) this.fullyGrown, (INetSerializable) this.raisedSeeds, (INetSerializable) this.programColored, (INetSerializable) this.dead, (INetSerializable) this.forageCrop, (INetSerializable) this.chanceForExtraCrops, (INetSerializable) this.netSeedIndex);
      this.dayOfCurrentPhase.fieldChangeVisibleEvent += (NetFieldBase<int, NetInt>.FieldChange) ((x, y, z) => this.updateDrawMath(this.tilePosition));
      this.fullyGrown.fieldChangeVisibleEvent += (NetFieldBase<bool, NetBool>.FieldChange) ((x, y, z) => this.updateDrawMath(this.tilePosition));
    }

    public Crop(bool forageCrop, int which, int tileX, int tileY)
      : this()
    {
      this.forageCrop.Value = forageCrop;
      this.whichForageCrop.Value = which;
      this.fullyGrown.Value = true;
      this.currentPhase.Value = 5;
      this.updateDrawMath(new Vector2((float) tileX, (float) tileY));
    }

    public Crop(int seedIndex, int tileX, int tileY)
      : this()
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
      if (seedIndex == 770)
      {
        seedIndex = Crop.getRandomLowGradeCropForThisSeason(Game1.currentSeason);
        if (seedIndex == 473)
          --seedIndex;
        if (Game1.currentLocation is IslandLocation)
        {
          switch (Game1.random.Next(4))
          {
            case 0:
              seedIndex = 479;
              break;
            case 1:
              seedIndex = 833;
              break;
            case 2:
              seedIndex = 481;
              break;
            case 3:
              seedIndex = 478;
              break;
          }
        }
      }
      if (dictionary.ContainsKey(seedIndex))
      {
        string[] strArray1 = dictionary[seedIndex].Split('/');
        foreach (string str in strArray1[0].Split(' '))
          this.phaseDays.Add(Convert.ToInt32(str));
        this.phaseDays.Add(99999);
        foreach (string str in strArray1[1].Split(' '))
          this.seasonsToGrowIn.Add(str);
        this.rowInSpriteSheet.Value = Convert.ToInt32(strArray1[2]);
        if ((int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet == 23)
          this.whichForageCrop.Value = seedIndex;
        else
          this.netSeedIndex.Value = seedIndex;
        this.indexOfHarvest.Value = Convert.ToInt32(strArray1[3]);
        this.regrowAfterHarvest.Value = Convert.ToInt32(strArray1[4]);
        this.harvestMethod.Value = Convert.ToInt32(strArray1[5]);
        this.ResetCropYield();
        this.raisedSeeds.Value = Convert.ToBoolean(strArray1[7]);
        string[] strArray2 = strArray1[8].Split(' ');
        if (strArray2.Length != 0 && strArray2[0].Equals("true"))
        {
          List<Color> colorList = new List<Color>();
          for (int index = 1; index < strArray2.Length; index += 3)
            colorList.Add(new Color((int) Convert.ToByte(strArray2[index]), (int) Convert.ToByte(strArray2[index + 1]), (int) Convert.ToByte(strArray2[index + 2])));
          Random random = new Random(tileX * 1000 + tileY + Game1.dayOfMonth);
          this.tintColor.Value = colorList[random.Next(colorList.Count)];
          this.programColored.Value = true;
        }
        this.flip.Value = Game1.random.NextDouble() < 0.5;
      }
      this.updateDrawMath(new Vector2((float) tileX, (float) tileY));
    }

    public virtual void InferSeedIndex()
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
      foreach (int key in dictionary.Keys)
      {
        if (Convert.ToInt32(dictionary[key].Split('/')[3]) == this.indexOfHarvest.Value)
        {
          this.netSeedIndex.Value = key;
          break;
        }
      }
    }

    public virtual void ResetCropYield()
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
      if ((int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet != 23 && this.netSeedIndex.Value == -1)
        this.InferSeedIndex();
      int key = (int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet != 23 ? this.netSeedIndex.Value : this.whichForageCrop.Value;
      if (!dictionary.ContainsKey(key))
        return;
      string[] strArray = dictionary[key].Split('/')[6].Split(' ');
      if (strArray.Length != 0 && strArray[0].Equals("true"))
      {
        this.minHarvest.Value = Convert.ToInt32(strArray[1]);
        this.maxHarvest.Value = Convert.ToInt32(strArray[2]);
        this.maxHarvestIncreasePerFarmingLevel.Value = Convert.ToInt32(strArray[3]);
        this.chanceForExtraCrops.Value = Convert.ToDouble(strArray[4]);
      }
      else
      {
        this.minHarvest.Value = 1;
        this.maxHarvest.Value = 1;
        this.maxHarvestIncreasePerFarmingLevel.Value = 0;
        this.chanceForExtraCrops.Value = 0.0;
      }
    }

    public virtual void ResetPhaseDays()
    {
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Crops");
      if ((int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet != 23 && this.netSeedIndex.Value == -1)
        this.InferSeedIndex();
      int key = (int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet != 23 ? this.netSeedIndex.Value : this.whichForageCrop.Value;
      if (!dictionary.ContainsKey(key))
        return;
      string[] strArray = dictionary[key].Split('/')[0].Split(' ');
      this.phaseDays.Clear();
      for (int index = 0; index < strArray.Length; ++index)
        this.phaseDays.Add(Convert.ToInt32(strArray[index]));
      this.phaseDays.Add(99999);
    }

    public static int getRandomLowGradeCropForThisSeason(string season)
    {
      if (season.Equals("winter"))
        season = Game1.random.NextDouble() < 0.33 ? "spring" : (Game1.random.NextDouble() < 0.5 ? "summer" : "fall");
      if (season == "spring")
        return Game1.random.Next(472, 476);
      if (!(season == "summer"))
      {
        if (season == "fall")
          return Game1.random.Next(487, 491);
      }
      else
      {
        switch (Game1.random.Next(4))
        {
          case 0:
            return 487;
          case 1:
            return 483;
          case 2:
            return 482;
          case 3:
            return 484;
        }
      }
      return -1;
    }

    public virtual void growCompletely()
    {
      this.currentPhase.Value = this.phaseDays.Count - 1;
      this.dayOfCurrentPhase.Value = 0;
      if ((int) (NetFieldBase<int, NetInt>) this.regrowAfterHarvest != -1)
        this.fullyGrown.Value = true;
      this.updateDrawMath(this.tilePosition);
    }

    public virtual bool hitWithHoe(int xTile, int yTile, GameLocation location, HoeDirt dirt)
    {
      if (!(bool) (NetFieldBase<bool, NetBool>) this.forageCrop || (int) (NetFieldBase<int, NetInt>) this.whichForageCrop != 2)
        return false;
      dirt.state.Value = Game1.IsRainingHere(location) ? 1 : 0;
      Object @object = new Object(829, 1);
      Game1.multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2((float) (xTile * 64), (float) (yTile * 64)), Color.White, flipped: (Game1.random.NextDouble() < 0.5), animationInterval: 50f));
      location.playSound("dirtyHit");
      Game1.createItemDebris(@object.getOne(), new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), -1);
      return true;
    }

    public virtual bool harvest(
      int xTile,
      int yTile,
      HoeDirt soil,
      JunimoHarvester junimoHarvester = null)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.dead)
        return junimoHarvester != null;
      bool flag = false;
      if ((bool) (NetFieldBase<bool, NetBool>) this.forageCrop)
      {
        Object i = (Object) null;
        int howMuch = 3;
        Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2 + xTile * 1000 + yTile * 2000);
        switch ((int) (NetFieldBase<int, NetInt>) this.whichForageCrop)
        {
          case 1:
            i = new Object(399, 1);
            break;
          case 2:
            soil.shake((float) Math.PI / 48f, (float) Math.PI / 40f, (double) (xTile * 64) < (double) Game1.player.Position.X);
            return false;
        }
        if (Game1.player.professions.Contains(16))
          i.Quality = 4;
        else if (random.NextDouble() < (double) Game1.player.ForagingLevel / 30.0)
          i.Quality = 2;
        else if (random.NextDouble() < (double) Game1.player.ForagingLevel / 15.0)
          i.Quality = 1;
        Game1.stats.ItemsForaged += (uint) i.Stack;
        if (junimoHarvester != null)
        {
          junimoHarvester.tryToAddItemToHut((Item) i);
          return true;
        }
        if (Game1.player.addItemToInventoryBool((Item) i))
        {
          Vector2 vector2 = new Vector2((float) xTile, (float) yTile);
          Game1.player.animateOnce(279 + Game1.player.FacingDirection);
          Game1.player.canMove = false;
          Game1.player.currentLocation.playSound(nameof (harvest));
          DelayedAction.playSoundAfterDelay("coin", 260);
          if ((int) (NetFieldBase<int, NetInt>) this.regrowAfterHarvest == -1)
          {
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(17, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, random.NextDouble() < 0.5, 125f));
            Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(14, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, random.NextDouble() < 0.5, 50f));
          }
          Game1.player.gainExperience(2, howMuch);
          return true;
        }
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
      }
      else if ((int) (NetFieldBase<int, NetInt>) this.currentPhase >= this.phaseDays.Count - 1 && (!(bool) (NetFieldBase<bool, NetBool>) this.fullyGrown || (int) (NetFieldBase<int, NetInt>) this.dayOfCurrentPhase <= 0))
      {
        int num1 = 1;
        int quality = 0;
        int num2 = 0;
        if ((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 0)
          return true;
        Random random = new Random(xTile * 7 + yTile * 11 + (int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame);
        switch ((int) (NetFieldBase<int, NetInt>) soil.fertilizer)
        {
          case 368:
            num2 = 1;
            break;
          case 369:
            num2 = 2;
            break;
          case 919:
            num2 = 3;
            break;
        }
        double num3 = 0.2 * ((double) Game1.player.FarmingLevel / 10.0) + 0.2 * (double) num2 * (((double) Game1.player.FarmingLevel + 2.0) / 12.0) + 0.01;
        double num4 = Math.Min(0.75, num3 * 2.0);
        if (num2 >= 3 && random.NextDouble() < num3 / 2.0)
          quality = 4;
        else if (random.NextDouble() < num3)
          quality = 2;
        else if (random.NextDouble() < num4 || num2 >= 3)
          quality = 1;
        if ((int) (NetFieldBase<int, NetInt>) this.minHarvest > 1 || (int) (NetFieldBase<int, NetInt>) this.maxHarvest > 1)
        {
          int num5 = 0;
          if (this.maxHarvestIncreasePerFarmingLevel.Value > 0)
            num5 = Game1.player.FarmingLevel / (int) (NetFieldBase<int, NetInt>) this.maxHarvestIncreasePerFarmingLevel;
          num1 = random.Next((int) (NetFieldBase<int, NetInt>) this.minHarvest, Math.Max((int) (NetFieldBase<int, NetInt>) this.minHarvest + 1, (int) (NetFieldBase<int, NetInt>) this.maxHarvest + 1 + num5));
        }
        if ((double) (NetFieldBase<double, NetDouble>) this.chanceForExtraCrops > 0.0)
        {
          while (random.NextDouble() < Math.Min(0.9, (double) (NetFieldBase<double, NetDouble>) this.chanceForExtraCrops))
            ++num1;
        }
        if ((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 771 || (int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 889)
          quality = 0;
        Object object1;
        if (!(bool) (NetFieldBase<bool, NetBool>) this.programColored)
        {
          object1 = new Object((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest, 1, quality: quality);
        }
        else
        {
          object1 = (Object) new ColoredObject((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest, 1, (Color) (NetFieldBase<Color, NetColor>) this.tintColor);
          object1.Quality = quality;
        }
        Object object2 = object1;
        if ((int) (NetFieldBase<int, NetInt>) this.harvestMethod == 1)
        {
          if (junimoHarvester != null)
            DelayedAction.playSoundAfterDelay("daggerswipe", 150, junimoHarvester.currentLocation);
          if (junimoHarvester != null && Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
            junimoHarvester.currentLocation.playSound(nameof (harvest));
          if (junimoHarvester != null && Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
            DelayedAction.playSoundAfterDelay("coin", 260, junimoHarvester.currentLocation);
          if (junimoHarvester != null)
            junimoHarvester.tryToAddItemToHut(object2.getOne());
          else
            Game1.createItemDebris(object2.getOne(), new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), -1);
          flag = true;
        }
        else if (junimoHarvester != null || Game1.player.addItemToInventoryBool(object2.getOne()))
        {
          Vector2 vector2 = new Vector2((float) xTile, (float) yTile);
          if (junimoHarvester == null)
          {
            Game1.player.animateOnce(279 + Game1.player.FacingDirection);
            Game1.player.canMove = false;
          }
          else
            junimoHarvester.tryToAddItemToHut(object2.getOne());
          if (random.NextDouble() < Game1.player.team.AverageLuckLevel() / 1500.0 + Game1.player.team.AverageDailyLuck() / 1200.0 + 9.99999974737875E-05)
          {
            num1 *= 2;
            if (junimoHarvester == null)
              Game1.player.currentLocation.playSound("dwoop");
            else if (Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
              junimoHarvester.currentLocation.playSound("dwoop");
          }
          else if ((int) (NetFieldBase<int, NetInt>) this.harvestMethod == 0)
          {
            if (junimoHarvester == null)
              Game1.player.currentLocation.playSound(nameof (harvest));
            else if (Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
              junimoHarvester.currentLocation.playSound(nameof (harvest));
            if (junimoHarvester == null)
              DelayedAction.playSoundAfterDelay("coin", 260, Game1.player.currentLocation);
            else if (Utility.isOnScreen(junimoHarvester.getTileLocationPoint(), 64, junimoHarvester.currentLocation))
              DelayedAction.playSoundAfterDelay("coin", 260, junimoHarvester.currentLocation);
            if ((int) (NetFieldBase<int, NetInt>) this.regrowAfterHarvest == -1 && (junimoHarvester == null || junimoHarvester.currentLocation.Equals(Game1.currentLocation)))
            {
              Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(17, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, Game1.random.NextDouble() < 0.5, 125f));
              Game1.multiplayer.broadcastSprites(Game1.currentLocation, new TemporaryAnimatedSprite(14, new Vector2(vector2.X * 64f, vector2.Y * 64f), Color.White, 7, Game1.random.NextDouble() < 0.5, 50f));
            }
          }
          flag = true;
        }
        else
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        if (flag)
        {
          if ((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 421)
          {
            this.indexOfHarvest.Value = 431;
            num1 = random.Next(1, 4);
          }
          int int32 = Convert.ToInt32(Game1.objectInformation[(int) (NetFieldBase<int, NetInt>) this.indexOfHarvest].Split('/')[1]);
          Object object3 = (bool) (NetFieldBase<bool, NetBool>) this.programColored ? (Object) new ColoredObject((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest, 1, (Color) (NetFieldBase<Color, NetColor>) this.tintColor) : new Object((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest, 1);
          float a = (float) (16.0 * Math.Log(0.018 * (double) int32 + 1.0, Math.E));
          if (junimoHarvester == null)
            Game1.player.gainExperience(0, (int) Math.Round((double) a));
          for (int index = 0; index < num1 - 1; ++index)
          {
            if (junimoHarvester == null)
              Game1.createItemDebris(object3.getOne(), new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), -1);
            else
              junimoHarvester.tryToAddItemToHut(object3.getOne());
          }
          if ((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 262 && random.NextDouble() < 0.4)
          {
            Object object4 = new Object(178, 1);
            if (junimoHarvester == null)
              Game1.createItemDebris(object4.getOne(), new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), -1);
            else
              junimoHarvester.tryToAddItemToHut(object4.getOne());
          }
          else if ((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 771)
          {
            if (soil != null && soil.currentLocation != null)
              soil.currentLocation.playSound("cut");
            if (random.NextDouble() < 0.1)
            {
              Object object5 = new Object(770, 1);
              if (junimoHarvester == null)
                Game1.createItemDebris(object5.getOne(), new Vector2((float) (xTile * 64 + 32), (float) (yTile * 64 + 32)), -1);
              else
                junimoHarvester.tryToAddItemToHut(object5.getOne());
            }
          }
          if ((int) (NetFieldBase<int, NetInt>) this.regrowAfterHarvest == -1)
            return true;
          this.fullyGrown.Value = true;
          if (this.dayOfCurrentPhase.Value == (int) (NetFieldBase<int, NetInt>) this.regrowAfterHarvest)
            this.updateDrawMath(this.tilePosition);
          this.dayOfCurrentPhase.Value = (int) (NetFieldBase<int, NetInt>) this.regrowAfterHarvest;
        }
      }
      return false;
    }

    public virtual int getRandomWildCropForSeason(string season)
    {
      if (season == "spring")
        return 16 + Game1.random.Next(4) * 2;
      if (!(season == "summer"))
      {
        if (season == "fall")
          return 404 + Game1.random.Next(4) * 2;
        return season == "winter" ? 412 + Game1.random.Next(4) * 2 : 22;
      }
      if (Game1.random.NextDouble() < 0.33)
        return 396;
      return Game1.random.NextDouble() >= 0.5 ? 402 : 398;
    }

    public virtual Rectangle getSourceRect(int number)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.dead)
        return new Rectangle(192 + number % 4 * 16, 384, 16, 32);
      int num = (int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet;
      if ((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 771)
      {
        if (Game1.currentSeason == "fall")
          num = (int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet + 1;
        else if (Game1.currentSeason == "winter")
          num = (int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet + 2;
      }
      return new Rectangle(Math.Min(240, ((bool) (NetFieldBase<bool, NetBool>) this.fullyGrown ? ((int) (NetFieldBase<int, NetInt>) this.dayOfCurrentPhase <= 0 ? 6 : 7) : (int) ((int) (NetFieldBase<int, NetInt>) this.phaseToShow != -1 ? (NetFieldBase<int, NetInt>) this.phaseToShow : (NetFieldBase<int, NetInt>) this.currentPhase) + ((int) ((int) (NetFieldBase<int, NetInt>) this.phaseToShow != -1 ? (NetFieldBase<int, NetInt>) this.phaseToShow : (NetFieldBase<int, NetInt>) this.currentPhase) != 0 || number % 2 != 0 ? 0 : -1) + 1) * 16 + (num % 2 != 0 ? 128 : 0)), num / 2 * 16 * 2, 16, 32);
    }

    public void Kill()
    {
      this.dead.Value = true;
      this.raisedSeeds.Value = false;
    }

    public virtual void newDay(
      int state,
      int fertilizer,
      int xTile,
      int yTile,
      GameLocation environment)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) environment.isOutdoors && ((bool) (NetFieldBase<bool, NetBool>) this.dead || !environment.SeedsIgnoreSeasonsHere() && !this.seasonsToGrowIn.Contains(environment.GetSeasonForLocation()) || !environment.SeedsIgnoreSeasonsHere() && (int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 90))
      {
        this.Kill();
      }
      else
      {
        if (state == 1 || (int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 771)
        {
          if (!(bool) (NetFieldBase<bool, NetBool>) this.fullyGrown)
            this.dayOfCurrentPhase.Value = Math.Min((int) (NetFieldBase<int, NetInt>) this.dayOfCurrentPhase + 1, this.phaseDays.Count > 0 ? this.phaseDays[Math.Min(this.phaseDays.Count - 1, (int) (NetFieldBase<int, NetInt>) this.currentPhase)] : 0);
          else
            --this.dayOfCurrentPhase.Value;
          if ((int) (NetFieldBase<int, NetInt>) this.dayOfCurrentPhase >= (this.phaseDays.Count > 0 ? this.phaseDays[Math.Min(this.phaseDays.Count - 1, (int) (NetFieldBase<int, NetInt>) this.currentPhase)] : 0) && (int) (NetFieldBase<int, NetInt>) this.currentPhase < this.phaseDays.Count - 1)
          {
            ++this.currentPhase.Value;
            this.dayOfCurrentPhase.Value = 0;
          }
          while ((int) (NetFieldBase<int, NetInt>) this.currentPhase < this.phaseDays.Count - 1 && this.phaseDays.Count > 0 && this.phaseDays[(int) (NetFieldBase<int, NetInt>) this.currentPhase] <= 0)
            ++this.currentPhase.Value;
          if ((int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet == 23 && (int) (NetFieldBase<int, NetInt>) this.phaseToShow == -1 && (int) (NetFieldBase<int, NetInt>) this.currentPhase > 0)
            this.phaseToShow.Value = Game1.random.Next(1, 7);
          if (environment is Farm && (int) (NetFieldBase<int, NetInt>) this.currentPhase == this.phaseDays.Count - 1 && ((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 276 || (int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 190 || (int) (NetFieldBase<int, NetInt>) this.indexOfHarvest == 254) && OneTimeRandom.GetDouble(Game1.uniqueIDForThisGame, (ulong) Game1.stats.DaysPlayed, (ulong) xTile, (ulong) yTile) < 0.01)
          {
            for (int x = xTile - 1; x <= xTile + 1; ++x)
            {
              for (int y = yTile - 1; y <= yTile + 1; ++y)
              {
                Vector2 key = new Vector2((float) x, (float) y);
                if (!environment.terrainFeatures.ContainsKey(key) || !(environment.terrainFeatures[key] is HoeDirt) || (environment.terrainFeatures[key] as HoeDirt).crop == null || (NetFieldBase<int, NetInt>) (environment.terrainFeatures[key] as HoeDirt).crop.indexOfHarvest != this.indexOfHarvest)
                  return;
              }
            }
            for (int x = xTile - 1; x <= xTile + 1; ++x)
            {
              for (int y = yTile - 1; y <= yTile + 1; ++y)
              {
                Vector2 key = new Vector2((float) x, (float) y);
                (environment.terrainFeatures[key] as HoeDirt).crop = (Crop) null;
              }
            }
            (environment as Farm).resourceClumps.Add((ResourceClump) new GiantCrop((int) (NetFieldBase<int, NetInt>) this.indexOfHarvest, new Vector2((float) (xTile - 1), (float) (yTile - 1))));
          }
        }
        if ((!(bool) (NetFieldBase<bool, NetBool>) this.fullyGrown || (int) (NetFieldBase<int, NetInt>) this.dayOfCurrentPhase <= 0) && (int) (NetFieldBase<int, NetInt>) this.currentPhase >= this.phaseDays.Count - 1 && (int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet == 23)
        {
          Vector2 vector2 = new Vector2((float) xTile, (float) yTile);
          string season = Game1.currentSeason;
          switch ((int) (NetFieldBase<int, NetInt>) this.whichForageCrop)
          {
            case 495:
              season = "spring";
              break;
            case 496:
              season = "summer";
              break;
            case 497:
              season = "fall";
              break;
            case 498:
              season = "winter";
              break;
          }
          if (environment.objects.ContainsKey(vector2))
          {
            if (environment.objects[vector2] is IndoorPot)
            {
              (environment.objects[vector2] as IndoorPot).heldObject.Value = new Object(vector2, this.getRandomWildCropForSeason(season), 1);
              (environment.objects[vector2] as IndoorPot).hoeDirt.Value.crop = (Crop) null;
            }
            else
              environment.objects.Remove(vector2);
          }
          if (!environment.objects.ContainsKey(vector2))
            environment.objects.Add(vector2, new Object(vector2, this.getRandomWildCropForSeason(season), 1)
            {
              IsSpawnedObject = true,
              CanBeGrabbed = true
            });
          if (environment.terrainFeatures.ContainsKey(vector2) && environment.terrainFeatures[vector2] != null && environment.terrainFeatures[vector2] is HoeDirt)
            (environment.terrainFeatures[vector2] as HoeDirt).crop = (Crop) null;
        }
        this.updateDrawMath(new Vector2((float) xTile, (float) yTile));
      }
    }

    public virtual bool isPaddyCrop() => this.indexOfHarvest.Value == 271 || this.indexOfHarvest.Value == 830;

    public virtual bool shouldDrawDarkWhenWatered() => !this.isPaddyCrop() && !this.raisedSeeds.Value;

    public virtual bool isWildSeedCrop() => (int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet == 23;

    public virtual void updateDrawMath(Vector2 tileLocation)
    {
      if (tileLocation.Equals(Vector2.Zero))
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.forageCrop)
      {
        this.drawPosition = new Vector2((float) ((double) tileLocation.X * 64.0 + (((double) tileLocation.X * 11.0 + (double) tileLocation.Y * 7.0) % 10.0 - 5.0) + 32.0), (float) ((double) tileLocation.Y * 64.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0) + 32.0));
        this.layerDepth = (float) (((double) tileLocation.Y * 64.0 + 32.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0);
        this.sourceRect = new Rectangle((int) ((double) tileLocation.X * 51.0 + (double) tileLocation.Y * 77.0) % 3 * 16, 128 + (int) (NetFieldBase<int, NetInt>) this.whichForageCrop * 16, 16, 16);
      }
      else
      {
        this.drawPosition = new Vector2((float) ((double) tileLocation.X * 64.0 + (!this.shouldDrawDarkWhenWatered() || (int) (NetFieldBase<int, NetInt>) this.currentPhase >= this.phaseDays.Count - 1 ? 0.0 : ((double) tileLocation.X * 11.0 + (double) tileLocation.Y * 7.0) % 10.0 - 5.0) + 32.0), (float) ((double) tileLocation.Y * 64.0 + ((bool) (NetFieldBase<bool, NetBool>) this.raisedSeeds || (int) (NetFieldBase<int, NetInt>) this.currentPhase >= this.phaseDays.Count - 1 ? 0.0 : ((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0) + 32.0));
        this.layerDepth = (float) (((double) tileLocation.Y * 64.0 + 32.0 + (!this.shouldDrawDarkWhenWatered() || (int) (NetFieldBase<int, NetInt>) this.currentPhase >= this.phaseDays.Count - 1 ? 0.0 : ((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0 / ((int) (NetFieldBase<int, NetInt>) this.currentPhase != 0 || !this.shouldDrawDarkWhenWatered() ? 1.0 : 2.0));
        this.sourceRect = this.getSourceRect((int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
        this.coloredSourceRect = new Rectangle(((bool) (NetFieldBase<bool, NetBool>) this.fullyGrown ? ((int) (NetFieldBase<int, NetInt>) this.dayOfCurrentPhase <= 0 ? 6 : 7) : (int) (NetFieldBase<int, NetInt>) this.currentPhase + 1 + 1) * 16 + ((int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet % 2 != 0 ? 128 : 0), (int) (NetFieldBase<int, NetInt>) this.rowInSpriteSheet / 2 * 16 * 2, 16, 32);
        this.coloredLayerDepth = (float) (((double) tileLocation.Y * 64.0 + 32.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0 / ((int) (NetFieldBase<int, NetInt>) this.currentPhase != 0 || !this.shouldDrawDarkWhenWatered() ? 1.0 : 2.0));
      }
      this.tilePosition = tileLocation;
    }

    public virtual void draw(SpriteBatch b, Vector2 tileLocation, Color toTint, float rotation)
    {
      Vector2 local = Game1.GlobalToLocal(Game1.viewport, this.drawPosition);
      if ((bool) (NetFieldBase<bool, NetBool>) this.forageCrop)
      {
        if ((int) (NetFieldBase<int, NetInt>) this.whichForageCrop == 2)
          b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) ((double) tileLocation.X * 64.0 + (((double) tileLocation.X * 11.0 + (double) tileLocation.Y * 7.0) % 10.0 - 5.0) + 32.0), (float) ((double) tileLocation.Y * 64.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0) + 64.0))), new Rectangle?(new Rectangle(128 + (int) ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + ((double) tileLocation.X * 111.0 + (double) tileLocation.Y * 77.0)) % 800.0 / 200.0) * 16, 128, 16, 16)), Color.White, rotation, new Vector2(8f, 16f), 4f, SpriteEffects.None, (float) (((double) tileLocation.Y * 64.0 + 32.0 + (((double) tileLocation.Y * 11.0 + (double) tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));
        else
          b.Draw(Game1.mouseCursors, local, new Rectangle?(this.sourceRect), Color.White, 0.0f, Crop.smallestTileSizeOrigin, 4f, SpriteEffects.None, this.layerDepth);
      }
      else
      {
        SpriteEffects effects = (bool) (NetFieldBase<bool, NetBool>) this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        b.Draw(Game1.cropSpriteSheet, local, new Rectangle?(this.sourceRect), toTint, rotation, Crop.origin, 4f, effects, this.layerDepth);
        Color color = this.tintColor.Value;
        if (color.Equals(Color.White) || (int) (NetFieldBase<int, NetInt>) this.currentPhase != this.phaseDays.Count - 1 || (bool) (NetFieldBase<bool, NetBool>) this.dead)
          return;
        b.Draw(Game1.cropSpriteSheet, local, new Rectangle?(this.coloredSourceRect), color, rotation, Crop.origin, 4f, effects, this.coloredLayerDepth);
      }
    }

    public virtual void drawInMenu(
      SpriteBatch b,
      Vector2 screenPosition,
      Color toTint,
      float rotation,
      float scale,
      float layerDepth)
    {
      b.Draw(Game1.cropSpriteSheet, screenPosition, new Rectangle?(this.getSourceRect(0)), toTint, rotation, new Vector2(32f, 96f), scale, (bool) (NetFieldBase<bool, NetBool>) this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
    }

    public virtual void drawWithOffset(
      SpriteBatch b,
      Vector2 tileLocation,
      Color toTint,
      float rotation,
      Vector2 offset)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.forageCrop)
      {
        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(this.sourceRect), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float) (((double) tileLocation.Y + 0.660000026226044) * 64.0 / 10000.0 + (double) tileLocation.X * 9.99999974737875E-06));
      }
      else
      {
        b.Draw(Game1.cropSpriteSheet, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(this.sourceRect), toTint, rotation, new Vector2(8f, 24f), 4f, (bool) (NetFieldBase<bool, NetBool>) this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) tileLocation.Y + 0.660000026226044) * 64.0 / 10000.0 + (double) tileLocation.X * 9.99999974737875E-06));
        if (this.tintColor.Equals(Color.White) || (int) (NetFieldBase<int, NetInt>) this.currentPhase != this.phaseDays.Count - 1 || (bool) (NetFieldBase<bool, NetBool>) this.dead)
          return;
        b.Draw(Game1.cropSpriteSheet, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), new Rectangle?(this.coloredSourceRect), (Color) (NetFieldBase<Color, NetColor>) this.tintColor, rotation, new Vector2(8f, 24f), 4f, (bool) (NetFieldBase<bool, NetBool>) this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (float) (((double) tileLocation.Y + 0.670000016689301) * 64.0 / 10000.0 + (double) tileLocation.X * 9.99999974737875E-06));
      }
    }
  }
}
