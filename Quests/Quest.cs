// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.Quest
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Quests
{
  [XmlInclude(typeof (SocializeQuest))]
  [XmlInclude(typeof (SlayMonsterQuest))]
  [XmlInclude(typeof (ResourceCollectionQuest))]
  [XmlInclude(typeof (ItemDeliveryQuest))]
  [XmlInclude(typeof (ItemHarvestQuest))]
  [XmlInclude(typeof (CraftingQuest))]
  [XmlInclude(typeof (FishingQuest))]
  [XmlInclude(typeof (GoSomewhereQuest))]
  [XmlInclude(typeof (LostItemQuest))]
  [XmlInclude(typeof (DescriptionElement))]
  [XmlInclude(typeof (SecretLostItemQuest))]
  public class Quest : INetObject<NetFields>, IQuest
  {
    public const int type_basic = 1;
    public const int type_crafting = 2;
    public const int type_itemDelivery = 3;
    public const int type_monster = 4;
    public const int type_socialize = 5;
    public const int type_location = 6;
    public const int type_fishing = 7;
    public const int type_building = 8;
    public const int type_harvest = 9;
    public const int type_resource = 10;
    public const int type_weeding = 11;
    public string _currentObjective = "";
    public string _questDescription = "";
    public string _questTitle = "";
    [XmlElement("rewardDescription")]
    public readonly NetString rewardDescription = new NetString();
    [XmlElement("completionString")]
    public readonly NetString completionString = new NetString();
    protected Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
    [XmlElement("accepted")]
    public readonly NetBool accepted = new NetBool();
    [XmlElement("completed")]
    public readonly NetBool completed = new NetBool();
    [XmlElement("dailyQuest")]
    public readonly NetBool dailyQuest = new NetBool();
    [XmlElement("showNew")]
    public readonly NetBool showNew = new NetBool();
    [XmlElement("canBeCancelled")]
    public readonly NetBool canBeCancelled = new NetBool();
    [XmlElement("destroy")]
    public readonly NetBool destroy = new NetBool();
    [XmlElement("id")]
    public readonly NetInt id = new NetInt();
    [XmlElement("moneyReward")]
    public readonly NetInt moneyReward = new NetInt();
    [XmlElement("questType")]
    public readonly NetInt questType = new NetInt();
    [XmlElement("daysLeft")]
    public readonly NetInt daysLeft = new NetInt();
    [XmlElement("dayQuestAccepted")]
    public readonly NetInt dayQuestAccepted = new NetInt(-1);
    public readonly NetIntList nextQuests = new NetIntList();
    private bool _loadedDescription;
    private bool _loadedTitle;

    public NetFields NetFields { get; } = new NetFields();

    public Quest() => this.initNetFields();

    protected virtual void initNetFields() => this.NetFields.AddFields((INetSerializable) this.rewardDescription, (INetSerializable) this.completionString, (INetSerializable) this.accepted, (INetSerializable) this.completed, (INetSerializable) this.dailyQuest, (INetSerializable) this.showNew, (INetSerializable) this.canBeCancelled, (INetSerializable) this.destroy, (INetSerializable) this.id, (INetSerializable) this.moneyReward, (INetSerializable) this.questType, (INetSerializable) this.daysLeft, (INetSerializable) this.nextQuests, (INetSerializable) this.dayQuestAccepted);

    public string questTitle
    {
      get
      {
        if (!this._loadedTitle)
        {
          switch (this.questType.Value)
          {
            case 3:
              this._questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:ItemDeliveryQuest.cs.13285");
              break;
            case 4:
              this._questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:SlayMonsterQuest.cs.13696");
              break;
            case 5:
              this._questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:SocializeQuest.cs.13785");
              break;
            case 7:
              this._questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingQuest.cs.13227");
              break;
            case 10:
              this._questTitle = Game1.content.LoadString("Strings\\StringsFromCSFiles:ResourceCollectionQuest.cs.13640");
              break;
          }
          Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
          if (dictionary != null && dictionary.ContainsKey((int) (NetFieldBase<int, NetInt>) this.id))
            this._questTitle = dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Split('/')[1];
          this._loadedTitle = true;
        }
        if (this._questTitle == null)
          this._questTitle = "";
        return this._questTitle;
      }
      set => this._questTitle = value;
    }

    [XmlIgnore]
    public string questDescription
    {
      get
      {
        if (!this._loadedDescription)
        {
          this.reloadDescription();
          Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
          if (dictionary != null && dictionary.ContainsKey((int) (NetFieldBase<int, NetInt>) this.id))
            this._questDescription = dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Split('/')[2];
          this._loadedDescription = true;
        }
        if (this._questDescription == null)
          this._questDescription = "";
        return this._questDescription;
      }
      set => this._questDescription = value;
    }

    [XmlIgnore]
    public string currentObjective
    {
      get
      {
        Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
        if (dictionary != null && dictionary.ContainsKey((int) (NetFieldBase<int, NetInt>) this.id))
        {
          string[] strArray = dictionary[(int) (NetFieldBase<int, NetInt>) this.id].Split('/');
          if (strArray[3].Length > 1)
            this._currentObjective = strArray[3];
        }
        this.reloadObjective();
        if (this._currentObjective == null)
          this._currentObjective = "";
        return this._currentObjective;
      }
      set => this._currentObjective = value;
    }

    public static Quest getQuestFromId(int id)
    {
      Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Quests");
      if (dictionary == null || !dictionary.ContainsKey(id))
        return (Quest) null;
      string[] strArray1 = dictionary[id].Split('/');
      string str1 = strArray1[0];
      Quest questFromId = (Quest) null;
      string[] strArray2 = strArray1[4].Split(' ');
      switch (str1)
      {
        case "Basic":
          questFromId = new Quest();
          questFromId.questType.Value = 1;
          break;
        case "Building":
          questFromId = new Quest();
          questFromId.questType.Value = 8;
          questFromId.completionString.Value = strArray2[0];
          break;
        case "Crafting":
          questFromId = (Quest) new CraftingQuest(Convert.ToInt32(strArray2[0]), strArray2[1].ToLower().Equals("true"));
          questFromId.questType.Value = 2;
          break;
        case "ItemDelivery":
          questFromId = (Quest) new ItemDeliveryQuest();
          (questFromId as ItemDeliveryQuest).target.Value = strArray2[0];
          (questFromId as ItemDeliveryQuest).item.Value = Convert.ToInt32(strArray2[1]);
          (questFromId as ItemDeliveryQuest).targetMessage = strArray1[9];
          if (strArray2.Length > 2)
            (questFromId as ItemDeliveryQuest).number.Value = Convert.ToInt32(strArray2[2]);
          questFromId.questType.Value = 3;
          break;
        case "ItemHarvest":
          questFromId = (Quest) new ItemHarvestQuest(Convert.ToInt32(strArray2[0]), strArray2.Length > 1 ? Convert.ToInt32(strArray2[1]) : 1);
          break;
        case "Location":
          questFromId = (Quest) new GoSomewhereQuest(strArray2[0]);
          questFromId.questType.Value = 6;
          break;
        case "LostItem":
          questFromId = (Quest) new LostItemQuest(strArray2[0], strArray2[2], Convert.ToInt32(strArray2[1]), Convert.ToInt32(strArray2[3]), Convert.ToInt32(strArray2[4]));
          break;
        case "Monster":
          questFromId = (Quest) new SlayMonsterQuest();
          (questFromId as SlayMonsterQuest).loadQuestInfo();
          (questFromId as SlayMonsterQuest).monster.Value.Name = strArray2[0].Replace('_', ' ');
          (questFromId as SlayMonsterQuest).monsterName.Value = (questFromId as SlayMonsterQuest).monster.Value.Name;
          (questFromId as SlayMonsterQuest).numberToKill.Value = Convert.ToInt32(strArray2[1]);
          if (strArray2.Length > 2)
            (questFromId as SlayMonsterQuest).target.Value = strArray2[2];
          else
            (questFromId as SlayMonsterQuest).target.Value = "null";
          questFromId.questType.Value = 4;
          break;
        case "SecretLostItem":
          questFromId = (Quest) new SecretLostItemQuest(strArray2[0], Convert.ToInt32(strArray2[1]), Convert.ToInt32(strArray2[2]), Convert.ToInt32(strArray2[3]));
          break;
        case "Social":
          questFromId = (Quest) new SocializeQuest();
          (questFromId as SocializeQuest).loadQuestInfo();
          break;
      }
      questFromId.id.Value = id;
      questFromId.questTitle = strArray1[1];
      questFromId.questDescription = strArray1[2];
      if (strArray1[3].Length > 1)
        questFromId.currentObjective = strArray1[3];
      foreach (string str2 in strArray1[5].Split(' '))
      {
        if (str2.StartsWith("h"))
        {
          if (Game1.IsMasterGame)
            str2 = str2.Substring(1);
          else
            continue;
        }
        questFromId.nextQuests.Add(Convert.ToInt32(str2));
      }
      questFromId.showNew.Value = true;
      questFromId.moneyReward.Value = Convert.ToInt32(strArray1[6]);
      questFromId.rewardDescription.Value = strArray1[6].Equals("-1") ? (string) null : strArray1[7];
      if (strArray1.Length > 8)
        questFromId.canBeCancelled.Value = strArray1[8].Equals("true");
      return questFromId;
    }

    public virtual void reloadObjective()
    {
    }

    public virtual void reloadDescription()
    {
    }

    public virtual void adjustGameLocation(GameLocation location)
    {
    }

    public virtual void accept() => this.accepted.Value = true;

    public virtual bool checkIfComplete(NPC n = null, int number1 = -1, int number2 = -2, Item item = null, string str = null)
    {
      if (this.completionString.Value == null || str == null || !str.Equals(this.completionString.Value))
        return false;
      this.questComplete();
      return true;
    }

    public bool hasReward()
    {
      if ((int) (NetFieldBase<int, NetInt>) this.moneyReward > 0)
        return true;
      return this.rewardDescription.Value != null && this.rewardDescription.Value.Length > 2;
    }

    public virtual bool isSecretQuest() => false;

    public virtual void questComplete()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.completed)
        return;
      if ((bool) (NetFieldBase<bool, NetBool>) this.dailyQuest || (int) (NetFieldBase<int, NetInt>) this.questType == 7)
        ++Game1.stats.QuestsCompleted;
      this.completed.Value = true;
      if (this.nextQuests.Count > 0)
      {
        foreach (int nextQuest in (NetList<int, NetInt>) this.nextQuests)
        {
          if (nextQuest > 0)
            Game1.player.questLog.Add(Quest.getQuestFromId(nextQuest));
        }
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest.cs.13636"), 2));
      }
      if ((int) (NetFieldBase<int, NetInt>) this.moneyReward <= 0 && (this.rewardDescription.Value == null || this.rewardDescription.Value.Length <= 2))
        Game1.player.questLog.Remove(this);
      else
        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest.cs.13636"), 2));
      Game1.playSound("questcomplete");
      if (this.id.Value == 126)
      {
        Game1.player.mailReceived.Add("emilyFiber");
        Game1.player.activeDialogueEvents.Add("emilyFiber", 2);
      }
      Game1.dayTimeMoneyBox.questsDirty = true;
    }

    public string GetName() => this.questTitle;

    public string GetDescription() => this.questDescription;

    public bool IsHidden() => this.isSecretQuest();

    public List<string> GetObjectiveDescriptions() => new List<string>()
    {
      this.currentObjective
    };

    public bool CanBeCancelled() => this.canBeCancelled.Value;

    public bool HasReward()
    {
      if (this.HasMoneyReward())
        return true;
      return this.rewardDescription.Value != null && this.rewardDescription.Value.Length > 2;
    }

    public bool HasMoneyReward() => this.completed.Value && this.moneyReward.Value > 0;

    public void MarkAsViewed() => this.showNew.Value = false;

    public bool ShouldDisplayAsNew() => this.showNew.Value;

    public bool ShouldDisplayAsComplete() => this.completed.Value && !this.IsHidden();

    public bool IsTimedQuest() => this.dailyQuest.Value;

    public int GetDaysLeft() => (int) (NetFieldBase<int, NetInt>) this.daysLeft;

    public int GetMoneyReward() => this.moneyReward.Value;

    public void OnMoneyRewardClaimed()
    {
      this.moneyReward.Value = 0;
      this.destroy.Value = true;
    }

    public bool OnLeaveQuestPage()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.completed && (int) (NetFieldBase<int, NetInt>) this.moneyReward <= 0)
        this.destroy.Value = true;
      if (!this.destroy.Value)
        return false;
      Game1.player.questLog.Remove(this);
      return true;
    }
  }
}
