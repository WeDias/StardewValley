// Decompiled with JetBrains decompiler
// Type: StardewValley.Dialogue
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley
{
  public class Dialogue
  {
    public const string dialogueHappy = "$h";
    public const string dialogueSad = "$s";
    public const string dialogueUnique = "$u";
    public const string dialogueNeutral = "$neutral";
    public const string dialogueLove = "$l";
    public const string dialogueAngry = "$a";
    public const string dialogueEnd = "$e";
    public const string dialogueBreak = "$b";
    public const string dialogueKill = "$k";
    public const string dialogueChance = "$c";
    public const string dialogueDependingOnWorldState = "$d";
    public const string dialogueQuickResponse = "$y";
    public const string dialoguePrerequisite = "$p";
    public const string dialogueSingle = "$1";
    public const string dialogueQuestion = "$q";
    public const string dialogueResponse = "$r";
    public const string breakSpecialCharacter = "{";
    public const string playerNameSpecialCharacter = "@";
    public const string genderDialogueSplitCharacter = "^";
    public const string genderDialogueSplitCharacter2 = "¦";
    public const string quickResponseDelineator = "*";
    public const string randomAdjectiveSpecialCharacter = "%adj";
    public const string randomNounSpecialCharacter = "%noun";
    public const string randomPlaceSpecialCharacter = "%place";
    public const string spouseSpecialCharacter = "%spouse";
    public const string randomNameSpecialCharacter = "%name";
    public const string firstNameLettersSpecialCharacter = "%firstnameletter";
    public const string timeSpecialCharacter = "%time";
    public const string bandNameSpecialCharacter = "%band";
    public const string bookNameSpecialCharacter = "%book";
    public const string rivalSpecialCharacter = "%rival";
    public const string petSpecialCharacter = "%pet";
    public const string farmNameSpecialCharacter = "%farm";
    public const string favoriteThingSpecialCharacter = "%favorite";
    public const string eventForkSpecialCharacter = "%fork";
    public const string yearSpecialCharacter = "%year";
    public const string kid1specialCharacter = "%kid1";
    public const string kid2SpecialCharacter = "%kid2";
    public const string revealTasteCharacter = "%revealtaste";
    private static bool nameArraysTranslated = false;
    public static string[] adjectives = new string[20]
    {
      "Purple",
      "Gooey",
      "Chalky",
      "Green",
      "Plush",
      "Chunky",
      "Gigantic",
      "Greasy",
      "Gloomy",
      "Practical",
      "Lanky",
      "Dopey",
      "Crusty",
      "Fantastic",
      "Rubbery",
      "Silly",
      "Courageous",
      "Reasonable",
      "Lonely",
      "Bitter"
    };
    public static string[] nouns = new string[23]
    {
      "Dragon",
      "Buffet",
      "Biscuit",
      "Robot",
      "Planet",
      "Pepper",
      "Tomb",
      "Hyena",
      "Lip",
      "Quail",
      "Cheese",
      "Disaster",
      "Raincoat",
      "Shoe",
      "Castle",
      "Elf",
      "Pump",
      "Chip",
      "Wig",
      "Mermaid",
      "Drumstick",
      "Puppet",
      "Submarine"
    };
    public static string[] verbs = new string[13]
    {
      "ran",
      "danced",
      "spoke",
      "galloped",
      "ate",
      "floated",
      "stood",
      "flowed",
      "smelled",
      "swam",
      "grilled",
      "cracked",
      "melted"
    };
    public static string[] positional = new string[13]
    {
      "atop",
      "near",
      "with",
      "alongside",
      "away from",
      "too close to",
      "dangerously close to",
      "far, far away from",
      "uncomfortably close to",
      "way above the",
      "miles below",
      "on a different planet from",
      "in a different century than"
    };
    public static string[] places = new string[12]
    {
      "Castle Village",
      "Basket Town",
      "Pine Mesa City",
      "Point Drake",
      "Minister Valley",
      "Grampleton",
      "Zuzu City",
      "a small island off the coast",
      "Fort Josa",
      "Chestervale",
      "Fern Islands",
      "Tanker Grove"
    };
    public static string[] colors = new string[16]
    {
      "/crimson",
      "/green",
      "/tan",
      "/purple",
      "/deep blue",
      "/neon pink",
      "/pale/yellow",
      "/chocolate/brown",
      "/sky/blue",
      "/bubblegum/pink",
      "/blood/red",
      "/bright/orange",
      "/aquamarine",
      "/silvery",
      "/glimmering/gold",
      "/rainbow"
    };
    public List<string> dialogues = new List<string>();
    private List<NPCDialogueResponse> playerResponses;
    private List<string> quickResponses;
    private bool isLastDialogueInteractive;
    private bool quickResponse;
    public bool isCurrentStringContinuedOnNextScreen;
    private bool dialogueToBeKilled;
    private bool finishedLastDialogue;
    public bool showPortrait;
    public bool removeOnNextMove;
    public string temporaryDialogueKey;
    public int currentDialogueIndex;
    private string currentEmotion;
    public string temporaryDialogue;
    public NPC speaker;
    public Dialogue.onAnswerQuestion answerQuestionBehavior;
    public Texture2D overridePortrait;
    public Action onFinish;

    private static void TranslateArraysOfStrings()
    {
      Dialogue.colors = new string[16]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.795"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.796"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.797"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.798"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.799"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.800"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.801"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.802"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.803"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.804"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.805"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.806"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.807"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.808"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.809"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.810")
      };
      Dialogue.adjectives = new string[20]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.679"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.680"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.681"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.682"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.683"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.684"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.685"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.686"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.687"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.688"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.689"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.690"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.691"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.692"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.693"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.694"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.695"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.696"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.697"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.698")
      };
      Dialogue.nouns = new string[23]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.699"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.700"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.701"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.702"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.703"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.704"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.705"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.706"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.707"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.708"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.709"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.710"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.711"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.712"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.713"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.714"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.715"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.716"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.717"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.718"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.719"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.720"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.721")
      };
      Dialogue.verbs = new string[13]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.722"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.723"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.724"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.725"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.726"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.727"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.728"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.729"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.730"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.731"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.732"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.733"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.734")
      };
      Dialogue.positional = new string[13]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.735"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.736"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.737"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.738"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.739"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.740"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.741"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.742"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.743"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.744"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.745"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.746"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.747")
      };
      Dialogue.places = new string[12]
      {
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.748"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.749"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.750"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.751"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.752"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.753"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.754"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.755"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.756"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.757"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.758"),
        Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.759")
      };
      Dialogue.nameArraysTranslated = true;
    }

    public string CurrentEmotion
    {
      get => this.currentEmotion;
      set => this.currentEmotion = value;
    }

    public Farmer farmer => Game1.CurrentEvent != null ? Game1.CurrentEvent.farmer : Game1.player;

    public Dialogue(string masterDialogue, NPC speaker)
    {
      if (!Dialogue.nameArraysTranslated)
        Dialogue.TranslateArraysOfStrings();
      this.speaker = speaker;
      this.parseDialogueString(masterDialogue);
      this.checkForSpecialDialogueAttributes();
    }

    public void setCurrentDialogue(string dialogue)
    {
      this.dialogues.Clear();
      this.currentDialogueIndex = 0;
      this.parseDialogueString(dialogue);
    }

    public void addMessageToFront(string dialogue)
    {
      this.currentDialogueIndex = 0;
      List<string> collection = new List<string>();
      collection.AddRange((IEnumerable<string>) this.dialogues);
      this.dialogues.Clear();
      this.parseDialogueString(dialogue);
      this.dialogues.AddRange((IEnumerable<string>) collection);
      this.checkForSpecialDialogueAttributes();
    }

    public static string getRandomVerb()
    {
      if (!Dialogue.nameArraysTranslated)
        Dialogue.TranslateArraysOfStrings();
      return Dialogue.verbs[Game1.random.Next(Dialogue.verbs.Length)];
    }

    public static string getRandomAdjective()
    {
      if (!Dialogue.nameArraysTranslated)
        Dialogue.TranslateArraysOfStrings();
      return Dialogue.adjectives[Game1.random.Next(Dialogue.adjectives.Length)];
    }

    public static string getRandomNoun()
    {
      if (!Dialogue.nameArraysTranslated)
        Dialogue.TranslateArraysOfStrings();
      return Dialogue.nouns[Game1.random.Next(Dialogue.nouns.Length)];
    }

    public static string getRandomPositional()
    {
      if (!Dialogue.nameArraysTranslated)
        Dialogue.TranslateArraysOfStrings();
      return Dialogue.positional[Game1.random.Next(Dialogue.positional.Length)];
    }

    public int getPortraitIndex()
    {
      string currentEmotion = this.currentEmotion;
      if (currentEmotion == "$neutral")
        return 0;
      if (currentEmotion == "$h")
        return 1;
      if (currentEmotion == "$s")
        return 2;
      if (currentEmotion == "$u")
        return 3;
      if (currentEmotion == "$l")
        return 4;
      if (currentEmotion == "$a")
        return 5;
      return int.TryParse(this.currentEmotion.Substring(1), out int _) ? Convert.ToInt32(this.currentEmotion.Substring(1)) : 0;
    }

    private void parseDialogueString(string masterString)
    {
      if (masterString == null)
        masterString = "...";
      this.temporaryDialogue = (string) null;
      if (this.playerResponses != null)
        this.playerResponses.Clear();
      string[] source = masterString.Split('#');
      for (int count = 0; count < source.Length; ++count)
      {
        if (source[count].Length >= 2)
        {
          source[count] = this.checkForSpecialCharacters(source[count]);
          string str1;
          try
          {
            str1 = source[count].Substring(0, 2);
          }
          catch (Exception ex)
          {
            str1 = "     ";
          }
          if (!str1.Equals("$e"))
          {
            if (str1.Equals("$b"))
            {
              if (this.dialogues.Count > 0)
                this.dialogues[this.dialogues.Count - 1] += "{";
            }
            else if (str1.Equals("$k"))
              this.dialogueToBeKilled = true;
            else if (str1.Equals("$1") && source[count].Split(' ').Length > 1)
            {
              string str2 = source[count].Split(' ')[1];
              if (this.farmer.mailReceived.Contains(str2))
              {
                count += 3;
                if (count < source.Length)
                {
                  source[count] = this.checkForSpecialCharacters(source[count]);
                  this.dialogues.Add(source[count]);
                }
              }
              else
              {
                source[count + 1] = this.checkForSpecialCharacters(source[count + 1]);
                this.dialogues.Add(str2 + "}" + source[count + 1]);
                break;
              }
            }
            else if (str1.Equals("$c") && source[count].Split(' ').Length > 1)
            {
              double num = Convert.ToDouble(source[count].Split(' ')[1]);
              if (Game1.random.NextDouble() > num)
              {
                ++count;
              }
              else
              {
                this.dialogues.Add(source[count + 1]);
                count += 3;
              }
            }
            else if (str1.Equals("$q"))
            {
              if (this.dialogues.Count > 0)
                this.dialogues[this.dialogues.Count - 1] += "{";
              string[] strArray1 = source[count].Split(' ');
              string[] strArray2 = strArray1[1].Split('/');
              bool flag = false;
              for (int index = 0; index < strArray2.Length; ++index)
              {
                if (this.farmer.DialogueQuestionsAnswered.Contains(Convert.ToInt32(strArray2[index])))
                {
                  flag = true;
                  break;
                }
              }
              if (flag && Convert.ToInt32(strArray2[0]) != -1)
              {
                if (!strArray1[2].Equals("null"))
                {
                  source = ((IEnumerable<string>) ((IEnumerable<string>) source).Take<string>(count).ToArray<string>()).Concat<string>((IEnumerable<string>) this.speaker.Dialogue[strArray1[2]].Split('#')).ToArray<string>();
                  --count;
                }
              }
              else
                this.isLastDialogueInteractive = true;
            }
            else if (str1.Equals("$r"))
            {
              string[] strArray = source[count].Split(' ');
              if (this.playerResponses == null)
                this.playerResponses = new List<NPCDialogueResponse>();
              this.isLastDialogueInteractive = true;
              this.playerResponses.Add(new NPCDialogueResponse(Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), strArray[3], source[count + 1]));
              ++count;
            }
            else if (str1.Equals("$p"))
            {
              string[] strArray3 = source[count].Split(' ');
              string[] strArray4 = source[count + 1].Split('|');
              bool flag = false;
              for (int index = 1; index < strArray3.Length; ++index)
              {
                if (this.farmer.DialogueQuestionsAnswered.Contains(Convert.ToInt32(strArray3[1])))
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
              {
                source = strArray4[0].Split('#');
                count = -1;
              }
              else
                source[count + 1] = ((IEnumerable<string>) source[count + 1].Split('|')).Last<string>();
            }
            else if (str1.Equals("$d"))
            {
              string[] strArray = source[count].Split(' ');
              string str3 = masterString.Substring(masterString.IndexOf('#') + 1);
              bool flag = false;
              string lower = strArray[1].ToLower();
              if (!(lower == "joja"))
              {
                if (!(lower == "cc") && !(lower == "communitycenter"))
                {
                  if (!(lower == "bus"))
                  {
                    if (lower == "kent")
                      flag = Game1.year >= 2;
                  }
                  else
                    flag = this.farmer.mailReceived.Contains("ccVault");
                }
                else
                  flag = Game1.isLocationAccessible("CommunityCenter");
              }
              else
                flag = Game1.isLocationAccessible("JojaMart");
              char separator = str3.Contains('|') ? '|' : '#';
              source = !flag ? str3.Split(separator)[1].Split('#') : str3.Split(separator)[0].Split('#');
              --count;
            }
            else if (str1.Equals("$y"))
            {
              this.quickResponse = true;
              this.isLastDialogueInteractive = true;
              if (this.quickResponses == null)
                this.quickResponses = new List<string>();
              if (this.playerResponses == null)
                this.playerResponses = new List<NPCDialogueResponse>();
              string str4 = source[count].Substring(source[count].IndexOf('\'') + 1);
              string[] strArray = str4.Substring(0, str4.Length - 1).Split('_');
              this.dialogues.Add(strArray[0]);
              for (int index = 1; index < strArray.Length; index += 2)
              {
                this.playerResponses.Add(new NPCDialogueResponse(-1, -1, "quickResponse" + index.ToString(), Game1.parseText(strArray[index])));
                this.quickResponses.Add(strArray[index + 1].Replace("*", "#$b#"));
              }
            }
            else if (source[count].Contains("^"))
            {
              if (this.farmer.IsMale)
                this.dialogues.Add(source[count].Substring(0, source[count].IndexOf("^")));
              else
                this.dialogues.Add(source[count].Substring(source[count].IndexOf("^") + 1));
            }
            else if (source[count].Contains("¦"))
            {
              if (this.farmer.IsMale)
                this.dialogues.Add(source[count].Substring(0, source[count].IndexOf("¦")));
              else
                this.dialogues.Add(source[count].Substring(source[count].IndexOf("¦") + 1));
            }
            else
              this.dialogues.Add(source[count]);
          }
        }
      }
    }

    public void prepareDialogueForDisplay()
    {
      if (this.dialogues.Count <= 0)
        return;
      if (this.speaker != null && (bool) (NetFieldBase<bool, NetBool>) this.speaker.shouldWearIslandAttire && Game1.player.friendshipData.ContainsKey(this.speaker.Name) && Game1.player.friendshipData[this.speaker.Name].IsDivorced() && this.currentEmotion == "$u")
        this.currentEmotion = "$neutral";
      this.dialogues[this.currentDialogueIndex] = Utility.ParseGiftReveals(this.dialogues[this.currentDialogueIndex]);
    }

    public string getCurrentDialogue()
    {
      if (this.currentDialogueIndex >= this.dialogues.Count || this.finishedLastDialogue)
        return "";
      this.showPortrait = true;
      if (this.speaker.Name.Equals("Dwarf") && !this.farmer.canUnderstandDwarves)
        return Dialogue.convertToDwarvish(this.dialogues[this.currentDialogueIndex]);
      if (this.temporaryDialogue != null)
        return this.temporaryDialogue;
      if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("}"))
      {
        this.farmer.mailReceived.Add(this.dialogues[this.currentDialogueIndex].Split('}')[0]);
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Substring(this.dialogues[this.currentDialogueIndex].IndexOf("}") + 1);
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("$k", "");
      }
      if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains('[') && this.dialogues[this.currentDialogueIndex].IndexOf(']') > 0)
      {
        string str = this.dialogues[this.currentDialogueIndex].Substring(this.dialogues[this.currentDialogueIndex].IndexOf('[') + 1, this.dialogues[this.currentDialogueIndex].IndexOf(']') - this.dialogues[this.currentDialogueIndex].IndexOf('[') - 1);
        string[] strArray = str.Split(' ');
        int result = -1;
        if (int.TryParse(strArray[Game1.random.Next(strArray.Length)], out result) && Game1.objectInformation.ContainsKey(result))
        {
          this.farmer.addItemToInventoryBool((Item) new Object(Vector2.Zero, result, (string) null, false, true, false, false), true);
          this.farmer.showCarrying();
        }
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("[" + str + "]", "");
      }
      if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("$k"))
      {
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("$k", "");
        this.dialogues.RemoveRange(this.currentDialogueIndex + 1, this.dialogues.Count - 1 - this.currentDialogueIndex);
        this.finishedLastDialogue = true;
      }
      if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Length > 1 && this.dialogues[this.currentDialogueIndex][0] == '%')
      {
        this.showPortrait = false;
        return this.dialogues[this.currentDialogueIndex].Substring(1);
      }
      return this.dialogues.Count <= 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.792") : this.dialogues[this.currentDialogueIndex].Replace("%time", Game1.getTimeOfDayString(Game1.timeOfDay));
    }

    public bool isItemGrabDialogue() => this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains('[');

    public bool isOnFinalDialogue() => this.currentDialogueIndex == this.dialogues.Count - 1;

    public bool isDialogueFinished() => this.finishedLastDialogue;

    public string checkForSpecialCharacters(string str)
    {
      string newValue1 = Utility.FilterUserName(this.farmer.Name);
      str = str.Replace("@", newValue1);
      str = str.Replace("%adj", Dialogue.adjectives[Game1.random.Next(Dialogue.adjectives.Length)].ToLower());
      if (str.Contains("%noun"))
        str = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de ? str.Substring(0, str.IndexOf("%noun") + "%noun".Length).Replace("%noun", Dialogue.nouns[Game1.random.Next(Dialogue.nouns.Length)]) + str.Substring(str.IndexOf("%noun") + "%noun".Length).Replace("%noun", Dialogue.nouns[Game1.random.Next(Dialogue.nouns.Length)]) : str.Substring(0, str.IndexOf("%noun") + "%noun".Length).Replace("%noun", Dialogue.nouns[Game1.random.Next(Dialogue.nouns.Length)].ToLower()) + str.Substring(str.IndexOf("%noun") + "%noun".Length).Replace("%noun", Dialogue.nouns[Game1.random.Next(Dialogue.nouns.Length)].ToLower());
      str = str.Replace("%place", Dialogue.places[Game1.random.Next(Dialogue.places.Length)]);
      str = str.Replace("%name", Dialogue.randomName());
      str = str.Replace("%firstnameletter", newValue1.Substring(0, Math.Max(0, newValue1.Length / 2)));
      str = str.Replace("%band", Game1.samBandName);
      if (str.Contains("%book"))
        str = str.Replace("%book", Game1.elliottBookName);
      if (!string.IsNullOrEmpty(str) && str.Contains("%spouse"))
      {
        if (this.farmer.spouse != null)
        {
          string[] strArray = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions")[this.farmer.spouse].Split('/');
          str = str.Replace("%spouse", strArray[strArray.Length - 1]);
        }
        else if (this.farmer.team.GetSpouse(this.farmer.UniqueMultiplayerID).HasValue)
        {
          Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(this.farmer.team.GetSpouse(this.farmer.UniqueMultiplayerID).Value);
          str = str.Replace("%spouse", farmerMaybeOffline.Name);
        }
      }
      string newValue2 = Utility.FilterUserName((string) (NetFieldBase<string, NetString>) this.farmer.farmName);
      str = str.Replace("%farm", newValue2);
      string newValue3 = Utility.FilterUserName((string) (NetFieldBase<string, NetString>) this.farmer.favoriteThing);
      str = str.Replace("%favorite", newValue3);
      str = str.Replace("%year", Game1.year.ToString() ?? "");
      int numberOfChildren = this.farmer.getNumberOfChildren();
      str = str.Replace("%kid1", numberOfChildren > 0 ? this.farmer.getChildren()[0].displayName : Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.793"));
      str = str.Replace("%kid2", numberOfChildren > 1 ? this.farmer.getChildren()[1].displayName : Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.794"));
      str = str.Replace("%pet", this.farmer.getPetDisplayName());
      if (str.Contains("¦"))
        str = this.farmer.IsMale ? str.Substring(0, str.IndexOf("¦")) : str.Substring(str.IndexOf("¦") + 1);
      if (str.Contains("%fork"))
      {
        str = str.Replace("%fork", "");
        if (Game1.currentLocation.currentEvent != null)
          Game1.currentLocation.currentEvent.specialEventVariable1 = true;
      }
      str = str.Replace("%rival", Utility.getOtherFarmerNames()[0].Split(' ')[1]);
      return str;
    }

    public static string randomName()
    {
      string str1 = "";
      string source1;
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.ja:
          string[] strArray1 = new string[38]
          {
            "ローゼン",
            "ミルド",
            "ココ",
            "ナミ",
            "こころ",
            "サルコ",
            "ハンゾー",
            "クッキー",
            "ココナツ",
            "せん",
            "ハル",
            "ラン",
            "オサム",
            "ヨシ",
            "ソラ",
            "ホシ",
            "まこと",
            "マサ",
            "ナナ",
            "リオ",
            "リン",
            "フジ",
            "うどん",
            "ミント",
            "さくら",
            "ボンボン",
            "レオ",
            "モリ",
            "コーヒー",
            "ミルク",
            "マロン",
            "クルミ",
            "サムライ",
            "カミ",
            "ゴロ",
            "マル",
            "チビ",
            "ユキダマ"
          };
          source1 = strArray1[new Random().Next(0, strArray1.Length)];
          break;
        case LocalizedContentManager.LanguageCode.ru:
          string[] strArray2 = new string[50]
          {
            "Августина",
            "Альф",
            "Анфиса",
            "Ариша",
            "Афоня",
            "Баламут",
            "Балкан",
            "Бандит",
            "Бланка",
            "Бобик",
            "Боня",
            "Борька",
            "Буренка",
            "Бусинка",
            "Вася",
            "Гаврюша",
            "Глаша",
            "Гоша",
            "Дуня",
            "Дуся",
            "Зорька",
            "Ивонна",
            "Игнат",
            "Кеша",
            "Клара",
            "Кузя",
            "Лада",
            "Максимус",
            "Маня",
            "Марта",
            "Маруся",
            "Моня",
            "Мотя",
            "Мурзик",
            "Мурка",
            "Нафаня",
            "Ника",
            "Нюша",
            "Проша",
            "Пятнушка",
            "Сеня",
            "Сивка",
            "Тихон",
            "Тоша",
            "Фунтик",
            "Шайтан",
            "Юнона",
            "Юпитер",
            "Ягодка",
            "Яшка"
          };
          source1 = strArray2[new Random().Next(0, strArray2.Length)];
          break;
        case LocalizedContentManager.LanguageCode.zh:
          string[] strArray3 = new string[183]
          {
            "雨果",
            "蛋挞",
            "小百合",
            "毛毛",
            "小雨",
            "小溪",
            "精灵",
            "安琪儿",
            "小糕",
            "玫瑰",
            "小黄",
            "晓雨",
            "阿江",
            "铃铛",
            "马琪",
            "果粒",
            "郁金香",
            "小黑",
            "雨露",
            "小江",
            "灵力",
            "萝拉",
            "豆豆",
            "小莲",
            "斑点",
            "小雾",
            "阿川",
            "丽丹",
            "玛雅",
            "阿豆",
            "花花",
            "琉璃",
            "滴答",
            "阿山",
            "丹麦",
            "梅西",
            "橙子",
            "花儿",
            "晓璃",
            "小夕",
            "山大",
            "咪咪",
            "卡米",
            "红豆",
            "花朵",
            "洋洋",
            "太阳",
            "小岩",
            "汪汪",
            "玛利亚",
            "小菜",
            "花瓣",
            "阳阳",
            "小夏",
            "石头",
            "阿狗",
            "邱洁",
            "苹果",
            "梨花",
            "小希",
            "天天",
            "浪子",
            "阿猫",
            "艾薇儿",
            "雪梨",
            "桃花",
            "阿喜",
            "云朵",
            "风儿",
            "狮子",
            "绮丽",
            "雪莉",
            "樱花",
            "小喜",
            "朵朵",
            "田田",
            "小红",
            "宝娜",
            "梅子",
            "小樱",
            "嘻嘻",
            "云儿",
            "小草",
            "小黄",
            "纳香",
            "阿梅",
            "茶花",
            "哈哈",
            "芸儿",
            "东东",
            "小羽",
            "哈豆",
            "桃子",
            "茶叶",
            "双双",
            "沫沫",
            "楠楠",
            "小爱",
            "麦当娜",
            "杏仁",
            "椰子",
            "小王",
            "泡泡",
            "小林",
            "小灰",
            "马格",
            "鱼蛋",
            "小叶",
            "小李",
            "晨晨",
            "小琳",
            "小慧",
            "布鲁",
            "晓梅",
            "绿叶",
            "甜豆",
            "小雪",
            "晓林",
            "康康",
            "安妮",
            "樱桃",
            "香板",
            "甜甜",
            "雪花",
            "虹儿",
            "美美",
            "葡萄",
            "薇儿",
            "金豆",
            "雪玲",
            "瑶瑶",
            "龙眼",
            "丁香",
            "晓云",
            "雪豆",
            "琪琪",
            "麦子",
            "糖果",
            "雪丽",
            "小艺",
            "小麦",
            "小圆",
            "雨佳",
            "小火",
            "麦茶",
            "圆圆",
            "春儿",
            "火灵",
            "板子",
            "黑点",
            "冬冬",
            "火花",
            "米粒",
            "喇叭",
            "晓秋",
            "跟屁虫",
            "米果",
            "欢欢",
            "爱心",
            "松子",
            "丫头",
            "双子",
            "豆芽",
            "小子",
            "彤彤",
            "棉花糖",
            "阿贵",
            "仙儿",
            "冰淇淋",
            "小彬",
            "贤儿",
            "冰棒",
            "仔仔",
            "格子",
            "水果",
            "悠悠",
            "莹莹",
            "巧克力",
            "梦洁",
            "汤圆",
            "静香",
            "茄子",
            "珍珠"
          };
          source1 = strArray3[new Random().Next(0, strArray3.Length)];
          break;
        default:
          int num = Game1.random.Next(3, 6);
          string[] strArray4 = new string[24]
          {
            "B",
            "Br",
            "J",
            "F",
            "S",
            "M",
            "C",
            "Ch",
            "L",
            "P",
            "K",
            "W",
            "G",
            "Z",
            "Tr",
            "T",
            "Gr",
            "Fr",
            "Pr",
            "N",
            "Sn",
            "R",
            "Sh",
            "St"
          };
          string[] strArray5 = new string[12]
          {
            "ll",
            "tch",
            "l",
            "m",
            "n",
            "p",
            "r",
            "s",
            "t",
            "c",
            "rt",
            "ts"
          };
          string[] source2 = new string[5]
          {
            "a",
            "e",
            "i",
            "o",
            "u"
          };
          string[] strArray6 = new string[5]
          {
            "ie",
            "o",
            "a",
            "ers",
            "ley"
          };
          Dictionary<string, string[]> dictionary1 = new Dictionary<string, string[]>();
          Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
          dictionary1.Add("a", new string[6]
          {
            "nie",
            "bell",
            "bo",
            "boo",
            "bella",
            "s"
          });
          dictionary1.Add("e", new string[4]
          {
            "ll",
            "llo",
            "",
            "o"
          });
          dictionary1.Add("i", new string[18]
          {
            "ck",
            "e",
            "bo",
            "ba",
            "lo",
            "la",
            "to",
            "ta",
            "no",
            "na",
            "ni",
            "a",
            "o",
            "zor",
            "que",
            "ca",
            "co",
            "mi"
          });
          dictionary1.Add("o", new string[12]
          {
            "nie",
            "ze",
            "dy",
            "da",
            "o",
            "ver",
            "la",
            "lo",
            "s",
            "ny",
            "mo",
            "ra"
          });
          dictionary1.Add("u", new string[4]
          {
            "rt",
            "mo",
            "",
            "s"
          });
          dictionary2.Add("a", new string[12]
          {
            "nny",
            "sper",
            "trina",
            "bo",
            "-bell",
            "boo",
            "lbert",
            "sko",
            "sh",
            "ck",
            "ishe",
            "rk"
          });
          dictionary2.Add("e", new string[9]
          {
            "lla",
            "llo",
            "rnard",
            "cardo",
            "ffe",
            "ppo",
            "ppa",
            "tch",
            "x"
          });
          dictionary2.Add("i", new string[18]
          {
            "llard",
            "lly",
            "lbo",
            "cky",
            "card",
            "ne",
            "nnie",
            "lbert",
            "nono",
            "nano",
            "nana",
            "ana",
            "nsy",
            "msy",
            "skers",
            "rdo",
            "rda",
            "sh"
          });
          dictionary2.Add("o", new string[17]
          {
            "nie",
            "zzy",
            "do",
            "na",
            "la",
            "la",
            "ver",
            "ng",
            "ngus",
            "ny",
            "-mo",
            "llo",
            "ze",
            "ra",
            "ma",
            "cco",
            "z"
          });
          dictionary2.Add("u", new string[11]
          {
            "ssie",
            "bbie",
            "ffy",
            "bba",
            "rt",
            "s",
            "mby",
            "mbo",
            "mbus",
            "ngus",
            "cky"
          });
          source1 = str1 + strArray4[Game1.random.Next(strArray4.Length - 1)];
          for (int index = 1; index < num - 1; ++index)
          {
            source1 = index % 2 != 0 ? source1 + source2[Game1.random.Next(source2.Length)] : source1 + strArray5[Game1.random.Next(strArray5.Length)];
            if (source1.Length >= num)
              break;
          }
          char ch;
          if (Game1.random.NextDouble() < 0.5 && !((IEnumerable<string>) source2).Contains<string>(source1.ElementAt<char>(source1.Length - 1).ToString() ?? ""))
            source1 += strArray6[Game1.random.Next(strArray6.Length)];
          else if (((IEnumerable<string>) source2).Contains<string>(source1.ElementAt<char>(source1.Length - 1).ToString() ?? ""))
          {
            if (Game1.random.NextDouble() < 0.8)
            {
              if (source1.Length <= 3)
              {
                string str2 = source1;
                string[] source3 = dictionary2[source1.ElementAt<char>(source1.Length - 1).ToString() ?? ""];
                Random random = Game1.random;
                Dictionary<string, string[]> dictionary3 = dictionary2;
                ch = source1.ElementAt<char>(source1.Length - 1);
                string key = ch.ToString() ?? "";
                int maxValue = dictionary3[key].Length - 1;
                int index = random.Next(maxValue);
                string str3 = ((IEnumerable<string>) source3).ElementAt<string>(index);
                source1 = str2 + str3;
              }
              else
              {
                string str4 = source1;
                string[] source4 = dictionary1[source1.ElementAt<char>(source1.Length - 1).ToString() ?? ""];
                Random random = Game1.random;
                Dictionary<string, string[]> dictionary4 = dictionary1;
                ch = source1.ElementAt<char>(source1.Length - 1);
                string key = ch.ToString() ?? "";
                int maxValue = dictionary4[key].Length - 1;
                int index = random.Next(maxValue);
                string str5 = ((IEnumerable<string>) source4).ElementAt<string>(index);
                source1 = str4 + str5;
              }
            }
          }
          else
            source1 += source2[Game1.random.Next(source2.Length)];
          for (int index = source1.Length - 1; index > 2; --index)
          {
            string[] source5 = source2;
            ch = source1[index];
            string str6 = ch.ToString();
            if (((IEnumerable<string>) source5).Contains<string>(str6))
            {
              string[] source6 = source2;
              ch = source1[index - 2];
              string str7 = ch.ToString();
              if (((IEnumerable<string>) source6).Contains<string>(str7))
              {
                ch = source1[index - 1];
                switch (ch)
                {
                  case 'c':
                    source1 = source1.Substring(0, index) + "k" + source1.Substring(index);
                    --index;
                    continue;
                  case 'l':
                    source1 = source1.Substring(0, index - 1) + "n" + source1.Substring(index);
                    --index;
                    continue;
                  case 'r':
                    source1 = source1.Substring(0, index - 1) + "k" + source1.Substring(index);
                    --index;
                    continue;
                  default:
                    continue;
                }
              }
            }
          }
          if (source1.Length <= 3 && Game1.random.NextDouble() < 0.1)
            source1 = Game1.random.NextDouble() < 0.5 ? source1 + source1 : source1 + "-" + source1;
          if (source1.Length <= 2 && source1.Last<char>() == 'e')
            source1 += Game1.random.NextDouble() < 0.3 ? "m" : (Game1.random.NextDouble() < 0.5 ? "p" : "b");
          if (source1.ToLower().Contains("sex") || source1.ToLower().Contains("taboo") || source1.ToLower().Contains("fuck") || source1.ToLower().Contains("rape") || source1.ToLower().Contains("cock") || source1.ToLower().Contains("willy") || source1.ToLower().Contains("cum") || source1.ToLower().Contains("goock") || source1.ToLower().Contains("trann") || source1.ToLower().Contains("gook") || source1.ToLower().Contains("bitch") || source1.ToLower().Contains("shit") || source1.ToLower().Contains("pusie") || source1.ToLower().Contains("kike") || source1.ToLower().Contains("nigg") || source1.ToLower().Contains("puss"))
          {
            source1 = Game1.random.NextDouble() < 0.5 ? "Bobo" : "Wumbus";
            break;
          }
          break;
      }
      return source1;
    }

    public string exitCurrentDialogue()
    {
      if (this.isOnFinalDialogue() && this.onFinish != null)
        this.onFinish();
      if (this.temporaryDialogue != null)
        return (string) null;
      int num = this.isCurrentStringContinuedOnNextScreen ? 1 : 0;
      if (this.currentDialogueIndex < this.dialogues.Count - 1)
      {
        ++this.currentDialogueIndex;
        this.checkForSpecialDialogueAttributes();
      }
      else
        this.finishedLastDialogue = true;
      return num != 0 ? this.getCurrentDialogue() : (string) null;
    }

    private void checkForSpecialDialogueAttributes()
    {
      if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("{"))
      {
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("{", "");
        this.isCurrentStringContinuedOnNextScreen = true;
      }
      else
        this.isCurrentStringContinuedOnNextScreen = false;
      this.checkEmotions();
    }

    private void checkEmotions()
    {
      if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("$h"))
      {
        this.currentEmotion = "$h";
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("$h", "");
      }
      else if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("$s"))
      {
        this.currentEmotion = "$s";
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("$s", "");
      }
      else if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("$u"))
      {
        this.currentEmotion = "$u";
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("$u", "");
      }
      else if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("$l"))
      {
        this.currentEmotion = "$l";
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("$l", "");
      }
      else if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("$a"))
      {
        this.currentEmotion = "$a";
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace("$a", "");
      }
      else if (this.dialogues.Count > 0 && this.dialogues[this.currentDialogueIndex].Contains("$"))
      {
        int num = this.dialogues[this.currentDialogueIndex].Length <= this.dialogues[this.currentDialogueIndex].IndexOf("$") + 2 || !char.IsDigit(this.dialogues[this.currentDialogueIndex][this.dialogues[this.currentDialogueIndex].IndexOf("$") + 2]) ? 1 : 2;
        string oldValue = this.dialogues[this.currentDialogueIndex].Substring(this.dialogues[this.currentDialogueIndex].IndexOf("$"), num + 1);
        this.currentEmotion = oldValue;
        this.dialogues[this.currentDialogueIndex] = this.dialogues[this.currentDialogueIndex].Replace(oldValue, "");
      }
      else
        this.currentEmotion = "$neutral";
    }

    public List<NPCDialogueResponse> getNPCResponseOptions() => this.playerResponses;

    public List<Response> getResponseOptions() => new List<Response>(this.playerResponses.Select<NPCDialogueResponse, Response>((Func<NPCDialogueResponse, Response>) (x => (Response) x)));

    public bool isCurrentDialogueAQuestion() => this.isLastDialogueInteractive && this.currentDialogueIndex == this.dialogues.Count - 1;

    public bool chooseResponse(Response response)
    {
      for (int index = 0; index < this.playerResponses.Count; ++index)
      {
        if (this.playerResponses[index].responseKey != null && response.responseKey != null && this.playerResponses[index].responseKey.Equals(response.responseKey))
        {
          if (this.answerQuestionBehavior != null)
          {
            if (this.answerQuestionBehavior(index))
              Game1.currentSpeaker = (NPC) null;
            this.isLastDialogueInteractive = false;
            this.finishedLastDialogue = true;
            this.answerQuestionBehavior = (Dialogue.onAnswerQuestion) null;
            return true;
          }
          if (this.quickResponse)
          {
            this.isLastDialogueInteractive = false;
            this.finishedLastDialogue = true;
            this.isCurrentStringContinuedOnNextScreen = true;
            this.speaker.setNewDialogue(this.quickResponses[index]);
            Game1.drawDialogue(this.speaker);
            this.speaker.faceTowardFarmerForPeriod(4000, 3, false, this.farmer);
            return true;
          }
          if (Game1.isFestival())
          {
            Game1.currentLocation.currentEvent.answerDialogueQuestion(this.speaker, this.playerResponses[index].responseKey);
            this.isLastDialogueInteractive = false;
            this.finishedLastDialogue = true;
            return false;
          }
          this.farmer.changeFriendship(this.playerResponses[index].friendshipChange, this.speaker);
          if (this.playerResponses[index].id != -1)
            this.farmer.addSeenResponse(this.playerResponses[index].id);
          this.isLastDialogueInteractive = false;
          this.finishedLastDialogue = false;
          this.parseDialogueString(this.speaker.Dialogue[this.playerResponses[index].responseKey]);
          this.isCurrentStringContinuedOnNextScreen = true;
          return false;
        }
      }
      return false;
    }

    public static string convertToDwarvish(string str)
    {
      string str1 = "";
      char ch;
      for (int index = 0; index < str.Length; ++index)
      {
        switch (str[index])
        {
          case '\n':
          case 'n':
          case 'p':
            continue;
          case ' ':
          case '!':
          case '"':
          case '\'':
          case ',':
          case '.':
          case '?':
          case 'h':
          case 'm':
          case 's':
            string str2 = str1;
            ch = str[index];
            string str3 = ch.ToString();
            str1 = str2 + str3;
            continue;
          case '0':
            str1 += "Q";
            continue;
          case '1':
            str1 += "M";
            continue;
          case '5':
            str1 += "X";
            continue;
          case '9':
            str1 += "V";
            continue;
          case 'A':
            str1 += "O";
            continue;
          case 'E':
            str1 += "U";
            continue;
          case 'I':
            str1 += "E";
            continue;
          case 'O':
            str1 += "A";
            continue;
          case 'U':
            str1 += "I";
            continue;
          case 'Y':
            str1 += "Ol";
            continue;
          case 'Z':
            str1 += "B";
            continue;
          case 'a':
            str1 += "o";
            continue;
          case 'c':
            str1 += "t";
            continue;
          case 'd':
            str1 += "p";
            continue;
          case 'e':
            str1 += "u";
            continue;
          case 'g':
            str1 += "l";
            continue;
          case 'i':
            str1 += "e";
            continue;
          case 'o':
            str1 += "a";
            continue;
          case 't':
            str1 += "n";
            continue;
          case 'u':
            str1 += "i";
            continue;
          case 'y':
            str1 += "ol";
            continue;
          case 'z':
            str1 += "b";
            continue;
          default:
            if (char.IsLetterOrDigit(str[index]))
            {
              string str4 = str1;
              ch = (char) ((uint) str[index] + 2U);
              string str5 = ch.ToString();
              str1 = str4 + str5;
              continue;
            }
            continue;
        }
      }
      return str1.Replace("nhu", "doo");
    }

    public delegate bool onAnswerQuestion(int whichResponse);
  }
}
