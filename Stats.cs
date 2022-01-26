// Decompiled with JetBrains decompiler
// Type: StardewValley.Stats
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Locations;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class Stats
  {
    public uint seedsSown;
    public uint itemsShipped;
    public uint itemsCooked;
    public uint itemsCrafted;
    public uint chickenEggsLayed;
    public uint duckEggsLayed;
    public uint cowMilkProduced;
    public uint goatMilkProduced;
    public uint rabbitWoolProduced;
    public uint sheepWoolProduced;
    public uint cheeseMade;
    public uint goatCheeseMade;
    public uint trufflesFound;
    public uint stoneGathered;
    public uint rocksCrushed;
    public uint dirtHoed;
    public uint giftsGiven;
    public uint timesUnconscious;
    public uint averageBedtime;
    public uint timesFished;
    public uint fishCaught;
    public uint bouldersCracked;
    public uint stumpsChopped;
    public uint stepsTaken;
    public uint monstersKilled;
    public uint diamondsFound;
    public uint prismaticShardsFound;
    public uint otherPreciousGemsFound;
    public uint caveCarrotsFound;
    public uint copperFound;
    public uint ironFound;
    public uint coalFound;
    public uint coinsFound;
    public uint goldFound;
    public uint iridiumFound;
    public uint barsSmelted;
    public uint beveragesMade;
    public uint preservesMade;
    public uint piecesOfTrashRecycled;
    public uint mysticStonesCrushed;
    public uint daysPlayed;
    public uint weedsEliminated;
    public uint sticksChopped;
    public uint notesFound;
    public uint questsCompleted;
    public uint starLevelCropsShipped;
    public uint cropsShipped;
    public uint itemsForaged;
    public uint slimesKilled;
    public uint geodesCracked;
    public uint goodFriends;
    public uint totalMoneyGifted;
    public uint individualMoneyEarned;
    public SerializableDictionary<string, int> specificMonstersKilled = new SerializableDictionary<string, int>();
    public SerializableDictionary<string, uint> stat_dictionary = new SerializableDictionary<string, uint>();

    public uint getStat(string label) => this.stat_dictionary.ContainsKey(label) ? this.stat_dictionary[label] : 0U;

    public void incrementStat(string label, int amount)
    {
      if (this.stat_dictionary.ContainsKey(label))
        this.stat_dictionary[label] += (uint) amount;
      else
        this.stat_dictionary.Add(label, (uint) amount);
    }

    public void monsterKilled(string name)
    {
      if (this.specificMonstersKilled.ContainsKey(name))
      {
        if (AdventureGuild.willThisKillCompleteAMonsterSlayerQuest(name))
        {
          this.specificMonstersKilled[name]++;
          Game1.player.hasCompletedAllMonsterSlayerQuests.Value = AdventureGuild.areAllMonsterSlayerQuestsComplete();
          string str1 = name;
          string str2;
          if (Game1.content.Load<Dictionary<string, string>>("Data\\Monsters").TryGetValue(name, out str1))
          {
            string[] strArray = str1.Split('/');
            str2 = strArray.Length <= 14 ? name : strArray[14];
          }
          else
            str2 = name;
          Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Stats.cs.5129"));
          Game1.multiplayer.globalChatInfoMessage("MonsterSlayer" + Game1.random.Next(4).ToString(), Game1.player.Name, str2);
          if (!AdventureGuild.areAllMonsterSlayerQuestsComplete())
            return;
          Game1.getSteamAchievement("Achievement_KeeperOfTheMysticRings");
        }
        else
          this.specificMonstersKilled[name]++;
      }
      else
        this.specificMonstersKilled.Add(name, 1);
    }

    public int getMonstersKilled(string name) => this.specificMonstersKilled.ContainsKey(name) ? this.specificMonstersKilled[name] : 0;

    public uint GoodFriends
    {
      get => this.goodFriends;
      set => this.goodFriends = value;
    }

    public uint CropsShipped
    {
      get => this.cropsShipped;
      set => this.cropsShipped = value;
    }

    public uint ItemsForaged
    {
      get => this.itemsForaged;
      set => this.itemsForaged = value;
    }

    public uint GeodesCracked
    {
      get => this.geodesCracked;
      set => this.geodesCracked = value;
    }

    public uint SlimesKilled
    {
      get => this.slimesKilled;
      set => this.slimesKilled = value;
    }

    public uint StarLevelCropsShipped
    {
      get => this.starLevelCropsShipped;
      set
      {
        this.starLevelCropsShipped = value;
        this.checkForStarCropsAchievements();
      }
    }

    public uint StoneGathered
    {
      get => this.stoneGathered;
      set => this.stoneGathered = value;
    }

    public uint QuestsCompleted
    {
      get => this.questsCompleted;
      set
      {
        this.questsCompleted = value;
        this.checkForQuestAchievements();
      }
    }

    public uint FishCaught
    {
      get => this.fishCaught;
      set => this.fishCaught = value;
    }

    public uint NotesFound
    {
      get => this.notesFound;
      set => this.notesFound = value;
    }

    public uint SticksChopped
    {
      get => this.sticksChopped;
      set => this.sticksChopped = value;
    }

    public uint WeedsEliminated
    {
      get => this.weedsEliminated;
      set => this.weedsEliminated = value;
    }

    public uint DaysPlayed
    {
      get => this.daysPlayed;
      set => this.daysPlayed = value;
    }

    public uint BouldersCracked
    {
      get => this.bouldersCracked;
      set => this.bouldersCracked = value;
    }

    public uint MysticStonesCrushed
    {
      get => this.mysticStonesCrushed;
      set => this.mysticStonesCrushed = value;
    }

    public uint GoatCheeseMade
    {
      get => this.goatCheeseMade;
      set => this.goatCheeseMade = value;
    }

    public uint CheeseMade
    {
      get => this.cheeseMade;
      set => this.cheeseMade = value;
    }

    public uint PiecesOfTrashRecycled
    {
      get => this.piecesOfTrashRecycled;
      set => this.piecesOfTrashRecycled = value;
    }

    public uint PreservesMade
    {
      get => this.preservesMade;
      set => this.preservesMade = value;
    }

    public uint BeveragesMade
    {
      get => this.beveragesMade;
      set => this.beveragesMade = value;
    }

    public uint BarsSmelted
    {
      get => this.barsSmelted;
      set => this.barsSmelted = value;
    }

    public uint IridiumFound
    {
      get => this.iridiumFound;
      set => this.iridiumFound = value;
    }

    public uint GoldFound
    {
      get => this.goldFound;
      set => this.goldFound = value;
    }

    public uint CoinsFound
    {
      get => this.coinsFound;
      set => this.coinsFound = value;
    }

    public uint CoalFound
    {
      get => this.coalFound;
      set => this.coalFound = value;
    }

    public uint IronFound
    {
      get => this.ironFound;
      set => this.ironFound = value;
    }

    public uint CopperFound
    {
      get => this.copperFound;
      set => this.copperFound = value;
    }

    public uint CaveCarrotsFound
    {
      get => this.caveCarrotsFound;
      set => this.caveCarrotsFound = value;
    }

    public uint OtherPreciousGemsFound
    {
      get => this.otherPreciousGemsFound;
      set => this.otherPreciousGemsFound = value;
    }

    public uint PrismaticShardsFound
    {
      get => this.prismaticShardsFound;
      set => this.prismaticShardsFound = value;
    }

    public uint DiamondsFound
    {
      get => this.diamondsFound;
      set => this.diamondsFound = value;
    }

    public uint MonstersKilled
    {
      get => this.monstersKilled;
      set => this.monstersKilled = value;
    }

    public uint StepsTaken
    {
      get => this.stepsTaken;
      set => this.stepsTaken = value;
    }

    public uint StumpsChopped
    {
      get => this.stumpsChopped;
      set => this.stumpsChopped = value;
    }

    public uint TimesFished
    {
      get => this.timesFished;
      set => this.timesFished = value;
    }

    public uint AverageBedtime
    {
      get => this.averageBedtime;
      set => this.averageBedtime = (this.averageBedtime * (this.daysPlayed - 1U) + value) / Math.Max(1U, this.daysPlayed);
    }

    public uint TimesUnconscious
    {
      get => this.timesUnconscious;
      set => this.timesUnconscious = value;
    }

    public uint GiftsGiven
    {
      get => this.giftsGiven;
      set => this.giftsGiven = value;
    }

    public uint DirtHoed
    {
      get => this.dirtHoed;
      set => this.dirtHoed = value;
    }

    public uint RocksCrushed
    {
      get => this.rocksCrushed;
      set => this.rocksCrushed = value;
    }

    public uint TrufflesFound
    {
      get => this.trufflesFound;
      set => this.trufflesFound = value;
    }

    public uint SheepWoolProduced
    {
      get => this.sheepWoolProduced;
      set => this.sheepWoolProduced = value;
    }

    public uint RabbitWoolProduced
    {
      get => this.rabbitWoolProduced;
      set => this.rabbitWoolProduced = value;
    }

    public uint GoatMilkProduced
    {
      get => this.goatMilkProduced;
      set => this.goatMilkProduced = value;
    }

    public uint CowMilkProduced
    {
      get => this.cowMilkProduced;
      set => this.cowMilkProduced = value;
    }

    public uint DuckEggsLayed
    {
      get => this.duckEggsLayed;
      set => this.duckEggsLayed = value;
    }

    public uint ItemsCrafted
    {
      get => this.itemsCrafted;
      set
      {
        this.itemsCrafted = value;
        this.checkForCraftingAchievements();
      }
    }

    public uint ChickenEggsLayed
    {
      get => this.chickenEggsLayed;
      set => this.chickenEggsLayed = value;
    }

    public uint ItemsCooked
    {
      get => this.itemsCooked;
      set => this.itemsCooked = value;
    }

    public uint ItemsShipped
    {
      get => this.itemsShipped;
      set => this.itemsShipped = value;
    }

    public uint SeedsSown
    {
      get => this.seedsSown;
      set => this.seedsSown = value;
    }

    public uint IndividualMoneyEarned
    {
      get => this.individualMoneyEarned;
      set
      {
        uint individualMoneyEarned = this.individualMoneyEarned;
        this.individualMoneyEarned = value;
        if (individualMoneyEarned < 1000000U && this.individualMoneyEarned >= 1000000U)
          Game1.multiplayer.globalChatInfoMessage("SoloEarned1mil_" + ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? "Male" : "Female"), Game1.player.Name);
        else if (individualMoneyEarned < 100000U && this.individualMoneyEarned >= 100000U)
          Game1.multiplayer.globalChatInfoMessage("SoloEarned100k_" + ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? "Male" : "Female"), Game1.player.Name);
        else if (individualMoneyEarned < 10000U && this.individualMoneyEarned >= 10000U)
        {
          Game1.multiplayer.globalChatInfoMessage("SoloEarned10k_" + ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? "Male" : "Female"), Game1.player.Name);
        }
        else
        {
          if (individualMoneyEarned >= 1000U || this.individualMoneyEarned < 1000U)
            return;
          Game1.multiplayer.globalChatInfoMessage("SoloEarned1k_" + ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale ? "Male" : "Female"), Game1.player.Name);
        }
      }
    }

    public void onMoneyGifted(uint amount)
    {
      uint totalMoneyGifted = this.totalMoneyGifted;
      this.totalMoneyGifted += amount;
      if (totalMoneyGifted <= 1000000U && this.totalMoneyGifted > 1000000U)
        Game1.multiplayer.globalChatInfoMessage("Gifted1mil", Game1.player.Name);
      else if (totalMoneyGifted <= 100000U && this.totalMoneyGifted > 100000U)
        Game1.multiplayer.globalChatInfoMessage("Gifted100k", Game1.player.Name);
      else if (totalMoneyGifted <= 10000U && this.totalMoneyGifted > 10000U)
      {
        Game1.multiplayer.globalChatInfoMessage("Gifted10k", Game1.player.Name);
      }
      else
      {
        if (totalMoneyGifted > 1000U || this.totalMoneyGifted <= 1000U)
          return;
        Game1.multiplayer.globalChatInfoMessage("Gifted1k", Game1.player.Name);
      }
    }

    public void takeStep()
    {
      ++this.StepsTaken;
      if (this.StepsTaken == 10000U)
        Game1.multiplayer.globalChatInfoMessage("Walked10k", Game1.player.Name);
      else if (this.StepsTaken == 100000U)
        Game1.multiplayer.globalChatInfoMessage("Walked100k", Game1.player.Name);
      else if (this.StepsTaken == 1000000U)
      {
        Game1.multiplayer.globalChatInfoMessage("Walked1m", Game1.player.Name);
      }
      else
      {
        if (this.StepsTaken != 10000000U)
          return;
        Game1.multiplayer.globalChatInfoMessage("Walked10m", Game1.player.Name);
      }
    }

    public void checkForCookingAchievements()
    {
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
      int num1 = 0;
      int num2 = 0;
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        if (Game1.player.cookingRecipes.ContainsKey(keyValuePair.Key))
        {
          int int32 = Convert.ToInt32(keyValuePair.Value.Split('/')[2].Split(' ')[0]);
          if (Game1.player.recipesCooked.ContainsKey(int32))
          {
            num2 += Game1.player.recipesCooked[int32];
            ++num1;
          }
        }
      }
      this.itemsCooked = (uint) num2;
      if (num1 == dictionary.Count)
        Game1.getAchievement(17);
      if (num1 >= 25)
        Game1.getAchievement(16);
      if (num1 < 10)
        return;
      Game1.getAchievement(15);
    }

    public void checkForCraftingAchievements()
    {
      Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
      int num1 = 0;
      int num2 = 0;
      foreach (string key in dictionary.Keys)
      {
        if (!(key == "Wedding Ring") && Game1.player.craftingRecipes.ContainsKey(key))
        {
          num2 += Game1.player.craftingRecipes[key];
          if (Game1.player.craftingRecipes[key] > 0)
            ++num1;
        }
      }
      this.itemsCrafted = (uint) num2;
      if (num1 >= dictionary.Count - 1)
        Game1.getAchievement(22);
      if (num1 >= 30)
        Game1.getAchievement(21);
      if (num1 < 15)
        return;
      Game1.getAchievement(20);
    }

    public void checkForShippingAchievements()
    {
      if (this.farmerShipped(24, 15) && this.farmerShipped(188, 15) && this.farmerShipped(190, 15) && this.farmerShipped(192, 15) && this.farmerShipped(248, 15) && this.farmerShipped(250, 15) && this.farmerShipped(252, 15) && this.farmerShipped(254, 15) && this.farmerShipped(256, 15) && this.farmerShipped(258, 15) && this.farmerShipped(260, 15) && this.farmerShipped(262, 15) && this.farmerShipped(264, 15) && this.farmerShipped(266, 15) && this.farmerShipped(268, 15) && this.farmerShipped(270, 15) && this.farmerShipped(272, 15) && this.farmerShipped(274, 15) && this.farmerShipped(276, 15) && this.farmerShipped(278, 15) && this.farmerShipped(280, 15) && this.farmerShipped(282, 15) && this.farmerShipped(284, 15) && this.farmerShipped(300, 15) && this.farmerShipped(304, 15) && this.farmerShipped(398, 15) && this.farmerShipped(400, 15) && this.farmerShipped(433, 15))
        Game1.getAchievement(31);
      if (!this.farmerShipped(24, 300) && !this.farmerShipped(188, 300) && !this.farmerShipped(190, 300) && !this.farmerShipped(192, 300) && !this.farmerShipped(248, 300) && !this.farmerShipped(250, 300) && !this.farmerShipped(252, 300) && !this.farmerShipped(254, 300) && !this.farmerShipped(256, 300) && !this.farmerShipped(258, 300) && !this.farmerShipped(260, 300) && !this.farmerShipped(262, 300) && !this.farmerShipped(264, 300) && !this.farmerShipped(266, 300) && !this.farmerShipped(268, 300) && !this.farmerShipped(270, 300) && !this.farmerShipped(272, 300) && !this.farmerShipped(274, 300) && !this.farmerShipped(276, 300) && !this.farmerShipped(278, 300) && !this.farmerShipped(280, 300) && !this.farmerShipped(282, 300) && !this.farmerShipped(284, 300) && !this.farmerShipped(454, 300) && !this.farmerShipped(300, 300) && !this.farmerShipped(304, 300) && !(this.farmerShipped(398, 300) | this.farmerShipped(433, 300)) && !this.farmerShipped(400, 300) && !this.farmerShipped(591, 300) && !this.farmerShipped(593, 300) && !this.farmerShipped(595, 300) && !this.farmerShipped(597, 300))
        return;
      Game1.getAchievement(32);
    }

    public void checkForStarCropsAchievements()
    {
      if (this.StarLevelCropsShipped < 100U)
        return;
      Game1.getAchievement(77);
    }

    private bool farmerShipped(int index, int number) => Game1.player.basicShipped.ContainsKey(index) && Game1.player.basicShipped[index] >= number;

    public void checkForFishingAchievements()
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) Game1.objectInformation)
      {
        if (keyValuePair.Value.Split('/')[3].Contains("Fish") && (keyValuePair.Key < 167 || keyValuePair.Key > 172) && (keyValuePair.Key < 898 || keyValuePair.Key > 902))
        {
          ++num3;
          if (Game1.player.fishCaught.ContainsKey(keyValuePair.Key))
          {
            num1 += Game1.player.fishCaught[keyValuePair.Key][0];
            ++num2;
          }
        }
      }
      this.fishCaught = (uint) num1;
      if (num1 >= 100)
        Game1.getAchievement(27);
      if (num2 == num3)
      {
        Game1.getAchievement(26);
        if (!Game1.player.hasOrWillReceiveMail("CF_Fish"))
          Game1.addMailForTomorrow("CF_Fish");
      }
      if (num2 >= 24)
        Game1.getAchievement(25);
      if (num2 < 10)
        return;
      Game1.getAchievement(24);
    }

    public void checkForArchaeologyAchievements()
    {
      int num = Game1.netWorldState.Value.MuseumPieces.Count();
      if (num >= 95)
        Game1.getAchievement(5);
      if (num < 40)
        return;
      Game1.getAchievement(28);
    }

    public void checkForMoneyAchievements()
    {
      if (Game1.player.totalMoneyEarned >= 10000000U)
        Game1.getAchievement(4);
      if (Game1.player.totalMoneyEarned >= 1000000U)
        Game1.getAchievement(3);
      if (Game1.player.totalMoneyEarned >= 250000U)
        Game1.getAchievement(2);
      if (Game1.player.totalMoneyEarned >= 50000U)
        Game1.getAchievement(1);
      if (Game1.player.totalMoneyEarned < 15000U)
        return;
      Game1.getAchievement(0);
    }

    public void checkForBuildingUpgradeAchievements()
    {
      if (Game1.player.HouseUpgradeLevel == 2)
        Game1.getAchievement(19);
      if (Game1.player.HouseUpgradeLevel != 1)
        return;
      Game1.getAchievement(18);
    }

    public void checkForQuestAchievements()
    {
      if (this.QuestsCompleted >= 40U)
      {
        Game1.getAchievement(30);
        Game1.addMailForTomorrow("quest35");
      }
      if (this.QuestsCompleted < 10U)
        return;
      Game1.getAchievement(29);
      Game1.addMailForTomorrow("quest10");
    }

    public void checkForFriendshipAchievements()
    {
      uint num1 = 0;
      uint num2 = 0;
      uint num3 = 0;
      foreach (Friendship friendship in Game1.player.friendshipData.Values)
      {
        if (friendship.Points >= 2500)
          ++num3;
        if (friendship.Points >= 2000)
          ++num2;
        if (friendship.Points >= 1250)
          ++num1;
      }
      this.GoodFriends = num2;
      if (num1 >= 20U)
        Game1.getAchievement(13);
      if (num1 >= 10U)
        Game1.getAchievement(12);
      if (num1 >= 4U)
        Game1.getAchievement(11);
      if (num1 >= 1U)
        Game1.getAchievement(6);
      if (num3 >= 8U)
        Game1.getAchievement(9);
      if (num3 >= 1U)
        Game1.getAchievement(7);
      Dictionary<string, string> dictionary1 = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
      foreach (string key in dictionary1.Keys)
      {
        string[] strArray = dictionary1[key].Split('/')[3].Split(' ');
        if (strArray[0].Equals("f") && Game1.player.friendshipData.ContainsKey(strArray[1]) && Game1.player.friendshipData[strArray[1]].Points >= Convert.ToInt32(strArray[2]) * 250 && !Game1.player.cookingRecipes.ContainsKey(key) && !Game1.player.hasOrWillReceiveMail(strArray[1] + "Cooking"))
          Game1.addMailForTomorrow(strArray[1] + "Cooking");
      }
      Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
      foreach (string key in dictionary2.Keys)
      {
        string[] strArray = dictionary2[key].Split('/')[4].Split(' ');
        if (strArray[0].Equals("f") && Game1.player.friendshipData.ContainsKey(strArray[1]) && Game1.player.friendshipData[strArray[1]].Points >= Convert.ToInt32(strArray[2]) * 250 && !Game1.player.craftingRecipes.ContainsKey(key) && !Game1.player.hasOrWillReceiveMail(strArray[1] + "Crafting"))
          Game1.addMailForTomorrow(strArray[1] + "Crafting");
      }
    }

    public bool isSharedAchievement(int which)
    {
      switch (which)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 28:
          return true;
        default:
          return false;
      }
    }

    public void checkForAchievements()
    {
      this.checkForCookingAchievements();
      this.checkForCraftingAchievements();
      this.checkForShippingAchievements();
      this.checkForStarCropsAchievements();
      this.checkForFishingAchievements();
      this.checkForArchaeologyAchievements();
      this.checkForMoneyAchievements();
      this.checkForBuildingUpgradeAchievements();
      this.checkForQuestAchievements();
      this.checkForFriendshipAchievements();
      Game1.player.hasCompletedAllMonsterSlayerQuests.Value = AdventureGuild.areAllMonsterSlayerQuestsComplete();
    }
  }
}
