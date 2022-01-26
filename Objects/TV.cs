// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.TV
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StardewValley.Objects
{
  public class TV : Furniture
  {
    public const int customChannel = 1;
    public const int weatherChannel = 2;
    public const int fortuneTellerChannel = 3;
    public const int tipsChannel = 4;
    public const int cookingChannel = 5;
    public const int fishingChannel = 6;
    private int currentChannel;
    private TemporaryAnimatedSprite screen;
    private TemporaryAnimatedSprite screenOverlay;
    private static Dictionary<int, string> weekToRecipeMap;

    public TV()
    {
    }

    public TV(int which, Vector2 tile)
      : base(which, tile)
    {
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity)
        return true;
      List<Response> responseList = new List<Response>();
      responseList.Add(new Response("Weather", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13105")));
      responseList.Add(new Response("Fortune", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13107")));
      string str = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
      if (str.Equals("Mon") || str.Equals("Thu"))
        responseList.Add(new Response("Livin'", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13111")));
      if (str.Equals("Sun"))
        responseList.Add(new Response("The", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13114")));
      if (str.Equals("Wed") && Game1.stats.DaysPlayed > 7U)
        responseList.Add(new Response("The", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13117")));
      if (Game1.Date.Season == "fall" && Game1.Date.DayOfMonth == 26 && Game1.stats.getStat("childrenTurnedToDoves") > 0U && !who.mailReceived.Contains("cursed_doll"))
        responseList.Add(new Response("???", "???"));
      if (Game1.player.mailReceived.Contains("pamNewChannel"))
        responseList.Add(new Response("Fishing", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel")));
      responseList.Add(new Response("(Leave)", Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13118")));
      Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13120"), responseList.ToArray(), new GameLocation.afterQuestionBehavior(this.selectChannel));
      Game1.player.Halt();
      return true;
    }

    public override Item getOne()
    {
      TV one = new TV((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex, (Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      one.drawPosition.Value = (Vector2) (NetFieldBase<Vector2, NetVector2>) this.drawPosition;
      one.defaultBoundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.defaultBoundingBox;
      one.boundingBox.Value = (Rectangle) (NetFieldBase<Rectangle, NetRectangle>) this.boundingBox;
      one.currentRotation.Value = (int) (NetFieldBase<int, NetInt>) this.currentRotation - 1;
      one.rotations.Value = (int) (NetFieldBase<int, NetInt>) this.rotations;
      one.rotate();
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment) => base.updateWhenCurrentLocation(time, environment);

    public virtual void selectChannel(Farmer who, string answer)
    {
      string str = answer.Split(' ')[0];
      if (!(str == "Weather"))
      {
        if (!(str == "Fortune"))
        {
          if (!(str == "Livin'"))
          {
            if (!(str == "The"))
            {
              if (!(str == "???"))
              {
                if (!(str == "Fishing"))
                  return;
                this.currentChannel = 6;
                this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(172, 33, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Fishing_Channel_Intro")));
                Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
              }
              else
              {
                Game1.changeMusicTrack("none");
                this.currentChannel = 666;
                this.screen = new TemporaryAnimatedSprite("Maps\\springobjects", new Rectangle(112, 64, 16, 16), 150f, 1, 999999, this.getScreenPosition() + ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1468 ? new Vector2(56f, 32f) : new Vector2(8f, 8f)), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, 3f, 0.0f, 0.0f, 0.0f);
                Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Cursed_Doll")));
                Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
              }
            }
            else
            {
              this.currentChannel = 5;
              this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(602, 361, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
              Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13127")));
              Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
            }
          }
          else
          {
            this.currentChannel = 4;
            this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(517, 361, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13124")));
            Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          }
        }
        else
        {
          this.currentChannel = 3;
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(540, 305, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(this.getFortuneTellerOpening()));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
        }
      }
      else
      {
        this.currentChannel = 2;
        this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(413, 305, 42, 28), 150f, 2, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
        Game1.drawObjectDialogue(Game1.parseText(this.getWeatherChannelOpening()));
        Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
      }
    }

    protected virtual string getFortuneTellerOpening()
    {
      switch (Game1.random.Next(5))
      {
        case 0:
          return !Game1.player.IsMale ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13130") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13128");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13132");
        case 2:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13133");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13134");
        case 4:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13135");
        default:
          return "";
      }
    }

    protected virtual string getWeatherChannelOpening() => Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13136");

    public virtual float getScreenSizeModifier() => (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 1468 && (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex != 2326 ? 2f : 4f;

    public virtual Vector2 getScreenPosition()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1466)
        return new Vector2((float) (this.boundingBox.X + 24), (float) this.boundingBox.Y);
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1468)
        return new Vector2((float) (this.boundingBox.X + 12), (float) (this.boundingBox.Y - 128 + 32));
      if ((int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 2326)
        return new Vector2((float) (this.boundingBox.X + 12), (float) (this.boundingBox.Y - 128 + 40));
      return (int) (NetFieldBase<int, NetInt>) this.parentSheetIndex == 1680 ? new Vector2((float) (this.boundingBox.X + 24), (float) (this.boundingBox.Y - 12)) : Vector2.Zero;
    }

    public virtual void proceedToNextScene()
    {
      if (this.currentChannel == 2)
      {
        if (this.screenOverlay == null)
        {
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(497, 305, 42, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f)
          {
            id = 777f
          };
          Game1.drawObjectDialogue(Game1.parseText(this.getWeatherForecast()));
          this.setWeatherOverlay();
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
        }
        else if (Game1.player.hasOrWillReceiveMail("Visited_Island") && (double) this.screen.id == 777.0)
        {
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors2", new Rectangle(148, 62, 42, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(this.getIslandWeatherForecast()));
          this.setWeatherOverlay(true);
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
        }
        else
          this.turnOffTV();
      }
      else if (this.currentChannel == 3)
      {
        if (this.screenOverlay == null)
        {
          this.screen = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(624, 305, 42, 28), 9999f, 1, 999999, this.getScreenPosition(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          Game1.drawObjectDialogue(Game1.parseText(this.getFortuneForecast(Game1.player)));
          this.setFortuneOverlay(Game1.player);
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
        }
        else
          this.turnOffTV();
      }
      else if (this.currentChannel == 4)
      {
        if (this.screenOverlay == null)
        {
          Game1.drawObjectDialogue(Game1.parseText(this.getTodaysTip()));
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          this.screenOverlay = new TemporaryAnimatedSprite()
          {
            alpha = 1E-07f
          };
        }
        else
          this.turnOffTV();
      }
      else if (this.currentChannel == 5)
      {
        if (this.screenOverlay == null)
        {
          Game1.multipleDialogues(this.getWeeklyRecipe());
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          this.screenOverlay = new TemporaryAnimatedSprite()
          {
            alpha = 1E-07f
          };
        }
        else
          this.turnOffTV();
      }
      else if (this.currentChannel == 666)
      {
        Game1.flashAlpha = 1f;
        Game1.soundBank.PlayCue("batScreech");
        Game1.createItemDebris((Item) new StardewValley.Object(103, 1), Game1.player.getStandingPosition(), 1, Game1.currentLocation);
        Game1.player.mailReceived.Add("cursed_doll");
        this.turnOffTV();
      }
      else
      {
        if (this.currentChannel != 6)
          return;
        if (this.screenOverlay == null)
        {
          Game1.multipleDialogues(this.getFishingInfo());
          Game1.afterDialogues = new Game1.afterFadeFunction(this.proceedToNextScene);
          this.screenOverlay = new TemporaryAnimatedSprite()
          {
            alpha = 1E-07f
          };
        }
        else
          this.turnOffTV();
      }
    }

    public virtual void turnOffTV()
    {
      this.screen = (TemporaryAnimatedSprite) null;
      this.screenOverlay = (TemporaryAnimatedSprite) null;
    }

    protected virtual void setWeatherOverlay(bool island = false)
    {
      WorldDate date = new WorldDate(Game1.Date);
      ++date.TotalDays;
      switch (!island ? (!Game1.IsMasterGame ? Game1.getWeatherModificationsForDate(date, Game1.netWorldState.Value.WeatherForTomorrow) : Game1.getWeatherModificationsForDate(date, Game1.weatherForTomorrow)) : Game1.netWorldState.Value.GetWeatherForLocation(Game1.getLocationFromName("IslandSouth").GetLocationContext()).weatherForTomorrow.Value)
      {
        case 0:
        case 6:
          this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(413, 333, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          break;
        case 1:
          this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(465, 333, 13, 13), 70f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          break;
        case 2:
          this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", Game1.currentSeason.Equals("spring") ? new Rectangle(465, 359, 13, 13) : (Game1.currentSeason.Equals("fall") ? new Rectangle(413, 359, 13, 13) : new Rectangle(465, 346, 13, 13)), 70f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          break;
        case 3:
          this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(413, 346, 13, 13), 120f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          break;
        case 4:
          this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(413, 372, 13, 13), 120f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          break;
        case 5:
          this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(465, 346, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(3f, 3f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
          break;
      }
    }

    private string[] getFishingInfo()
    {
      List<string> stringList1 = new List<string>();
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      int seasonNumber = Utility.getSeasonNumber(Game1.currentSeason);
      stringBuilder1.AppendLine("---" + Utility.getSeasonNameFromNumber(seasonNumber) + "---^^");
      Dictionary<int, string> dictionary1 = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
      Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
      List<string> stringList2 = new List<string>();
      int num = 0;
      foreach (KeyValuePair<int, string> keyValuePair1 in dictionary1)
      {
        if (!keyValuePair1.Value.Contains("spring summer fall winter"))
        {
          stringList2.Clear();
          foreach (KeyValuePair<string, string> keyValuePair2 in dictionary2)
          {
            if (keyValuePair2.Value.Split('/')[4 + seasonNumber].Contains(keyValuePair1.Key.ToString() ?? "") && !stringList2.Contains(this.getSanitizedFishingLocation(keyValuePair2.Key)))
              stringList2.Add(this.getSanitizedFishingLocation(keyValuePair2.Key));
          }
          if (stringList2.Count > 0)
          {
            string[] source = keyValuePair1.Value.Split('/');
            string str1 = ((IEnumerable<string>) source).Count<string>() > 13 ? source[13] : source[0];
            string str2 = source[7];
            string str3 = source[5].Split(' ')[0];
            string str4 = source[5].Split(' ')[1];
            stringBuilder2.Append(str1);
            stringBuilder2.Append("...... ");
            stringBuilder2.Append(Game1.getTimeOfDayString(Convert.ToInt32(str3)).Replace(" ", ""));
            stringBuilder2.Append("-");
            stringBuilder2.Append(Game1.getTimeOfDayString(Convert.ToInt32(str4)).Replace(" ", ""));
            if (str2 != "both")
              stringBuilder2.Append(", " + Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel_" + str2));
            bool flag = false;
            foreach (string str5 in stringList2)
            {
              if (str5 != "")
              {
                flag = true;
                stringBuilder2.Append(", ");
                stringBuilder2.Append(str5);
              }
            }
            if (flag)
            {
              stringBuilder2.Append("^^");
              stringBuilder1.Append(stringBuilder2.ToString());
              ++num;
            }
            stringBuilder2.Clear();
            if (num > 3)
            {
              stringList1.Add(stringBuilder1.ToString());
              stringBuilder1.Clear();
              num = 0;
            }
          }
        }
      }
      return stringList1.ToArray();
    }

    private string getSanitizedFishingLocation(string rawLocationName)
    {
      if (rawLocationName == "Town" || rawLocationName == "Forest")
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel_River");
      if (rawLocationName == "Beach")
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel_Ocean");
      return rawLocationName == "Mountain" ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_Fishing_Channel_Lake") : "";
    }

    protected virtual string getTodaysTip()
    {
      Dictionary<string, string> dictionary1 = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\TipChannel");
      Dictionary<string, string> dictionary2 = dictionary1;
      uint num = Game1.stats.DaysPlayed % 224U;
      string key1 = num.ToString() ?? "";
      if (!dictionary2.ContainsKey(key1))
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13148");
      Dictionary<string, string> dictionary3 = dictionary1;
      num = Game1.stats.DaysPlayed % 224U;
      string key2 = num.ToString() ?? "";
      return dictionary3[key2];
    }

    protected int getRerunWeek()
    {
      int maxValue = Math.Min(((int) Game1.stats.DaysPlayed - 3) / 7, 32);
      if (TV.weekToRecipeMap == null)
      {
        TV.weekToRecipeMap = new Dictionary<int, string>();
        Dictionary<string, string> dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\CookingChannel");
        foreach (string key in dictionary.Keys)
          TV.weekToRecipeMap[Convert.ToInt32(key)] = dictionary[key].Split('/')[0];
      }
      List<int> intList = new List<int>();
      IEnumerable<Farmer> allFarmers = Game1.getAllFarmers();
      for (int key = 1; key <= maxValue; ++key)
      {
        foreach (Farmer farmer in allFarmers)
        {
          if (!farmer.cookingRecipes.ContainsKey(TV.weekToRecipeMap[key]))
          {
            intList.Add(key);
            break;
          }
        }
      }
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      return intList.Count != 0 ? intList[random.Next(intList.Count)] : Math.Max(1, 1 + random.Next(maxValue));
    }

    protected virtual string[] getWeeklyRecipe()
    {
      string[] weeklyRecipe = new string[2];
      int num = (int) (Game1.stats.DaysPlayed % 224U / 7U);
      if (Game1.stats.DaysPlayed % 224U == 0U)
        num = 32;
      Dictionary<string, string> dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\TV\\CookingChannel");
      FarmerTeam team = Game1.player.team;
      if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Wed"))
      {
        if ((int) (NetFieldBase<int, NetInt>) team.lastDayQueenOfSauceRerunUpdated != Game1.Date.TotalDays)
        {
          team.lastDayQueenOfSauceRerunUpdated.Set(Game1.Date.TotalDays);
          team.queenOfSauceRerunWeek.Set(this.getRerunWeek());
        }
        num = team.queenOfSauceRerunWeek.Value;
      }
      try
      {
        string str = dictionary[num.ToString() ?? ""].Split('/')[0];
        weeklyRecipe[0] = dictionary[num.ToString() ?? ""].Split('/')[1];
        if (CraftingRecipe.cookingRecipes.ContainsKey(str))
        {
          string[] strArray = CraftingRecipe.cookingRecipes[str].Split('/');
          weeklyRecipe[1] = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? (Game1.player.cookingRecipes.ContainsKey(dictionary[num.ToString() ?? ""].Split('/')[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) str) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) str)) : (Game1.player.cookingRecipes.ContainsKey(dictionary[num.ToString() ?? ""].Split('/')[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) strArray[strArray.Length - 1]) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) strArray[strArray.Length - 1]));
        }
        else
          weeklyRecipe[1] = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? (Game1.player.cookingRecipes.ContainsKey(dictionary[num.ToString() ?? ""].Split('/')[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) str) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) str)) : (Game1.player.cookingRecipes.ContainsKey(dictionary[num.ToString() ?? ""].Split('/')[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) ((IEnumerable<string>) dictionary[num.ToString() ?? ""].Split('/')).Last<string>()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) ((IEnumerable<string>) dictionary[num.ToString() ?? ""].Split('/')).Last<string>()));
        if (!Game1.player.cookingRecipes.ContainsKey(str))
          Game1.player.cookingRecipes.Add(str, 0);
      }
      catch (Exception ex)
      {
        string str = dictionary["1"].Split('/')[0];
        weeklyRecipe[0] = dictionary["1"].Split('/')[1];
        weeklyRecipe[1] = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? (Game1.player.cookingRecipes.ContainsKey(dictionary["1"].Split('/')[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) str) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) str)) : (Game1.player.cookingRecipes.ContainsKey(dictionary["1"].Split('/')[0]) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) ((IEnumerable<string>) dictionary["1"].Split('/')).Last<string>()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) ((IEnumerable<string>) dictionary["1"].Split('/')).Last<string>()));
        if (!Game1.player.cookingRecipes.ContainsKey(str))
          Game1.player.cookingRecipes.Add(str, 0);
      }
      return weeklyRecipe;
    }

    private string getIslandWeatherForecast()
    {
      ++new WorldDate(Game1.Date).TotalDays;
      int num = Game1.netWorldState.Value.GetWeatherForLocation(Game1.getLocationFromName("IslandSouth").GetLocationContext()).weatherForTomorrow.Value;
      string str = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV_IslandWeatherIntro");
      string islandWeatherForecast;
      switch (num)
      {
        case 0:
          islandWeatherForecast = str + (Game1.random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13182") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13183"));
          break;
        case 1:
          islandWeatherForecast = str + Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13184");
          break;
        case 3:
          islandWeatherForecast = str + Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13185");
          break;
        default:
          islandWeatherForecast = str + "???";
          break;
      }
      return islandWeatherForecast;
    }

    protected virtual string getWeatherForecast()
    {
      WorldDate date = new WorldDate(Game1.Date);
      ++date.TotalDays;
      switch (!Game1.IsMasterGame ? Game1.getWeatherModificationsForDate(date, Game1.netWorldState.Value.WeatherForTomorrow) : Game1.getWeatherModificationsForDate(date, Game1.weatherForTomorrow))
      {
        case 0:
        case 6:
          return Game1.random.NextDouble() >= 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13183") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13182");
        case 1:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13184");
        case 2:
          if (Game1.currentSeason.Equals("spring"))
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13187");
          return !Game1.currentSeason.Equals("fall") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13190") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13189");
        case 3:
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13185");
        case 4:
          Dictionary<string, string> dictionary;
          try
          {
            dictionary = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + (Game1.dayOfMonth + 1).ToString());
          }
          catch (Exception ex)
          {
            return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13164");
          }
          string str1 = dictionary["name"];
          string str2 = dictionary["conditions"].Split('/')[0];
          int int32_1 = Convert.ToInt32(dictionary["conditions"].Split('/')[1].Split(' ')[0]);
          int int32_2 = Convert.ToInt32(dictionary["conditions"].Split('/')[1].Split(' ')[1]);
          string str3 = "";
          if (!(str2 == "Town"))
          {
            if (!(str2 == "Beach"))
            {
              if (str2 == "Forest")
                str3 = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13174");
            }
            else
              str3 = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13172");
          }
          else
            str3 = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13170");
          return Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13175", (object) str1, (object) str3, (object) Game1.getTimeOfDayString(int32_1), (object) Game1.getTimeOfDayString(int32_2));
        case 5:
          return Game1.random.NextDouble() >= 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13181") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13180");
        default:
          return "";
      }
    }

    public virtual void setFortuneOverlay(Farmer who)
    {
      if (who.DailyLuck < -0.07)
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(592, 346, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
      else if (who.DailyLuck < -0.02)
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(540, 346, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
      else if (who.DailyLuck > 0.07)
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(644, 333, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
      else if (who.DailyLuck > 0.02)
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(592, 333, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
      else
        this.screenOverlay = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(540, 333, 13, 13), 100f, 4, 999999, this.getScreenPosition() + new Vector2(15f, 1f) * this.getScreenSizeModifier(), false, false, (float) ((double) (this.boundingBox.Bottom - 1) / 10000.0 + 1.99999994947575E-05), 0.0f, Color.White, this.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f);
    }

    public virtual string getFortuneForecast(Farmer who)
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      string fortuneForecast = (double) (NetFieldBase<double, NetDouble>) who.team.sharedDailyLuck != -0.12 ? (who.DailyLuck >= -0.07 ? (who.DailyLuck >= -0.02 ? ((double) (NetFieldBase<double, NetDouble>) who.team.sharedDailyLuck != 0.12 ? (who.DailyLuck <= 0.07 ? (who.DailyLuck <= 0.02 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13200") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13199")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13198")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13197")) : (random.NextDouble() < 0.5 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13193") : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13195"))) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13192")) : Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13191");
      if (who.DailyLuck == 0.0)
        fortuneForecast = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13201");
      return fortuneForecast;
    }

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      base.draw(spriteBatch, x, y, alpha);
      if (this.screen == null)
        return;
      this.screen.update(Game1.currentGameTime);
      this.screen.draw(spriteBatch);
      if (this.screenOverlay == null)
        return;
      this.screenOverlay.update(Game1.currentGameTime);
      this.screenOverlay.draw(spriteBatch);
    }
  }
}
