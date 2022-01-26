// Decompiled with JetBrains decompiler
// Type: StardewValley.DeliverObjective
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class DeliverObjective : OrderObjective
  {
    [XmlElement("acceptableContextTagSets")]
    public NetStringList acceptableContextTagSets = new NetStringList();
    [XmlElement("targetName")]
    public NetString targetName = new NetString();
    [XmlElement("message")]
    public NetString message = new NetString();

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      if (data.ContainsKey("AcceptedContextTags"))
        this.acceptableContextTagSets.Add(order.Parse(data["AcceptedContextTags"]));
      if (data.ContainsKey("TargetName"))
        this.targetName.Value = order.Parse(data["TargetName"]);
      else
        this.targetName.Value = this._order.requester.Value;
      if (data.ContainsKey("Message"))
        this.message.Value = order.Parse(data["Message"]);
      else
        this.message.Value = "";
    }

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.acceptableContextTagSets, (INetSerializable) this.targetName, (INetSerializable) this.message);
    }

    public override bool ShouldShowProgress() => false;

    protected override void _Register()
    {
      base._Register();
      this._order.onItemDelivered += new Func<Farmer, NPC, Item, int>(this.OnItemDelivered);
    }

    protected override void _Unregister()
    {
      base._Unregister();
      this._order.onItemDelivered -= new Func<Farmer, NPC, Item, int>(this.OnItemDelivered);
    }

    public virtual int OnItemDelivered(Farmer farmer, NPC npc, Item item)
    {
      if (this.IsComplete() || npc.Name != this.targetName.Value)
        return 0;
      bool flag1 = true;
      foreach (string acceptableContextTagSet in (NetList<string, NetString>) this.acceptableContextTagSets)
      {
        flag1 = false;
        bool flag2 = false;
        foreach (string str1 in acceptableContextTagSet.Split(','))
        {
          bool flag3 = false;
          foreach (string str2 in str1.Split('/'))
          {
            if (item.HasContextTag(str2.Trim()))
            {
              flag3 = true;
              break;
            }
          }
          if (!flag3)
            flag2 = true;
        }
        if (!flag2)
        {
          flag1 = true;
          break;
        }
      }
      if (!flag1)
        return 0;
      int val2 = this.GetMaxCount() - this.GetCount();
      int amount = Math.Min(item.Stack, val2);
      if (amount < val2)
        return 0;
      Item one = item.getOne();
      one.Stack = amount;
      this._order.donatedItems.Add(one);
      item.Stack -= amount;
      this.IncrementCount(amount);
      if (!string.IsNullOrEmpty(this.message.Value))
      {
        npc.CurrentDialogue.Push(new Dialogue(this.message.Value, npc));
        Game1.drawDialogue(npc);
      }
      return amount;
    }
  }
}
