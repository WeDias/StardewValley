// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.FishingQuest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  public class FishingQuest : Quest
  {
    [XmlElement("target")]
    public readonly NetString target = new NetString();
    public string targetMessage;
    [XmlElement("numberToFish")]
    public readonly NetInt numberToFish = new NetInt();
    [XmlElement("reward")]
    public readonly NetInt reward = new NetInt();
    [XmlElement("numberFished")]
    public readonly NetInt numberFished = new NetInt();
    [XmlElement("whichFish")]
    public readonly NetInt whichFish = new NetInt();
    [XmlElement("fish")]
    public readonly NetRef<StardewValley.Object> fish = new NetRef<StardewValley.Object>();
    public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
    public readonly NetDescriptionElementList dialogueparts = new NetDescriptionElementList();
    [XmlElement("objective")]
    public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

    public FishingQuest() => this.questType.Value = 7;

    protected override void initNetFields() => this.NetFields.AddFields((INetSerializable) this.parts, (INetSerializable) this.dialogueparts, (INetSerializable) this.objective, (INetSerializable) this.target, (INetSerializable) this.numberToFish, (INetSerializable) this.reward, (INetSerializable) this.numberFished, (INetSerializable) this.whichFish, (INetSerializable) this.fish);

    public void loadQuestInfo()
    {
      if (this.target.Value != null && this.fish.Value != null)
        return;
      this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingQuest.cs.13227");
      if (this.random.NextDouble() < 0.5)
      {
        string currentSeason = Game1.currentSeason;
        if (!(currentSeason == "spring"))
        {
          if (!(currentSeason == "summer"))
          {
            if (!(currentSeason == "fall"))
            {
              if (currentSeason == "winter")
              {
                int[] numArray = new int[9]
                {
                  130,
                  131,
                  136,
                  141,
                  144,
                  146,
                  147,
                  150,
                  151
                };
                this.whichFish.Value = numArray[this.random.Next(numArray.Length)];
              }
            }
            else
            {
              int[] numArray = new int[8]
              {
                129,
                131,
                136,
                137,
                139,
                142,
                143,
                150
              };
              this.whichFish.Value = numArray[this.random.Next(numArray.Length)];
            }
          }
          else
          {
            int[] numArray = new int[10]
            {
              130,
              131,
              136,
              138,
              142,
              144,
              145,
              146,
              149,
              150
            };
            this.whichFish.Value = numArray[this.random.Next(numArray.Length)];
          }
        }
        else
        {
          int[] numArray = new int[8]
          {
            129,
            131,
            136,
            137,
            142,
            143,
            145,
            147
          };
          this.whichFish.Value = numArray[this.random.Next(numArray.Length)];
        }
        this.fish.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.whichFish, 1);
        this.numberToFish.Value = (int) Math.Ceiling(90.0 / (double) Math.Max(1, (int) (NetFieldBase<int, NetInt>) this.fish.Value.price)) + Game1.player.FishingLevel / 5;
        this.reward.Value = this.numberToFish.Value * (int) (NetFieldBase<int, NetInt>) this.fish.Value.price;
        this.target.Value = "Demetrius";
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13228", (object) this.fish.Value, (object) this.numberToFish.Value));
        this.dialogueparts.Clear();
        this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13231", (object) this.fish.Value, (object) ((IEnumerable<DescriptionElement>) new DescriptionElement[4]
        {
          (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13233",
          (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13234",
          (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13235",
          new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13236", (object) this.fish.Value)
        }).ElementAt<DescriptionElement>(this.random.Next(4))));
        this.objective.Value = this.fish.Value.name.Equals("Octopus") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13243", (object) 0, (object) this.numberToFish.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", (object) 0, (object) this.numberToFish.Value, (object) this.fish.Value);
      }
      else
      {
        string currentSeason = Game1.currentSeason;
        if (!(currentSeason == "spring"))
        {
          if (!(currentSeason == "summer"))
          {
            if (!(currentSeason == "fall"))
            {
              if (currentSeason == "winter")
              {
                int[] numArray = new int[13]
                {
                  130,
                  131,
                  136,
                  141,
                  143,
                  144,
                  146,
                  147,
                  150,
                  151,
                  699,
                  702,
                  705
                };
                this.whichFish.Value = numArray[this.random.Next(numArray.Length)];
              }
            }
            else
            {
              int[] numArray = new int[11]
              {
                129,
                131,
                136,
                137,
                139,
                142,
                143,
                150,
                699,
                702,
                705
              };
              this.whichFish.Value = numArray[this.random.Next(numArray.Length)];
            }
          }
          else
          {
            int[] numArray = new int[12]
            {
              128,
              130,
              131,
              136,
              138,
              142,
              144,
              145,
              146,
              149,
              150,
              702
            };
            this.whichFish.Value = numArray[this.random.Next(numArray.Length)];
          }
        }
        else
        {
          int[] numArray = new int[9]
          {
            129,
            131,
            136,
            137,
            142,
            143,
            145,
            147,
            702
          };
          this.whichFish.Value = numArray[this.random.Next(numArray.Length)];
        }
        this.target.Value = "Willy";
        this.fish.Value = new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.whichFish, 1);
        this.numberToFish.Value = (int) Math.Ceiling(90.0 / (double) Math.Max(1, (int) (NetFieldBase<int, NetInt>) this.fish.Value.price)) + Game1.player.FishingLevel / 5;
        this.reward.Value = this.numberToFish.Value * (int) (NetFieldBase<int, NetInt>) this.fish.Value.price;
        this.parts.Clear();
        if ((bool) (NetFieldBase<bool, NetBool>) Game1.player.isMale)
          this.parts.Add(this.fish.Value.name.Equals("Squid") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13248", (object) this.reward.Value, (object) this.numberToFish.Value, (object) new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13253")) : new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13248", (object) this.reward.Value, (object) this.numberToFish.Value, (object) this.fish.Value));
        else
          this.parts.Add(this.fish.Value.name.Equals("Squid") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13251", (object) this.reward.Value, (object) this.numberToFish.Value, (object) new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13253")) : new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13251", (object) this.reward.Value, (object) this.numberToFish.Value, (object) this.fish.Value));
        this.dialogueparts.Clear();
        this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13256", (object) this.fish.Value));
        this.dialogueparts.Add(((IEnumerable<DescriptionElement>) new DescriptionElement[4]
        {
          (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13258",
          (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13259",
          new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13260", (object) ((IEnumerable<DescriptionElement>) new DescriptionElement[6]
          {
            (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13261",
            (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13262",
            (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13263",
            (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13264",
            (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13265",
            (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13266"
          }).ElementAt<DescriptionElement>(this.random.Next(6))),
          (DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13267"
        }).ElementAt<DescriptionElement>(this.random.Next(4)));
        this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13268"));
        this.objective.Value = this.fish.Value.name.Equals("Squid") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13255", (object) 0, (object) this.numberToFish.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", (object) 0, (object) this.numberToFish.Value, (object) this.fish.Value);
      }
      this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13274", (object) this.reward.Value));
      this.parts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:FishingQuest.cs.13275");
    }

    public override void reloadDescription()
    {
      if (this._questDescription == "")
        this.loadQuestInfo();
      if (this.parts.Count == 0 || this.parts == null || this.dialogueparts.Count == 0 || this.dialogueparts == null)
        return;
      string str1 = "";
      string str2 = "";
      foreach (DescriptionElement part in (NetList<DescriptionElement, NetDescriptionElementRef>) this.parts)
        str1 += part.loadDescriptionElement();
      foreach (DescriptionElement dialoguepart in (NetList<DescriptionElement, NetDescriptionElementRef>) this.dialogueparts)
        str2 += dialoguepart.loadDescriptionElement();
      this.questDescription = str1;
      this.targetMessage = str2;
    }

    public override void reloadObjective()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.numberFished < (int) (NetFieldBase<int, NetInt>) this.numberToFish)
        this.objective.Value = this.fish.Value.name.Equals("Octopus") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13243", (object) this.numberFished.Value, (object) this.numberToFish.Value) : (this.fish.Value.name.Equals("Squid") ? new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13255", (object) this.numberFished.Value, (object) this.numberToFish.Value) : new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13244", (object) this.numberFished.Value, (object) this.numberToFish.Value, (object) this.fish.Value));
      if (this.objective.Value == null)
        return;
      this.currentObjective = this.objective.Value.loadDescriptionElement();
    }

    public override bool checkIfComplete(
      NPC n = null,
      int fishid = -1,
      int number2 = 1,
      Item item = null,
      string monsterName = null)
    {
      this.loadQuestInfo();
      if (n == null && fishid != -1 && fishid == (int) (NetFieldBase<int, NetInt>) this.whichFish && (int) (NetFieldBase<int, NetInt>) this.numberFished < (int) (NetFieldBase<int, NetInt>) this.numberToFish)
      {
        this.numberFished.Value = Math.Min((int) (NetFieldBase<int, NetInt>) this.numberToFish, (int) (NetFieldBase<int, NetInt>) this.numberFished + number2);
        if ((int) (NetFieldBase<int, NetInt>) this.numberFished >= (int) (NetFieldBase<int, NetInt>) this.numberToFish)
        {
          if (this.target.Value == null)
            this.target.Value = "Willy";
          this.objective.Value = new DescriptionElement("Strings\\Quests:ObjectiveReturnToNPC", (object) Game1.getCharacterFromName((string) (NetFieldBase<string, NetString>) this.target));
          Game1.playSound("jingle1");
        }
      }
      else if (n != null && (int) (NetFieldBase<int, NetInt>) this.numberFished >= (int) (NetFieldBase<int, NetInt>) this.numberToFish && this.target.Value != null && n.Name.Equals(this.target.Value) && n.isVillager() && !(bool) (NetFieldBase<bool, NetBool>) this.completed)
      {
        n.CurrentDialogue.Push(new Dialogue(this.targetMessage, n));
        this.moneyReward.Value = (int) (NetFieldBase<int, NetInt>) this.reward;
        this.questComplete();
        Game1.drawDialogue(n);
        return true;
      }
      return false;
    }
  }
}
