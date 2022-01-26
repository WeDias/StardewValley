// Decompiled with JetBrains decompiler
// Type: StardewValley.GiftObjective
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class GiftObjective : OrderObjective
  {
    [XmlElement("acceptableContextTagSets")]
    public NetStringList acceptableContextTagSets = new NetStringList();
    [XmlElement("minimumLikeLevel")]
    public NetEnum<GiftObjective.LikeLevels> minimumLikeLevel = new NetEnum<GiftObjective.LikeLevels>(GiftObjective.LikeLevels.None);

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      if (data.ContainsKey("AcceptedContextTags"))
        this.acceptableContextTagSets.Add(order.Parse(data["AcceptedContextTags"]));
      if (!data.ContainsKey("MinimumLikeLevel"))
        return;
      this.minimumLikeLevel.Value = (GiftObjective.LikeLevels) Enum.Parse(typeof (GiftObjective.LikeLevels), data["MinimumLikeLevel"]);
    }

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.acceptableContextTagSets, (INetSerializable) this.minimumLikeLevel);
    }

    protected override void _Register()
    {
      base._Register();
      this._order.onGiftGiven += new Action<Farmer, NPC, Item>(this.OnGiftGiven);
    }

    protected override void _Unregister()
    {
      base._Unregister();
      this._order.onGiftGiven -= new Action<Farmer, NPC, Item>(this.OnGiftGiven);
    }

    public virtual void OnGiftGiven(Farmer farmer, NPC npc, Item item)
    {
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
        return;
      if (this.minimumLikeLevel.Value > GiftObjective.LikeLevels.None)
      {
        int tasteForThisItem = npc.getGiftTasteForThisItem(item);
        GiftObjective.LikeLevels likeLevels = GiftObjective.LikeLevels.None;
        switch (tasteForThisItem)
        {
          case 0:
            likeLevels = GiftObjective.LikeLevels.Loved;
            break;
          case 2:
            likeLevels = GiftObjective.LikeLevels.Liked;
            break;
          case 4:
            likeLevels = GiftObjective.LikeLevels.Disliked;
            break;
          case 6:
            likeLevels = GiftObjective.LikeLevels.Hated;
            break;
          case 8:
            likeLevels = GiftObjective.LikeLevels.Neutral;
            break;
        }
        if (likeLevels < this.minimumLikeLevel.Value)
          return;
      }
      this.IncrementCount(1);
    }

    public enum LikeLevels
    {
      None,
      Hated,
      Disliked,
      Neutral,
      Liked,
      Loved,
    }
  }
}
