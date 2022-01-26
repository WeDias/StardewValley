// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.SlayMonsterQuest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  public class SlayMonsterQuest : Quest
  {
    public string targetMessage;
    [XmlElement("monsterName")]
    public readonly NetString monsterName = new NetString();
    [XmlElement("target")]
    public readonly NetString target = new NetString();
    [XmlElement("monster")]
    public readonly NetRef<Monster> monster = new NetRef<Monster>();
    [XmlIgnore]
    [Obsolete]
    public NPC actualTarget;
    [XmlElement("numberToKill")]
    public readonly NetInt numberToKill = new NetInt();
    [XmlElement("reward")]
    public readonly NetInt reward = new NetInt();
    [XmlElement("numberKilled")]
    public readonly NetInt numberKilled = new NetInt();
    public readonly NetDescriptionElementList parts = new NetDescriptionElementList();
    public readonly NetDescriptionElementList dialogueparts = new NetDescriptionElementList();
    [XmlElement("objective")]
    public readonly NetDescriptionElementRef objective = new NetDescriptionElementRef();

    public SlayMonsterQuest() => this.questType.Value = 4;

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.parts, (INetSerializable) this.dialogueparts, (INetSerializable) this.objective, (INetSerializable) this.monsterName, (INetSerializable) this.target, (INetSerializable) this.monster, (INetSerializable) this.numberToKill, (INetSerializable) this.reward, (INetSerializable) this.numberKilled);
    }

    public void loadQuestInfo()
    {
      for (int index = 0; index < this.random.Next(1, 100); ++index)
        this.random.Next();
      if (this.target.Value != null && (NetFieldBase<Monster, NetRef<Monster>>) this.monster != (NetRef<Monster>) null)
        return;
      this.questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13696");
      List<string> source = new List<string>();
      int deepestMineLevel = Utility.GetAllPlayerDeepestMineLevel();
      if (deepestMineLevel < 39)
      {
        source.Add("Green Slime");
        if (deepestMineLevel > 10)
          source.Add("Rock Crab");
        if (deepestMineLevel > 30)
          source.Add("Duggy");
      }
      else if (deepestMineLevel < 79)
      {
        source.Add("Frost Jelly");
        if (deepestMineLevel > 70)
          source.Add("Skeleton");
        source.Add("Dust Spirit");
      }
      else
      {
        source.Add("Sludge");
        source.Add("Ghost");
        source.Add("Lava Crab");
        source.Add("Squid Kid");
      }
      int num = this.monsterName.Value == null ? 1 : (this.numberToKill.Value == 0 ? 1 : 0);
      if (num != 0)
        this.monsterName.Value = source.ElementAt<string>(this.random.Next(source.Count));
      if (this.monsterName.Value == "Frost Jelly" || this.monsterName.Value == "Sludge")
      {
        this.monster.Value = new Monster("Green Slime", Vector2.Zero);
        this.monster.Value.Name = this.monsterName.Value;
      }
      else
        this.monster.Value = new Monster(this.monsterName.Value, Vector2.Zero);
      if (num != 0)
      {
        switch (this.monsterName.Value)
        {
          case "Duggy":
            this.parts.Clear();
            this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13711", (object) this.numberToKill.Value));
            this.target.Value = "Clint";
            this.numberToKill.Value = this.random.Next(2, 4);
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 150;
            break;
          case "Frost Jelly":
            this.numberToKill.Value = this.random.Next(4, 9);
            this.numberToKill.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill - (int) (NetFieldBase<int, NetInt>) this.numberToKill % 2;
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 85;
            break;
          case "Ghost":
            this.numberToKill.Value = this.random.Next(1, 3);
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 250;
            break;
          case "Green Slime":
            this.numberToKill.Value = this.random.Next(4, 9);
            this.numberToKill.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill - (int) (NetFieldBase<int, NetInt>) this.numberToKill % 2;
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 60;
            break;
          case "Lava Crab":
            this.numberToKill.Value = this.random.Next(2, 6);
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 180;
            break;
          case "Rock Crab":
            this.numberToKill.Value = this.random.Next(2, 6);
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 75;
            break;
          case "Sludge":
            this.numberToKill.Value = this.random.Next(4, 9);
            this.numberToKill.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill - (int) (NetFieldBase<int, NetInt>) this.numberToKill % 2;
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 125;
            break;
          case "Squid Kid":
            this.numberToKill.Value = this.random.Next(1, 3);
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 350;
            break;
          default:
            this.numberToKill.Value = this.random.Next(1, 4);
            this.reward.Value = (int) (NetFieldBase<int, NetInt>) this.numberToKill * 120;
            break;
        }
      }
      if (this.monsterName.Value.Equals("Green Slime") || this.monsterName.Value.Equals("Frost Jelly") || this.monsterName.Value.Equals("Sludge"))
      {
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13723", (object) this.numberToKill.Value, this.monsterName.Value.Equals("Frost Jelly") ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13725") : (this.monsterName.Value.Equals("Sludge") ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13727") : (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13728"))));
        this.target.Value = "Lewis";
        this.dialogueparts.Clear();
        this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13730");
        if (this.random.NextDouble() < 0.5)
        {
          this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13731");
          this.dialogueparts.Add((DescriptionElement) (this.random.NextDouble() < 0.5 ? "Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13732" : "Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13733"));
          DescriptionElement descriptionElement = ((IEnumerable<DescriptionElement>) new DescriptionElement[16]
          {
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.795",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.796",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.797",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.798",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.799",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.800",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.801",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.802",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.803",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.804",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.805",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.806",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.807",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.808",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.809",
            (DescriptionElement) "Strings\\StringsFromCSFiles:Dialogue.cs.810"
          }).ElementAt<DescriptionElement>(this.random.Next(16));
          this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13734", this.random.NextDouble() < 0.5 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13735") : (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13736"), (object) descriptionElement, this.random.NextDouble() < 0.3 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13740") : (this.random.NextDouble() < 0.5 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13741") : (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13742"))));
        }
        else
          this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13744");
      }
      else if (this.monsterName.Value.Equals("Rock Crab") || this.monsterName.Value.Equals("Lava Crab"))
      {
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13747", (object) this.numberToKill.Value));
        this.target.Value = "Demetrius";
        this.dialogueparts.Clear();
        this.dialogueparts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13750", (object) this.monster.Value));
      }
      else
      {
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13752", (object) this.monster.Value, (object) this.numberToKill.Value, this.random.NextDouble() < 0.3 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13755") : (this.random.NextDouble() < 0.5 ? (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13756") : (object) new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13757"))));
        this.target.Value = "Wizard";
        this.dialogueparts.Clear();
        this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13760");
      }
      if (this.target.Value.Equals("Wizard") && !Utility.doesAnyFarmerHaveMail("wizardJunimoNote") && !Utility.doesAnyFarmerHaveMail("JojaMember"))
      {
        this.parts.Clear();
        this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13764", (object) this.numberToKill.Value, (object) this.monster.Value));
        this.target.Value = "Lewis";
        this.dialogueparts.Clear();
        this.dialogueparts.Add((DescriptionElement) "Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13767");
      }
      this.parts.Add(new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13274", (object) this.reward.Value));
      this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13770", (object) "0", (object) this.numberToKill.Value, (object) this.monster.Value);
    }

    public override void reloadDescription()
    {
      if (this._questDescription == "")
        this.loadQuestInfo();
      string str1 = "";
      string str2 = "";
      if (this.parts != null && this.parts.Count != 0)
      {
        foreach (DescriptionElement part in (NetList<DescriptionElement, NetDescriptionElementRef>) this.parts)
          str1 += part.loadDescriptionElement();
        this.questDescription = str1;
      }
      if (this.dialogueparts != null && this.dialogueparts.Count != 0)
      {
        foreach (DescriptionElement dialoguepart in (NetList<DescriptionElement, NetDescriptionElementRef>) this.dialogueparts)
          str2 += dialoguepart.loadDescriptionElement();
        this.targetMessage = str2;
      }
      else
      {
        if ((int) (NetFieldBase<int, NetInt>) this.id == 0)
          return;
        Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
        if (dictionary == null || !dictionary.ContainsKey((int) (NetFieldBase<int, NetInt>) this.id))
          return;
        string[] strArray = dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Split('/');
        if (strArray == null || strArray.Length < 9)
          return;
        this.targetMessage = strArray[9];
      }
    }

    public override void reloadObjective()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.numberKilled == 0 && (int) (NetFieldBase<int, NetInt>) this.id != 0)
        return;
      if ((int) (NetFieldBase<int, NetInt>) this.numberKilled < (int) (NetFieldBase<int, NetInt>) this.numberToKill)
        this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13770", (object) this.numberKilled.Value, (object) this.numberToKill.Value, (object) this.monster.Value);
      if (this.objective.Value == null)
        return;
      this.currentObjective = this.objective.Value.loadDescriptionElement();
    }

    public override bool checkIfComplete(
      NPC n = null,
      int number1 = -1,
      int number2 = -1,
      Item item = null,
      string monsterName = null)
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.completed)
        return false;
      if (monsterName == null)
        monsterName = "Green Slime";
      if (n == null && monsterName != null && monsterName.Contains(this.monsterName.Value) && (int) (NetFieldBase<int, NetInt>) this.numberKilled < (int) (NetFieldBase<int, NetInt>) this.numberToKill)
      {
        this.numberKilled.Value = Math.Min((int) (NetFieldBase<int, NetInt>) this.numberToKill, (int) (NetFieldBase<int, NetInt>) this.numberKilled + 1);
        if ((int) (NetFieldBase<int, NetInt>) this.numberKilled >= (int) (NetFieldBase<int, NetInt>) this.numberToKill)
        {
          if (this.target.Value == null || this.target.Value.Equals("null"))
          {
            this.questComplete();
          }
          else
          {
            this.objective.Value = new DescriptionElement("Strings\\StringsFromCSFiles:FishingQuest.cs.13277", (object) Game1.getCharacterFromName((string) (NetFieldBase<string, NetString>) this.target));
            Game1.playSound("jingle1");
          }
        }
        else if (this.monster.Value == null)
        {
          if (monsterName == "Frost Jelly" || monsterName == "Sludge")
          {
            this.monster.Value = new Monster("Green Slime", Vector2.Zero);
            this.monster.Value.Name = monsterName;
          }
          else
            this.monster.Value = new Monster(monsterName, Vector2.Zero);
        }
        Game1.dayTimeMoneyBox.moneyDial.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(387, 497, 3, 8), 800f, 1, 0, Game1.dayTimeMoneyBox.position + new Vector2(228f, 244f), false, false, 1f, 0.01f, Color.White, 4f, 0.3f, 0.0f, 0.0f)
        {
          scaleChangeChange = -0.012f
        });
      }
      else if (n != null && this.target.Value != null && !this.target.Value.Equals("null") && (int) (NetFieldBase<int, NetInt>) this.numberKilled >= (int) (NetFieldBase<int, NetInt>) this.numberToKill && n.Name.Equals(this.target.Value) && n.isVillager())
      {
        this.reloadDescription();
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
