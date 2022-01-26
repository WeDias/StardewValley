// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Lexicon
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Linq;

namespace StardewValley.BellsAndWhistles
{
  public class Lexicon
  {
    /// <summary>
    /// 
    /// A noun to represent some kind of "bad" object. Kind of has connotations of it being disgusting or cheap. preface with "that" or "such"
    /// 
    /// </summary>
    /// <returns></returns>
    public static string getRandomNegativeItemSlanderNoun()
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      string[] strArray = Game1.content.LoadString("Strings\\Lexicon:RandomNegativeItemNoun").Split('#');
      return strArray[random.Next(strArray.Length)];
    }

    public static string getProperArticleForWord(string word)
    {
      if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
        return "";
      string properArticleForWord = "a";
      if (word != null && word.Length > 0)
      {
        switch (word.ToLower()[0])
        {
          case 'a':
            properArticleForWord += "n";
            break;
          case 'e':
            properArticleForWord += "n";
            break;
          case 'i':
            properArticleForWord += "n";
            break;
          case 'o':
            properArticleForWord += "n";
            break;
          case 'u':
            properArticleForWord += "n";
            break;
        }
      }
      return properArticleForWord;
    }

    public static string capitalize(string text)
    {
      switch (text)
      {
        case "":
        case null:
          return text;
        default:
          if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
          {
            int num = 0;
            for (int index = 0; index < text.Length; ++index)
            {
              if (char.IsLetter(text[index]))
              {
                num = index;
                break;
              }
            }
            return num == 0 ? text.First<char>().ToString().ToUpper() + text.Substring(1) : text.Substring(0, num) + text[num].ToString().ToUpper() + text.Substring(num + 1);
          }
          goto case "";
      }
    }

    public static string makePlural(string word, bool ignore = false)
    {
      if (ignore || LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en || word == null || word.EndsWith(" Seeds") || word.EndsWith(" Shorts") || word.EndsWith(" Bass") || word.EndsWith(" Flowers"))
        return word;
      switch (word)
      {
        case "Algae Soup":
          return "bowls of Algae Soup";
        case "Bream":
        case "Broken Glasses":
        case "Carp":
        case "Chub":
        case "Clay":
        case "Crab Cakes":
        case "Cranberries":
        case "Dried Sunflowers":
        case "Driftwood":
        case "Fossilized Ribs":
        case "Ghostfish":
        case "Glass Shards":
        case "Glazed Yams":
        case "Green Canes":
        case "Hashbrowns":
        case "Hops":
        case "Largemouth Bass":
        case "Mixed Seeds":
        case "Pancakes":
        case "Pepper Poppers":
        case "Pickles":
        case "Red Canes":
        case "Roasted Hazelnuts":
        case "Sandfish":
        case "Smallmouth Bass":
        case "Star Shards":
        case "Tea Leaves":
        case "Weeds":
          return word;
        case "Coal":
          return "lumps of Coal";
        case "Dragon Tooth":
          return "Dragon Teeth";
        case "Garlic":
          return "bulbs of Garlic";
        case "Ginger":
          return "pieces of Ginger";
        case "Jelly":
          return "Jellies";
        case "Rice Pudding":
          return "bowls of Rice Pudding";
        case "Salt":
          return "pieces of Salt";
        case "Wheat":
          return "bushels of Wheat";
        default:
          if (word.Last<char>() == 'y')
            return word.Substring(0, word.Length - 1) + "ies";
          return word.Last<char>() == 's' || word.Last<char>() == 'z' || word.Last<char>() == 'x' || word.Length > 2 && word.Substring(word.Length - 2) == "sh" || word.Length > 2 && word.Substring(word.Length - 2) == "ch" ? word + "es" : word + "s";
      }
    }

    public static string prependArticle(string word) => LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en ? word : Lexicon.getProperArticleForWord(word) + " " + word;

    /// <summary>
    /// 
    /// Adjectives like "wonderful" "amazing" "excellent", prefaced with "are"  "is"  "was" "will be" "usually is", etc.
    /// these wouldn't really make sense for an object, more for a person,place, or event
    /// </summary>
    /// <returns></returns>
    public static string getRandomPositiveAdjectiveForEventOrPerson(NPC n = null)
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      string[] strArray = n == null || n.Age == 0 ? (n == null || n.Gender != 0 ? (n == null || n.Gender != 1 ? Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_PlaceOrEvent").Split('#') : Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_AdultFemale").Split('#')) : Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_AdultMale").Split('#')) : Game1.content.LoadString("Strings\\Lexicon:RandomPositiveAdjective_Child").Split('#');
      return strArray[random.Next(strArray.Length)];
    }

    /// <summary>
    /// 
    /// Adjectives like "horrible" "distasteful" "boring", prefaced with "are"  "is"  "was" "will be" "usuall is", etc.
    /// these wouldn't really make sense for an object, more for a person,place, or event
    /// </summary>
    /// <returns></returns>
    public static string getRandomNegativeAdjectiveForEventOrPerson(NPC n = null)
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      string[] strArray = n == null || n.Age == 0 ? (n == null || n.Gender != 0 ? (n == null || n.Gender != 1 ? Game1.content.LoadString("Strings\\Lexicon:RandomNegativeAdjective_PlaceOrEvent").Split('#') : Game1.content.LoadString("Strings\\Lexicon:RandomNegativeAdjective_AdultFemale").Split('#')) : Game1.content.LoadString("Strings\\Lexicon:RandomNegativeAdjective_AdultMale").Split('#')) : Game1.content.LoadString("Strings\\Lexicon:RandomNegativeAdjective_Child").Split('#');
      return strArray[random.Next(strArray.Length)];
    }

    /// <summary>
    /// 
    /// An adjective to represent something tasty, like "delicious", "tasty", "wonderful", "satisfying"
    /// 
    /// </summary>
    /// <returns></returns>
    public static string getRandomDeliciousAdjective(NPC n = null)
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      string[] strArray = n == null || n.Age != 2 ? Game1.content.LoadString("Strings\\Lexicon:RandomDeliciousAdjective").Split('#') : Game1.content.LoadString("Strings\\Lexicon:RandomDeliciousAdjective_Child").Split('#');
      return strArray[random.Next(strArray.Length)];
    }

    /// <summary>
    /// 
    /// Adjective to describe something that is not tasty. "gross", "disgusting", "nasty"
    /// </summary>
    /// <returns></returns>
    public static string getRandomNegativeFoodAdjective(NPC n = null)
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      string[] strArray = n == null || n.Age != 2 ? (n == null || n.Manners != 1 ? Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective").Split('#') : Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective_Polite").Split('#')) : Game1.content.LoadString("Strings\\Lexicon:RandomNegativeFoodAdjective_Child").Split('#');
      return strArray[random.Next(strArray.Length)];
    }

    /// <summary>Adjectives like "decent" "good"</summary>
    /// <returns></returns>
    public static string getRandomSlightlyPositiveAdjectiveForEdibleNoun(NPC n = null)
    {
      Random random = new Random((int) Game1.stats.DaysPlayed + (int) Game1.uniqueIDForThisGame / 2);
      string[] strArray = Game1.content.LoadString("Strings\\Lexicon:RandomSlightlyPositiveFoodAdjective").Split('#');
      return strArray[random.Next(strArray.Length)];
    }

    /// <summary>
    /// Returns a generic term for a child of a given gender, i.e. "boy" or "girl".
    /// </summary>
    public static string getGenderedChildTerm(bool isMale) => isMale ? Game1.content.LoadString("Strings\\Lexicon:ChildTerm_Male") : Game1.content.LoadString("Strings\\Lexicon:ChildTerm_Female");

    /// <summary>Returns a gendered pronoun (i.e. "him" or "her")</summary>
    public static string getPronoun(bool isMale) => isMale ? Game1.content.LoadString("Strings\\Lexicon:Pronoun_Male") : Game1.content.LoadString("Strings\\Lexicon:Pronoun_Female");

    /// <summary>
    /// Returns a possessive gendered pronoun (i.e. "his" or "her")
    /// </summary>
    public static string getPossessivePronoun(bool isMale) => isMale ? Game1.content.LoadString("Strings\\Lexicon:Possessive_Pronoun_Male") : Game1.content.LoadString("Strings\\Lexicon:Possessive_Pronoun_Female");
  }
}
