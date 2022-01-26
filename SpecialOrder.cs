// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrder
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.GameData;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  [XmlInclude(typeof (SpecialOrder))]
  [XmlInclude(typeof (OrderObjective))]
  [XmlInclude(typeof (ShipObjective))]
  [XmlInclude(typeof (SlayObjective))]
  [XmlInclude(typeof (DeliverObjective))]
  [XmlInclude(typeof (FishObjective))]
  [XmlInclude(typeof (GiftObjective))]
  [XmlInclude(typeof (JKScoreObjective))]
  [XmlInclude(typeof (ReachMineFloorObjective))]
  [XmlInclude(typeof (CollectObjective))]
  [XmlInclude(typeof (DonateObjective))]
  [XmlInclude(typeof (MailReward))]
  [XmlInclude(typeof (MoneyReward))]
  [XmlInclude(typeof (GemsReward))]
  [XmlInclude(typeof (ResetEventReward))]
  [XmlInclude(typeof (OrderReward))]
  [XmlInclude(typeof (FriendshipReward))]
  public class SpecialOrder : INetObject<NetFields>, IQuest
  {
    [XmlIgnore]
    public Action<Farmer, Item, int> onItemShipped;
    [XmlIgnore]
    public Action<Farmer, Monster> onMonsterSlain;
    [XmlIgnore]
    public Action<Farmer, Item> onFishCaught;
    [XmlIgnore]
    public Action<Farmer, NPC, Item> onGiftGiven;
    [XmlIgnore]
    public Func<Farmer, NPC, Item, int> onItemDelivered;
    [XmlIgnore]
    public Action<Farmer, Item> onItemCollected;
    [XmlIgnore]
    public Action<Farmer, int> onMineFloorReached;
    [XmlIgnore]
    public Action<Farmer, int> onJKScoreAchieved;
    [XmlIgnore]
    protected bool _objectiveRegistrationDirty;
    [XmlElement("preSelectedItems")]
    public NetStringDictionary<int, NetInt> preSelectedItems = new NetStringDictionary<int, NetInt>();
    [XmlElement("selectedRandomElements")]
    public NetStringDictionary<int, NetInt> selectedRandomElements = new NetStringDictionary<int, NetInt>();
    [XmlElement("objectives")]
    public NetList<OrderObjective, NetRef<OrderObjective>> objectives = new NetList<OrderObjective, NetRef<OrderObjective>>();
    [XmlElement("generationSeed")]
    public NetInt generationSeed = new NetInt();
    [XmlElement("seenParticipantsIDs")]
    public NetLongDictionary<bool, NetBool> seenParticipants = new NetLongDictionary<bool, NetBool>();
    [XmlElement("participantsIDs")]
    public NetLongDictionary<bool, NetBool> participants = new NetLongDictionary<bool, NetBool>();
    [XmlElement("unclaimedRewardsIDs")]
    public NetLongDictionary<bool, NetBool> unclaimedRewards = new NetLongDictionary<bool, NetBool>();
    [XmlElement("donatedItems")]
    public readonly NetCollection<Item> donatedItems = new NetCollection<Item>();
    [XmlElement("appliedSpecialRules")]
    public bool appliedSpecialRules;
    [XmlIgnore]
    public readonly NetMutex donateMutex = new NetMutex();
    [XmlIgnore]
    protected int _isIslandOrder = -1;
    [XmlElement("rewards")]
    public NetList<OrderReward, NetRef<OrderReward>> rewards = new NetList<OrderReward, NetRef<OrderReward>>();
    [XmlIgnore]
    protected int _moneyReward = -1;
    [XmlElement("questKey")]
    public NetString questKey = new NetString();
    [XmlElement("questName")]
    public NetString questName = new NetString("Strings\\SpecialOrders:PlaceholderName");
    [XmlElement("questDescription")]
    public NetString questDescription = new NetString("Strings\\SpecialOrders:PlaceholderDescription");
    [XmlElement("requester")]
    public NetString requester = new NetString();
    [XmlElement("orderType")]
    public NetString orderType = new NetString("");
    [XmlElement("specialRule")]
    public NetString specialRule = new NetString("");
    [XmlElement("readyForRemoval")]
    public NetBool readyForRemoval = new NetBool(false);
    [XmlElement("itemToRemoveOnEnd")]
    public NetInt itemToRemoveOnEnd = new NetInt(-1);
    [XmlElement("mailToRemoveOnEnd")]
    public NetString mailToRemoveOnEnd = new NetString((string) null);
    [XmlIgnore]
    protected string _localizedName;
    [XmlIgnore]
    protected string _localizedDescription;
    [XmlElement("dueDate")]
    public NetInt dueDate = new NetInt();
    [XmlElement("duration")]
    public NetEnum<SpecialOrder.QuestDuration> questDuration = new NetEnum<SpecialOrder.QuestDuration>();
    [XmlIgnore]
    protected List<OrderObjective> _registeredObjectives = new List<OrderObjective>();
    [XmlIgnore]
    protected Dictionary<Item, bool> _highlightLookup;
    [XmlIgnore]
    protected SpecialOrderData _orderData;
    [XmlElement("questState")]
    public NetEnum<SpecialOrder.QuestState> questState = new NetEnum<SpecialOrder.QuestState>(SpecialOrder.QuestState.InProgress);

    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public SpecialOrder() => this.InitializeNetFields();

    public virtual void SetDuration(SpecialOrder.QuestDuration duration)
    {
      this.questDuration.Value = duration;
      WorldDate worldDate = new WorldDate();
      switch (duration)
      {
        case SpecialOrder.QuestDuration.Week:
          worldDate = new WorldDate(Game1.year, Game1.currentSeason, (Game1.dayOfMonth - 1) / 7 * 7);
          ++worldDate.TotalDays;
          worldDate.TotalDays += 7;
          break;
        case SpecialOrder.QuestDuration.Month:
          worldDate = new WorldDate(Game1.year, Game1.currentSeason, 0);
          ++worldDate.TotalDays;
          worldDate.TotalDays += 28;
          break;
        case SpecialOrder.QuestDuration.TwoWeeks:
          worldDate = new WorldDate(Game1.year, Game1.currentSeason, (Game1.dayOfMonth - 1) / 7 * 7);
          ++worldDate.TotalDays;
          worldDate.TotalDays += 14;
          break;
        case SpecialOrder.QuestDuration.TwoDays:
          worldDate = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
          worldDate.TotalDays += 2;
          break;
        case SpecialOrder.QuestDuration.ThreeDays:
          worldDate = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
          worldDate.TotalDays += 3;
          break;
      }
      this.dueDate.Value = worldDate.TotalDays;
    }

    public virtual void OnFail()
    {
      foreach (OrderObjective objective in this.objectives)
        objective.OnFail();
      for (int index = 0; index < this.donatedItems.Count; ++index)
      {
        Item donatedItem = this.donatedItems[index];
        this.donatedItems[index] = (Item) null;
        if (donatedItem != null)
        {
          Game1.player.team.returnedDonations.Add(donatedItem);
          Game1.player.team.newLostAndFoundItems.Value = true;
        }
      }
      if (Game1.IsMasterGame)
        this.HostHandleQuestEnd();
      this.questState.Value = SpecialOrder.QuestState.Failed;
      this._RemoveSpecialRuleIfNecessary();
    }

    public virtual int GetCompleteObjectivesCount()
    {
      int completeObjectivesCount = 0;
      foreach (OrderObjective objective in this.objectives)
      {
        if (objective.IsComplete())
          ++completeObjectivesCount;
      }
      return completeObjectivesCount;
    }

    public virtual void ConfirmCompleteDonations()
    {
      foreach (OrderObjective objective in this.objectives)
      {
        if (objective is DonateObjective)
          (objective as DonateObjective).Confirm();
      }
    }

    public virtual void UpdateDonationCounts()
    {
      this._highlightLookup = (Dictionary<Item, bool>) null;
      int num1 = 0;
      int num2 = 0;
      foreach (OrderObjective objective in this.objectives)
      {
        if (objective is DonateObjective)
        {
          DonateObjective donateObjective = objective as DonateObjective;
          int new_count = 0;
          if (donateObjective.GetCount() >= donateObjective.GetMaxCount())
            ++num1;
          foreach (Item donatedItem in this.donatedItems)
          {
            if (donateObjective.IsValidItem(donatedItem))
              new_count += donatedItem.Stack;
          }
          donateObjective.SetCount(new_count);
          if (donateObjective.GetCount() >= donateObjective.GetMaxCount())
            ++num2;
        }
      }
      if (num2 <= num1)
        return;
      Game1.playSound("newArtifact");
    }

    public bool HighlightAcceptableItems(Item item)
    {
      if (this._highlightLookup != null && this._highlightLookup.ContainsKey(item))
        return this._highlightLookup[item];
      if (this._highlightLookup == null)
        this._highlightLookup = new Dictionary<Item, bool>();
      foreach (OrderObjective objective in this.objectives)
      {
        if (objective is DonateObjective && (objective as DonateObjective).GetAcceptCount(item, 1) > 0)
        {
          this._highlightLookup[item] = true;
          return true;
        }
      }
      this._highlightLookup[item] = false;
      return false;
    }

    public virtual int GetAcceptCount(Item item)
    {
      int acceptCount1 = 0;
      int stack = item.Stack;
      foreach (OrderObjective objective in this.objectives)
      {
        if (objective is DonateObjective)
        {
          int acceptCount2 = (objective as DonateObjective).GetAcceptCount(item, stack);
          stack -= acceptCount2;
          acceptCount1 += acceptCount2;
        }
      }
      return acceptCount1;
    }

    public static bool CheckTags(string tag_list)
    {
      if (tag_list == null)
        return true;
      List<string> stringList = new List<string>();
      foreach (string str in tag_list.Split(','))
        stringList.Add(str.Trim());
      foreach (string str in stringList)
      {
        string tag = str;
        if (tag.Length != 0)
        {
          bool flag = true;
          if (tag[0] == '!')
          {
            flag = false;
            tag = tag.Substring(1);
          }
          if (SpecialOrder.CheckTag(tag) != flag)
            return false;
        }
      }
      return true;
    }

    protected static bool CheckTag(string tag)
    {
      if (tag == "NOT_IMPLEMENTED")
        return false;
      if (tag.StartsWith("dropbox_"))
      {
        string box_id = tag.Substring("dropbox_".Length);
        foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
        {
          if (specialOrder.UsesDropBox(box_id))
            return true;
        }
      }
      if (tag.StartsWith("rule_") && Game1.player.team.SpecialOrderRuleActive(tag.Substring("rule_".Length)) || tag.StartsWith("completed_") && Game1.player.team.completedSpecialOrders.ContainsKey(tag.Substring("season_".Length)))
        return true;
      if (tag.StartsWith("season_"))
      {
        string str = tag.Substring("season_".Length);
        if (Game1.currentSeason == str)
          return true;
      }
      else if (tag.StartsWith("mail_"))
      {
        if (Game1.MasterPlayer.hasOrWillReceiveMail(tag.Substring("mail_".Length)))
          return true;
      }
      else if (tag.StartsWith("event_"))
      {
        if (Game1.MasterPlayer.eventsSeen.Contains(Convert.ToInt32(tag.Substring("event_".Length))))
          return true;
      }
      else
      {
        if (tag == "island")
          return Utility.doesAnyFarmerHaveOrWillReceiveMail("seenBoatJourney");
        if (tag.StartsWith("knows_"))
        {
          string key = tag.Substring("knows_".Length);
          foreach (Farmer allFarmer in Game1.getAllFarmers())
          {
            if (allFarmer.friendshipData.ContainsKey(key))
              return true;
          }
        }
      }
      return false;
    }

    public bool IsIslandOrder()
    {
      if (this._isIslandOrder == -1)
      {
        Dictionary<string, SpecialOrderData> dictionary = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
        if (dictionary.ContainsKey(this.questKey.Value))
          this._isIslandOrder = !dictionary[this.questKey.Value].RequiredTags.Contains("island") ? 0 : 1;
      }
      return this._isIslandOrder == 1;
    }

    public static bool IsSpecialOrdersBoardUnlocked() => Game1.stats.DaysPlayed >= 58U;

    public static void UpdateAvailableSpecialOrders(bool force_refresh)
    {
      if (Game1.player.team.availableSpecialOrders != null)
      {
        foreach (SpecialOrder availableSpecialOrder in Game1.player.team.availableSpecialOrders)
        {
          if ((availableSpecialOrder.questDuration.Value == SpecialOrder.QuestDuration.TwoDays || availableSpecialOrder.questDuration.Value == SpecialOrder.QuestDuration.ThreeDays) && !Game1.player.team.acceptedSpecialOrderTypes.Contains(availableSpecialOrder.orderType.Value))
            availableSpecialOrder.SetDuration((SpecialOrder.QuestDuration) (NetFieldBase<SpecialOrder.QuestDuration, NetEnum<SpecialOrder.QuestDuration>>) availableSpecialOrder.questDuration);
        }
      }
      if (Game1.player.team.availableSpecialOrders.Count > 0 && !force_refresh)
        return;
      Game1.player.team.availableSpecialOrders.Clear();
      Game1.player.team.acceptedSpecialOrderTypes.Clear();
      Dictionary<string, SpecialOrderData> dictionary = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
      List<string> stringList = new List<string>((IEnumerable<string>) dictionary.Keys);
      for (int index = 0; index < stringList.Count; ++index)
      {
        string key = stringList[index];
        bool flag = false;
        if (!flag && dictionary[key].Repeatable != "True" && Game1.MasterPlayer.team.completedSpecialOrders.ContainsKey(key))
          flag = true;
        if (Game1.dayOfMonth >= 16 && dictionary[key].Duration == "Month")
          flag = true;
        if (!flag && !SpecialOrder.CheckTags(dictionary[key].RequiredTags))
          flag = true;
        if (!flag)
        {
          foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
          {
            if ((string) (NetFieldBase<string, NetString>) specialOrder.questKey == key)
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
        {
          stringList.RemoveAt(index);
          --index;
        }
      }
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) ((double) Game1.stats.DaysPlayed * 1.29999995231628));
      Game1.player.team.availableSpecialOrders.Clear();
      string[] strArray = new string[2]{ "", "Qi" };
      foreach (string str in strArray)
      {
        List<string> collection1 = new List<string>();
        foreach (string key in stringList)
        {
          if (dictionary[key].OrderType == str)
            collection1.Add(key);
        }
        List<string> collection2 = new List<string>((IEnumerable<string>) collection1);
        if (str != "Qi")
        {
          for (int index = 0; index < collection1.Count; ++index)
          {
            if (Game1.player.team.completedSpecialOrders.ContainsKey(collection1[index]))
            {
              collection1.RemoveAt(index);
              --index;
            }
          }
        }
        for (int index1 = 0; index1 < 2; ++index1)
        {
          if (collection1.Count == 0)
          {
            if (collection2.Count != 0)
              collection1 = new List<string>((IEnumerable<string>) collection2);
            else
              break;
          }
          int index2 = random.Next(collection1.Count);
          string key = collection1[index2];
          Game1.player.team.availableSpecialOrders.Add(SpecialOrder.GetSpecialOrder(key, new int?(random.Next())));
          collection1.Remove(key);
          collection2.Remove(key);
        }
      }
    }

    public static SpecialOrder GetSpecialOrder(string key, int? generation_seed)
    {
      Dictionary<string, SpecialOrderData> dictionary = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
      if (!generation_seed.HasValue)
        generation_seed = new int?(Game1.random.Next());
      if (!dictionary.ContainsKey(key))
        return (SpecialOrder) null;
      Random random1 = new Random(generation_seed.Value);
      SpecialOrderData specialOrderData = dictionary[key];
      SpecialOrder order = new SpecialOrder();
      order.generationSeed.Value = generation_seed.Value;
      order._orderData = specialOrderData;
      order.questKey.Value = key;
      order.questName.Value = specialOrderData.Name;
      order.requester.Value = specialOrderData.Requester;
      order.orderType.Value = specialOrderData.OrderType.Trim();
      order.specialRule.Value = specialOrderData.SpecialRule.Trim();
      if (specialOrderData.ItemToRemoveOnEnd != null)
      {
        int result = -1;
        if (int.TryParse(specialOrderData.ItemToRemoveOnEnd, out result))
          order.itemToRemoveOnEnd.Value = result;
      }
      if (specialOrderData.MailToRemoveOnEnd != null)
        order.mailToRemoveOnEnd.Value = specialOrderData.MailToRemoveOnEnd;
      order.selectedRandomElements.Clear();
      if (specialOrderData.RandomizedElements != null)
      {
        foreach (RandomizedElement randomizedElement in specialOrderData.RandomizedElements)
        {
          List<int> list1 = new List<int>();
          for (int index = 0; index < randomizedElement.Values.Count; ++index)
          {
            if (SpecialOrder.CheckTags(randomizedElement.Values[index].RequiredTags))
              list1.Add(index);
          }
          int random2 = Utility.GetRandom<int>(list1, random1);
          order.selectedRandomElements[randomizedElement.Name] = random2;
          string str1 = randomizedElement.Values[random2].Value;
          if (str1.StartsWith("PICK_ITEM"))
          {
            string[] strArray = str1.Substring("PICK_ITEM".Length).Split(',');
            List<int> list2 = new List<int>();
            foreach (string str2 in strArray)
            {
              string str3 = str2.Trim();
              if (str3.Length != 0)
              {
                if (char.IsDigit(str3[0]))
                {
                  int result = -1;
                  if (int.TryParse(str3, out result))
                    list2.Add(result);
                }
                else
                {
                  Item obj = Utility.fuzzyItemSearch(str3);
                  if (Utility.IsNormalObjectAtParentSheetIndex(obj, obj.ParentSheetIndex))
                    list2.Add(obj.ParentSheetIndex);
                }
              }
            }
            order.preSelectedItems[randomizedElement.Name] = Utility.GetRandom<int>(list2, random1);
          }
        }
      }
      if (specialOrderData.Duration == "Month")
        order.SetDuration(SpecialOrder.QuestDuration.Month);
      else if (specialOrderData.Duration == "TwoWeeks")
        order.SetDuration(SpecialOrder.QuestDuration.TwoWeeks);
      else if (specialOrderData.Duration == "TwoDays")
        order.SetDuration(SpecialOrder.QuestDuration.TwoDays);
      else if (specialOrderData.Duration == "ThreeDays")
        order.SetDuration(SpecialOrder.QuestDuration.ThreeDays);
      else
        order.SetDuration(SpecialOrder.QuestDuration.Week);
      order.questDescription.Value = specialOrderData.Text;
      foreach (SpecialOrderObjectiveData objective in specialOrderData.Objectives)
      {
        Type type = Type.GetType("StardewValley." + objective.Type.Trim() + "Objective");
        if (!(type == (Type) null) && type.IsSubclassOf(typeof (OrderObjective)))
        {
          OrderObjective instance = (OrderObjective) Activator.CreateInstance(type);
          if (instance != null)
          {
            instance.description.Value = order.Parse(objective.Text);
            instance.maxCount.Value = int.Parse(order.Parse(objective.RequiredCount));
            instance.Load(order, objective.Data);
            order.objectives.Add(instance);
          }
        }
      }
      foreach (SpecialOrderRewardData reward in specialOrderData.Rewards)
      {
        Type type = Type.GetType("StardewValley." + reward.Type.Trim() + "Reward");
        if (!(type == (Type) null) && type.IsSubclassOf(typeof (OrderReward)))
        {
          OrderReward instance = (OrderReward) Activator.CreateInstance(type);
          if (instance != null)
          {
            instance.Load(order, reward.Data);
            order.rewards.Add(instance);
          }
        }
      }
      return order;
    }

    public virtual string MakeLocalizationReplacements(string data)
    {
      data = data.Trim();
      int startIndex;
      do
      {
        startIndex = data.LastIndexOf('[');
        if (startIndex >= 0)
        {
          int num = data.IndexOf(']', startIndex);
          if (num == -1)
            return data;
          string str1 = data.Substring(startIndex + 1, num - startIndex - 1);
          string str2 = Game1.content.LoadString("Strings\\SpecialOrderStrings:" + str1);
          data = data.Remove(startIndex, num - startIndex + 1);
          data = data.Insert(startIndex, str2);
        }
      }
      while (startIndex >= 0);
      return data;
    }

    public virtual string Parse(string data)
    {
      data = data.Trim();
      this.GetData();
      data = this.MakeLocalizationReplacements(data);
      int startIndex;
      do
      {
        startIndex = data.LastIndexOf('{');
        if (startIndex >= 0)
        {
          int num = data.IndexOf('}', startIndex);
          if (num == -1)
            return data;
          string str1 = data.Substring(startIndex + 1, num - startIndex - 1);
          string str2 = str1;
          string key = str1;
          string str3 = (string) null;
          if (str1.Contains(":"))
          {
            string[] strArray = str1.Split(':');
            key = strArray[0];
            if (strArray.Length > 1)
              str3 = strArray[1];
          }
          if (this._orderData.RandomizedElements != null)
          {
            if (this.preSelectedItems.ContainsKey(key))
            {
              Object @object = new Object(Vector2.Zero, this.preSelectedItems[key], 0);
              if (str3 == "Text")
                str2 = @object.DisplayName;
              else if (str3 == "TextPlural")
                str2 = Lexicon.makePlural(@object.DisplayName);
              else if (str3 == "TextPluralCapitalized")
                str2 = Utility.capitalizeFirstLetter(Lexicon.makePlural(@object.DisplayName));
              else if (str3 == "Tags")
              {
                string str4 = "id_" + Utility.getStandardDescriptionFromItem((Item) @object, 0, '_');
                str2 = str4.Substring(0, str4.Length - 2).ToLower();
              }
              else if (str3 == "Price")
                str2 = @object.sellToStorePrice().ToString() ?? "";
            }
            else if (this.selectedRandomElements.ContainsKey(key))
            {
              foreach (RandomizedElement randomizedElement in this._orderData.RandomizedElements)
              {
                if (randomizedElement.Name == key)
                {
                  str2 = this.MakeLocalizationReplacements(randomizedElement.Values[this.selectedRandomElements[key]].Value);
                  break;
                }
              }
            }
          }
          if (str3 != null)
          {
            string[] strArray = str2.Split('|');
            for (int index = 0; index < strArray.Length; index += 2)
            {
              if (index + 1 <= strArray.Length && strArray[index] == str3)
              {
                str2 = strArray[index + 1];
                break;
              }
            }
          }
          data = data.Remove(startIndex, num - startIndex + 1);
          data = data.Insert(startIndex, str2);
        }
      }
      while (startIndex >= 0);
      return data;
    }

    public virtual SpecialOrderData GetData()
    {
      if (this._orderData == null)
      {
        Dictionary<string, SpecialOrderData> dictionary = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
        if (dictionary.ContainsKey(this.questKey.Value))
          this._orderData = dictionary[this.questKey.Value];
      }
      return this._orderData;
    }

    public virtual void InitializeNetFields()
    {
      this.NetFields.AddFields((INetSerializable) this.questName, (INetSerializable) this.questDescription, (INetSerializable) this.dueDate, (INetSerializable) this.objectives, (INetSerializable) this.rewards, (INetSerializable) this.questState, (INetSerializable) this.donatedItems, (INetSerializable) this.questKey, (INetSerializable) this.requester, (INetSerializable) this.generationSeed, (INetSerializable) this.selectedRandomElements, (INetSerializable) this.preSelectedItems, (INetSerializable) this.orderType, (INetSerializable) this.specialRule, (INetSerializable) this.participants, (INetSerializable) this.seenParticipants, (INetSerializable) this.unclaimedRewards, (INetSerializable) this.donateMutex.NetFields, (INetSerializable) this.itemToRemoveOnEnd, (INetSerializable) this.mailToRemoveOnEnd, (INetSerializable) this.questDuration, (INetSerializable) this.readyForRemoval);
      this.objectives.OnArrayReplaced += (NetList<OrderObjective, NetRef<OrderObjective>>.ArrayReplacedEvent) ((a, b, c) => this._objectiveRegistrationDirty = true);
      this.objectives.OnElementChanged += (NetList<OrderObjective, NetRef<OrderObjective>>.ElementChangedEvent) ((a, int_index, old_value, new_value) => this._objectiveRegistrationDirty = true);
    }

    protected virtual void _UpdateObjectiveRegistration()
    {
      for (int index = 0; index < this._registeredObjectives.Count; ++index)
      {
        OrderObjective registeredObjective = this._registeredObjectives[index];
        if (!this.objectives.Contains(registeredObjective))
          registeredObjective.Unregister();
      }
      foreach (OrderObjective objective in this.objectives)
      {
        if (!this._registeredObjectives.Contains(objective))
        {
          objective.Register(this);
          this._registeredObjectives.Add(objective);
        }
      }
    }

    public bool UsesDropBox(string box_id)
    {
      if (this.questState.Value != SpecialOrder.QuestState.InProgress)
        return false;
      foreach (OrderObjective objective in this.objectives)
      {
        if (objective is DonateObjective && (objective as DonateObjective).dropBox.Value == box_id)
          return true;
      }
      return false;
    }

    public int GetMinimumDropBoxCapacity(string box_id)
    {
      int val1 = 9;
      foreach (OrderObjective objective in this.objectives)
      {
        if (objective is DonateObjective && (objective as DonateObjective).dropBox.Value == box_id && (objective as DonateObjective).minimumCapacity.Value > 0)
          val1 = Math.Max(val1, (int) (NetFieldBase<int, NetInt>) (objective as DonateObjective).minimumCapacity);
      }
      return val1;
    }

    public virtual void Update()
    {
      this._AddSpecialRulesIfNecessary();
      if (this._objectiveRegistrationDirty)
      {
        this._objectiveRegistrationDirty = false;
        this._UpdateObjectiveRegistration();
      }
      if (!this.readyForRemoval.Value)
      {
        if (this.questState.Value == SpecialOrder.QuestState.InProgress && !this.participants.ContainsKey(Game1.player.UniqueMultiplayerID))
          this.participants[Game1.player.UniqueMultiplayerID] = true;
        else if (this.questState.Value == SpecialOrder.QuestState.Complete)
        {
          if (this.unclaimedRewards.ContainsKey(Game1.player.UniqueMultiplayerID))
          {
            this.unclaimedRewards.Remove(Game1.player.UniqueMultiplayerID);
            ++Game1.stats.QuestsCompleted;
            Game1.playSound("questcomplete");
            Game1.dayTimeMoneyBox.questsDirty = true;
            foreach (OrderReward reward in this.rewards)
              reward.Grant();
          }
          if (this.participants.ContainsKey(Game1.player.UniqueMultiplayerID) && this.GetMoneyReward() <= 0)
            this.RemoveFromParticipants();
        }
      }
      this.donateMutex.Update(Game1.getOnlineFarmers());
      if (this.donateMutex.IsLockHeld() && Game1.activeClickableMenu == null)
        this.donateMutex.ReleaseLock();
      if (Game1.activeClickableMenu == null)
        this._highlightLookup = (Dictionary<Item, bool>) null;
      if (!Game1.IsMasterGame || this.questState.Value == SpecialOrder.QuestState.InProgress)
        return;
      this.MarkForRemovalIfEmpty();
      if (!this.readyForRemoval.Value)
        return;
      this._RemoveSpecialRuleIfNecessary();
      Game1.player.team.specialOrders.Remove(this);
    }

    public virtual void RemoveFromParticipants()
    {
      this.participants.Remove(Game1.player.UniqueMultiplayerID);
      this.MarkForRemovalIfEmpty();
    }

    public virtual void MarkForRemovalIfEmpty()
    {
      if (this.participants.Count() != 0)
        return;
      this.readyForRemoval.Value = true;
    }

    public virtual void HostHandleQuestEnd()
    {
      if (!Game1.IsMasterGame)
        return;
      if (this.itemToRemoveOnEnd.Value >= 0 && !Game1.player.team.itemsToRemoveOvernight.Contains(this.itemToRemoveOnEnd.Value))
        Game1.player.team.itemsToRemoveOvernight.Add(this.itemToRemoveOnEnd.Value);
      if (this.mailToRemoveOnEnd.Value == null || Game1.player.team.mailToRemoveOvernight.Contains(this.mailToRemoveOnEnd.Value))
        return;
      Game1.player.team.mailToRemoveOvernight.Add(this.mailToRemoveOnEnd.Value);
    }

    protected void _AddSpecialRulesIfNecessary()
    {
      if (!Game1.IsMasterGame || this.appliedSpecialRules || this.questState.Value != SpecialOrder.QuestState.InProgress)
        return;
      this.appliedSpecialRules = true;
      foreach (string str1 in this.specialRule.Value.Split(','))
      {
        string str2 = str1.Trim();
        if (!Game1.player.team.SpecialOrderRuleActive(str2, this))
        {
          this.AddSpecialRule(str2);
          if (Game1.player.team.specialRulesRemovedToday.Contains(str2))
            Game1.player.team.specialRulesRemovedToday.Remove(str2);
        }
      }
    }

    protected void _RemoveSpecialRuleIfNecessary()
    {
      if (!Game1.IsMasterGame || !this.appliedSpecialRules)
        return;
      this.appliedSpecialRules = false;
      foreach (string str1 in this.specialRule.Value.Split(','))
      {
        string str2 = str1.Trim();
        if (!Game1.player.team.SpecialOrderRuleActive(str2, this))
        {
          this.RemoveSpecialRule(str2);
          if (!Game1.player.team.specialRulesRemovedToday.Contains(str2))
            Game1.player.team.specialRulesRemovedToday.Add(str2);
        }
      }
    }

    public virtual void AddSpecialRule(string rule)
    {
      if (rule == "MINE_HARD")
      {
        ++Game1.netWorldState.Value.MinesDifficulty;
        Game1.player.team.kickOutOfMinesEvent.Fire();
        Game1.netWorldState.Value.LowestMineLevelForOrder = 0;
      }
      else
      {
        if (!(rule == "SC_HARD"))
          return;
        ++Game1.netWorldState.Value.SkullCavesDifficulty;
        Game1.player.team.kickOutOfMinesEvent.Fire();
      }
    }

    public static void RemoveSpecialRuleAtEndOfDay(string rule)
    {
      if (rule == "MINE_HARD")
      {
        if (Game1.netWorldState.Value.MinesDifficulty > 0)
          --Game1.netWorldState.Value.MinesDifficulty;
        Game1.netWorldState.Value.LowestMineLevelForOrder = -1;
      }
      else if (rule == "SC_HARD")
      {
        if (Game1.netWorldState.Value.SkullCavesDifficulty <= 0)
          return;
        --Game1.netWorldState.Value.SkullCavesDifficulty;
      }
      else
      {
        if (!(rule == "QI_COOKING"))
          return;
        Utility.iterateAllItems((Action<Item>) (item =>
        {
          if (!(item is Object) || !((item as Object).orderData.Value == "QI_COOKING"))
            return;
          (item as Object).orderData.Value = (string) null;
          item.MarkContextTagsDirty();
        }));
      }
    }

    public virtual void RemoveSpecialRule(string rule)
    {
      if (!(rule == "QI_BEANS"))
        return;
      Game1.player.team.itemsToRemoveOvernight.Add(890);
      Game1.player.team.itemsToRemoveOvernight.Add(889);
    }

    public virtual bool HasMoneyReward() => this.questState.Value == SpecialOrder.QuestState.Complete && this.GetMoneyReward() > 0 && this.participants.ContainsKey(Game1.player.UniqueMultiplayerID);

    public virtual void Fail()
    {
    }

    public virtual void AddObjective(OrderObjective objective) => this.objectives.Add(objective);

    public void CheckCompletion()
    {
      if (this.questState.Value != SpecialOrder.QuestState.InProgress)
        return;
      foreach (OrderObjective objective in this.objectives)
      {
        if ((bool) (NetFieldBase<bool, NetBool>) objective.failOnCompletion && objective.IsComplete())
        {
          this.OnFail();
          return;
        }
      }
      foreach (OrderObjective objective in this.objectives)
      {
        if (!(bool) (NetFieldBase<bool, NetBool>) objective.failOnCompletion && !objective.IsComplete())
          return;
      }
      if (!Game1.IsMasterGame)
        return;
      foreach (long key in this.participants.Keys)
      {
        if (!this.unclaimedRewards.ContainsKey(key))
          this.unclaimedRewards[key] = true;
      }
      Game1.multiplayer.globalChatInfoMessage("CompletedSpecialOrder", this.GetName());
      this.HostHandleQuestEnd();
      Game1.player.team.completedSpecialOrders[this.questKey.Value] = true;
      this.questState.Value = SpecialOrder.QuestState.Complete;
      this._RemoveSpecialRuleIfNecessary();
    }

    public override string ToString()
    {
      string str = "";
      foreach (OrderObjective objective in this.objectives)
      {
        str += (string) (NetFieldBase<string, NetString>) objective.description;
        if (objective.GetMaxCount() > 1)
        {
          string[] strArray = new string[6]
          {
            str,
            " (",
            null,
            null,
            null,
            null
          };
          int num = objective.GetCount();
          strArray[2] = num.ToString();
          strArray[3] = "/";
          num = objective.GetMaxCount();
          strArray[4] = num.ToString();
          strArray[5] = ")";
          str = string.Concat(strArray);
        }
        str += "\n";
      }
      return str.Trim();
    }

    public string GetName()
    {
      if (this._localizedName == null)
        this._localizedName = this.MakeLocalizationReplacements(this.questName.Value);
      return this._localizedName;
    }

    public string GetDescription()
    {
      if (this._localizedDescription == null)
        this._localizedDescription = this.Parse(this.questDescription.Value).Trim();
      return this._localizedDescription;
    }

    public List<string> GetObjectiveDescriptions()
    {
      List<string> objectiveDescriptions = new List<string>();
      foreach (OrderObjective objective in this.objectives)
        objectiveDescriptions.Add(this.Parse(objective.GetDescription()));
      return objectiveDescriptions;
    }

    public bool CanBeCancelled() => false;

    public void MarkAsViewed()
    {
      if (this.seenParticipants.ContainsKey(Game1.player.UniqueMultiplayerID))
        return;
      this.seenParticipants[Game1.player.UniqueMultiplayerID] = true;
    }

    public bool IsHidden() => !this.participants.ContainsKey(Game1.player.UniqueMultiplayerID);

    public bool ShouldDisplayAsNew() => !this.seenParticipants.ContainsKey(Game1.player.UniqueMultiplayerID);

    public bool HasReward() => this.HasMoneyReward();

    public int GetMoneyReward()
    {
      if (this._moneyReward == -1)
      {
        this._moneyReward = 0;
        foreach (OrderReward reward in this.rewards)
        {
          if (reward is MoneyReward)
            this._moneyReward += (reward as MoneyReward).GetRewardMoneyAmount();
        }
      }
      return this._moneyReward;
    }

    public bool ShouldDisplayAsComplete() => (uint) this.questState.Value > 0U;

    public bool IsTimedQuest() => true;

    public int GetDaysLeft() => this.questState.Value != SpecialOrder.QuestState.InProgress ? 0 : (int) (NetFieldBase<int, NetInt>) this.dueDate - Game1.Date.TotalDays;

    public void OnMoneyRewardClaimed()
    {
      this.participants.Remove(Game1.player.UniqueMultiplayerID);
      this.MarkForRemovalIfEmpty();
    }

    public bool OnLeaveQuestPage()
    {
      if (this.participants.ContainsKey(Game1.player.UniqueMultiplayerID))
        return false;
      this.MarkForRemovalIfEmpty();
      return true;
    }

    public enum QuestState
    {
      InProgress,
      Complete,
      Failed,
    }

    public enum QuestDuration
    {
      Week,
      Month,
      TwoWeeks,
      TwoDays,
      ThreeDays,
    }
  }
}
